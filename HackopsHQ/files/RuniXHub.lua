local OrionLib = loadstring(game:HttpGet(('https://raw.githubusercontent.com/shlexware/Orion/main/source')))()

local Window = OrionLib:MakeWindow({
    Name = "RuniX Hub",
    HidePremium = false,
    SaveConfig = true,
    ConfigFolder = "RuniXHub",
    IntroEnabled = true,
    IntroText = "Welcome to RuniX Hub!",
})

local PlayerTab = Window:MakeTab({
    Name = "Player",
    Icon = "rbxassetid://4483345998",
    PremiumOnly = false
})

local PlayerSection = PlayerTab:AddSection({
    Name = "Player"
})

OrionLib:MakeNotification({
    Name = "RuniX Hub Loaded!",
    Content = "Thanks for using!",
    Image = "rbxassetid://4483345998", -- Replace with a relevant image ID if needed
    Time = 5 -- Notification duration in seconds
})

-- Declare variables to store slider objects
local WalkSpeedSlider, JumpPowerSlider, GravitySlider, FOVSlider

-- Add sliders and store them in variables
WalkSpeedSlider = PlayerTab:AddSlider({
    Name = "Walk Speed",
    Min = 10,
    Max = 500,
    Default = 16,
    Color = Color3.fromRGB(255,255,255),
    Increment = 1,
    ValueName = "Speed",
    Callback = function(Value)
        game.Players.LocalPlayer.Character.Humanoid.WalkSpeed = Value
    end    
})

JumpPowerSlider = PlayerTab:AddSlider({
    Name = "Jump Power",
    Min = 10,
    Max = 500,
    Default = 50,
    Color = Color3.fromRGB(255,255,255),
    Increment = 1,
    ValueName = "Jump",
    Callback = function(Value)
        game.Players.LocalPlayer.Character.Humanoid.JumpPower = Value
    end    
})

GravitySlider = PlayerTab:AddSlider({
    Name = "Gravity",
    Min = 0,
    Max = 500,
    Default = 196,
    Color = Color3.fromRGB(255,255,255),
    Increment = 10,
    ValueName = "Gravity",
    Callback = function(Value)
        workspace.Gravity = Value
    end    
})

FOVSlider = PlayerTab:AddSlider({
    Name = "Field of View",
    Min = 0,
    Max = 120,
    Default = 70,
    Color = Color3.fromRGB(255,255,255),
    Increment = 1,
    ValueName = "FOV",
    Callback = function(Value)
        game:GetService("Workspace").CurrentCamera.FieldOfView = Value
    end    
})

-- Add reset button
PlayerTab:AddButton({
    Name = "Reset All Values",
    Callback = function()
        -- Reset sliders to default values
        WalkSpeedSlider:Set(16)
        JumpPowerSlider:Set(50)
        GravitySlider:Set(196)
        FOVSlider:Set(70)
    end    
})

-- Scripts Tab
local ScriptsTab = Window:MakeTab({
    Name = "Scripts",
    Icon = "rbxassetid://4483345998",
    PremiumOnly = false
})

local ScriptsSection = ScriptsTab:AddSection({
    Name = "Scripts"
})

-- Function to create script buttons
local function addScriptButton(name, url)
    ScriptsTab:AddButton({
        Name = name,
        Callback = function()
            loadstring(game:HttpGet(url))()
        end    
    })
end

-- Add script buttons
addScriptButton("Load Infinite Yield", "https://raw.githubusercontent.com/EdgeIY/infiniteyield/master/source")
addScriptButton("RIVALS | BEST", "https://raw.githubusercontent.com/ShadowByte098/Hub/refs/heads/main/R")
addScriptButton("FISCH | BEST", "https://you.whimper.xyz/sources/CupPink/fisch.lua")
addScriptButton("Be Npc Or Die | BEST", "https://raw.githubusercontent.com/ShadowByte098/Hub/refs/heads/main/npc")
addScriptButton("Simple Aimbot", "https://raw.githubusercontent.com/Stsbba/UNC-B/refs/heads/main/A%20super%20simple%20aimbot")
addScriptButton("AirHub | Aimbot", "https://raw.githubusercontent.com/ShadowByte098/Hub/refs/heads/main/air")
addScriptButton("NeverPatched | ChatBypass TROLL", "https://rawscripts.net/raw/Universal-Script-NeverPatched-Bypass-official-21146")
addScriptButton("Prison Life FE TROLL", "https://raw.githubusercontent.com/g0fuzzyprisonfe/main/main.lua")
addScriptButton("Front and Backflip TROLL", "https://raw.githubusercontent.com/ShadowByte098/Hub/refs/heads/main/FB")
addScriptButton("Freaky Jerk Off Script TROLL", "https://pastefy.app/YZoglOyJ/raw")
addScriptButton("Vape V4 | BedWars (might not work)", "https://raw.githubusercontent.com/7GrandDadPGN/VapeV4ForRoblox/main/NewMainScript.lua")

-- X-Ray Vision
local xrayEnabled = false
local rainbowSpeed = 0.2

local function getRainbowColor(t)
    local hue = (t * rainbowSpeed) % 1
    return Color3.fromHSV(hue, 1, 1)
end

local function addHighlightToCharacter(character)
    if not character then return end
    local highlight = character:FindFirstChild("Highlight")
    if not highlight then
        highlight = Instance.new("Highlight")
        highlight.FillTransparency = 0.5
        highlight.OutlineTransparency = 1
        highlight.Parent = character
    end
end

local function toggleXray(enable)
    xrayEnabled = enable

    if enable then
        for _, player in pairs(game.Players:GetPlayers()) do
            if player ~= game.Players.LocalPlayer and player.Character then
                addHighlightToCharacter(player.Character)
            end
        end
    else
        for _, player in pairs(game.Players:GetPlayers()) do
            if player.Character then
                local highlight = player.Character:FindFirstChild("Highlight")
                if highlight then
                    highlight:Destroy()
                end
            end
        end
    end
end

game:GetService("RunService").RenderStepped:Connect(function()
    if xrayEnabled then
        local rainbowColor = getRainbowColor(tick())
        for _, player in pairs(game.Players:GetPlayers()) do
            if player.Character then
                local highlight = player.Character:FindFirstChild("Highlight")
                if highlight then
                    highlight.FillColor = rainbowColor
                end
            end
        end
    end
end)

game.Players.PlayerAdded:Connect(function(player)
    player.CharacterAdded:Connect(function(character)
        if xrayEnabled then
            addHighlightToCharacter(character)
        end
    end)
end)

for _, player in pairs(game.Players:GetPlayers()) do
    player.CharacterAdded:Connect(function(character)
        if xrayEnabled then
            addHighlightToCharacter(character)
        end
    end)
end

ScriptsTab:AddToggle({
    Name = "Enable ESP",
    Default = false,
    Callback = function(value)
        toggleXray(value)
    end
})

OrionLib:Init()