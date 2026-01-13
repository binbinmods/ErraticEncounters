// These are your imports, mostly you'll be needing these 5 for every plugin. Some will need more.

using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using static Obeliskial_Essentials.CardDescriptionNew;
using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
// using static ErraticEncounters.EssentialsCompatibility;


// The Plugin csharp file is used to specify some general info about your plugin. and set up things for 


// Make sure all your files have the same namespace and this namespace matches the RootNamespace in the .csproj file
// All files that are in the same namespace are compiled together and can "see" each other more easily.

namespace ErraticEncounters
{
    // These are used to create the actual plugin. If you don't need Obeliskial Essentials for your mod, 
    // delete the BepInDependency and the associated code "RegisterMod()" below.

    // If you have other dependencies, such as obeliskial content, make sure to include them here.
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials", BepInDependency.DependencyFlags.SoftDependency)] // this is the name of the .dll in the !libs folder.
    [BepInProcess("AcrossTheObelisk.exe")] //Don't change this

    // If PluginInfo isn't working, you are either:
    // 1. Using BepInEx v6
    // 2. Have an issue with your csproj file (not loading the analyzer or BepInEx appropriately)
    // 3. You have an issue with your solution file (not referencing the correct csproj file)


    public class Plugin : BaseUnityPlugin
    {

        // If desired, you can create configs for users by creating a ConfigEntry object here, 
        // Configs allows users to specify certain things about the mod. 
        // The most common would be a flag to enable/disable portions of the mod or the entire mod.

        // You can use: config = Config.Bind() to set the title, default value, and description of the config.
        // It automatically creates the appropriate configs.


        public static ConfigEntry<bool> EnableMod { get; set; }
        public static ConfigEntry<bool> EnableDebugging { get; set; }
        public static ConfigEntry<bool> EnableDLCMode { get; set; }
        public static ConfigEntry<bool> DisablePositioning { get; set; }
        // public static ConfigEntry<bool> EnableChampions { get; set; }
        public static ConfigEntry<bool> EnableBossesInHallway { get; set; }
        public static ConfigEntry<bool> EnableBossRandomization { get; set; }
        public static ConfigEntry<bool> EnableEventCombatRandomization { get; set; }
        public static ConfigEntry<bool> IncludeOCBosses { get; set; }

        public static string PluginName;
        public static string PluginVersion;
        public static string PluginGUID;

        internal static int ModDate = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;


        public static string debugBase = $"{PluginInfo.PLUGIN_GUID} ";

        private void Awake()
        {

            // The Logger will allow you to print things to the LogOutput (found in the BepInEx directory)
            Log = Logger;

            // Sets the title, default values, and descriptions
            string modName = "ErraticEncounters";
            EnableMod = Config.Bind(new ConfigDefinition(modName, "EnableMod"), true, new ConfigDescription("Enables the mod. If false, the mod will not work then next time you load the game."));
            EnableDebugging = Config.Bind(new ConfigDefinition(modName, "EnableDebugging"), false, new ConfigDescription("Enables the debugging"));
            EnableDLCMode = Config.Bind(new ConfigDefinition(modName, "EnableDLCMode"), false, new ConfigDescription("Enables DLC mode, meaning only DLC enemies will spawn"));
            // EnableChampions = Config.Bind(new ConfigDefinition(modName, "EnableChampions"), true, new ConfigDescription("Makes all combats have a champion enemy or boss"));
            DisablePositioning = Config.Bind(new ConfigDefinition(modName, "DisablePositioning"), true, new ConfigDescription("Disables the positioning of enemies. "));
            EnableBossRandomization = Config.Bind(new ConfigDefinition(modName, "EnableBossRandomization"), false, new ConfigDescription("Also Randomizes bosses (to ones of the same difficulty)"));
            EnableBossesInHallway = Config.Bind(new ConfigDefinition(modName, "EnableBossesInHallway"), false, new ConfigDescription("If true, bosses can spawn in hallway fights"));
            EnableEventCombatRandomization = Config.Bind(new ConfigDefinition(modName, "EnableEventCombatRandomization"), true, new ConfigDescription("Enables the event combat randomization"));
            IncludeOCBosses = Config.Bind(new ConfigDefinition(modName, "IncludeOCBosses"), false, new ConfigDescription("If true, OC bosses be added to the pool of enemies"));



            // apply patches, this functionally runs all the code for Harmony, running your mod
            PluginName = PluginInfo.PLUGIN_NAME;
            PluginVersion = PluginInfo.PLUGIN_VERSION;
            PluginGUID = PluginInfo.PLUGIN_GUID;
            if (EnableMod.Value)
            {
                if (EssentialsCompatibility.Enabled)
                    EssentialsCompatibility.EssentialsRegister();
                else
                    LogInfo($"{PluginGUID} {PluginVersion} has loaded!");
                harmony.PatchAll();
            }
        }


        // These are some functions to make debugging a tiny bit easier.
        internal static void LogDebug(string msg)
        {
            if (EnableDebugging.Value)
            {
                Log.LogDebug(debugBase + msg);
            }

        }
        internal static void LogInfo(string msg)
        {
            Log.LogInfo(debugBase + msg);
        }
        internal static void LogError(string msg)
        {
            Log.LogError(debugBase + msg);
        }


    }
}