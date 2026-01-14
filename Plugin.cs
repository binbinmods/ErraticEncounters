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


        public static ConfigEntry<bool> CompleteRandomization { get; set; }
        public static ConfigEntry<bool> EnableDebugging { get; set; }
        public static ConfigEntry<bool> EnableDLCMode { get; set; } // DONE
        public static ConfigEntry<bool> AllowDuplicates { get; set; } // DONE
        public static ConfigEntry<bool> DisablePositioning { get; set; } // DONE
        public static ConfigEntry<bool> AddChampionsToPool { get; set; } // DONE
        public static ConfigEntry<bool> AddBossesToPool { get; set; } // DONE
        public static ConfigEntry<bool> RandomizeBosses { get; set; } // DONE
        public static ConfigEntry<bool> FairlyRandomizeBosses { get; set; } // TODO
        // public static ConfigEntry<bool> IncludeOCBosses { get; set; }
        public static ConfigEntry<bool> RandomizeEventCombat { get; set; } // DONE
        public static ConfigEntry<bool> EnableHardEnemiesOnly { get; set; } // TODO


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
            CompleteRandomization = Config.Bind(new ConfigDefinition(modName, "EnableMod"), true, new ConfigDescription("Enables the mod. This completely randomizes the enemies in all non-event combats."));
            EnableDebugging = Config.Bind(new ConfigDefinition(modName, "EnableDebugging"), false, new ConfigDescription("Enables the debugging"));
            EnableDLCMode = Config.Bind(new ConfigDefinition(modName, "EnableDLCMode"), false, new ConfigDescription("Enables DLC mode, meaning only DLC enemies will spawn"));
            AllowDuplicates = Config.Bind(new ConfigDefinition(modName, "AllowDuplicates"), true, new ConfigDescription("Enables duplicates of the same enemy to spawn in the same combat"));
            AddChampionsToPool = Config.Bind(new ConfigDefinition(modName, "EnableChampions"), true, new ConfigDescription("Makes all combats have a champion enemy or boss"));
            AddBossesToPool = Config.Bind(new ConfigDefinition(modName, "EnableBossesInHallway"), false, new ConfigDescription("If true, bosses can spawn in hallway fights"));
            DisablePositioning = Config.Bind(new ConfigDefinition(modName, "DisablePositioning"), true, new ConfigDescription("Disables the positioning of enemies. "));
            RandomizeEventCombat = Config.Bind(new ConfigDefinition(modName, "RandomizeEvents"), true, new ConfigDescription("Enables the randomization of non-boss event combats"));
            RandomizeBosses = Config.Bind(new ConfigDefinition(modName, "RandomizeBosses"), false, new ConfigDescription("Also Randomizes bosses (to ones of any difficulty)"));
            FairlyRandomizeBosses = Config.Bind(new ConfigDefinition(modName, "FairlyRandomizeBosses"), false, new ConfigDescription("Randomizes bosses to ones of any difficulty, but not the same difficulty"));
            // IncludeOCBosses = Config.Bind(new ConfigDefinition(modName, "IncludeOCBosses"), false, new ConfigDescription("If true, OC bosses be added to the pool of enemies"));
            // EnableHardEnemiesOnly = Config.Bind(new ConfigDefinition(modName, "EnableHardEnemiesOnly"), false, new ConfigDescription("If true, only hard enemies will spawn"));


            // apply patches, this functionally runs all the code for Harmony, running your mod
            PluginName = PluginInfo.PLUGIN_NAME;
            PluginVersion = PluginInfo.PLUGIN_VERSION;
            PluginGUID = PluginInfo.PLUGIN_GUID;
            if (CompleteRandomization.Value)
            {
                if (EssentialsCompatibility.Enabled)
                    EssentialsCompatibility.EssentialsRegister();
                else
                    LogInfo($"{PluginGUID} {PluginVersion} has loaded!");
                harmony.PatchAll();
            }
        }


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