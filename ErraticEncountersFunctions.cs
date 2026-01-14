using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
// using Obeliskial_Content;
// using Obeliskial_Essentials;
using System.IO;
using static UnityEngine.Mathf;
using UnityEngine.TextCore.LowLevel;
using static ErraticEncounters.Plugin;
using System.Collections.ObjectModel;
using UnityEngine;
using MonoMod.Utils;

namespace ErraticEncounters
{
    public class ErraticEncountersFunctions
    {
        public static List<string> AllEnemies = [];
        public static List<string> OriginalEnemies =
        [
            "advisor",
            "angel",
            "angel_dark",
            "archer",
            "blazingcornie",
            "blacksheep",
            "brigand",
            "bruiser",
            "brute",
            "cornie",
            "cornierandom",
            "crucible",
            "cutthroat",
            "dracomancer",
            "draconoid",
            "dragonfly",
            "driller",
            "druid",
            "dryad",
            "dryad_h",
            "dualist",
            "evoker",
            "farmer",
            "fiend",
            "fireimp",
            "fisher",
            "forecaster",
            "golem",
            "gunner",
            "hatchlingeasy",
            "hatchling",
            "herder",
            "hypnocrab",
            "initiated",
            "knight",
            "knight_dark",
            "lancer",
            "lancer_dark",
            "lavaelemental",
            "livingrock",
            "localwerewolfa",
            "lumberjack",
            "magus",
            "magus_dark",
            "marauder",
            "monk",
            "monk_dark",
            "mutant",
            "mystic",
            "neophyte",
            "obsidianelemental",
            "oddball",
            "parasite",
            "pendulum",
            "psychic",
            "pyromancer",
            "rabbit",
            "raider",
            "raidereasy",
            "ravager",
            "royalguard",
            "ruffian",
            "sapling",
            "sawtooth",
            "scrapper",
            "sentinel",
            "shaman",
            "sheep",
            "sheeprandom",
            "sheepshearer",
            "siren",
            "skirmisher",
            "skyhuntereasy",
            "slime",
            "slimeasy",
            "spellbinder",
            "spider",
            "squirel",
            "squirelesy",
            "stalker",
            "stoneelemental",
            "stormbringer",
            "stormcaller",
            "swampelemental",
            "swamphorror",
            "swordman",
            "tainteddryad",
            "taintedsapling",
            "taintedtrunky",
            "thug",
            "trunky",
            "vanguard",
            "vanguard_dark",
            "volatileimp",
            "warboara",
            "warboarb",
            "wildboara",
            "wildboararandom",
            "wildboarb"
        ];
        public static List<string> DLCEnemies = [];
        public static Dictionary<string, NPCData> DLCPool = [];
        public static Dictionary<string, NPCData> BossPool = [];
        public static Dictionary<string, NPCData> BaseNPCPool = [];

        /// <summary>
        /// Gets all available enemy IDs from the base NPC pool. Caches the result for performance.
        /// </summary>
        /// <returns>List of all enemy ID strings</returns>
        public static List<string> GetAllEnemies()
        {
            LogDebug("GetAllEnemies - Called");
            // TODO Rework
            if (AllEnemies.Count == 0)
            {
                LogDebug("GetAllEnemies - Initializing enemy list from base pool");
                AllEnemies = [.. GetBaseNPCPool().Keys];
                LogDebug($"GetAllEnemies - Loaded {AllEnemies.Count} enemies");
            }
            else
            {
                LogDebug($"GetAllEnemies - Returning cached list with {AllEnemies.Count} enemies");
            }
            return AllEnemies;
        }

        /// <summary>
        /// Gets the base NPC pool from Globals.Instance using reflection. Caches the result for performance.
        /// </summary>
        /// <returns>Dictionary of NPC ID strings to NPCData objects</returns>
        public static Dictionary<string, NPCData> GetBaseNPCPool()
        {
            LogDebug("GetBaseNPCPool - Called");
            if (BaseNPCPool.Count == 0)
            {
                LogDebug("GetBaseNPCPool - Initializing base NPC pool from Globals");
                BaseNPCPool = Traverse.Create(Globals.Instance).Field("_NPCs").GetValue<Dictionary<string, NPCData>>();
                LogDebug($"GetBaseNPCPool - Loaded {BaseNPCPool.Count} NPCs");
            }

            return BaseNPCPool;
        }


        // public static List<NPCData> GetOCEnemiesData()
        // {
        //     if (OCEnemiesData.Count == 0)
        //     {
        //         OCEnemiesData = OCEnemies.Select(npc => Globals.Instance.GetNPC(npc)).ToList();
        //     }
        //     return OCEnemiesData;
        // }

        /// <summary>
        /// Gets the list of original content (OC) enemy IDs. These are base game enemies that can be filtered out in DLC mode.
        /// </summary>
        /// <returns>List of OC enemy ID strings</returns>
        public static List<string> GetOCEnemies()
        {
            LogDebug($"GetOCEnemies - Returning {OriginalEnemies.Count} OC enemies");
            return OriginalEnemies;
        }
        /// <summary>
        /// Gets the list of DLC enemy IDs by excluding OC enemies from all enemies. Caches the result for performance.
        /// </summary>
        /// <returns>List of DLC enemy ID strings</returns>
        public static List<string> GetDLCEnemies()
        {
            LogDebug("GetDLCEnemies - Called");
            if (DLCEnemies.Count == 0)
            {
                LogDebug("GetDLCEnemies - Calculating DLC enemies by excluding OC enemies");
                HashSet<string> enemies = [.. GetAllEnemies()];
                int allEnemiesCount = enemies.Count;
                enemies.ExceptWith(GetOCEnemies());
                DLCEnemies = enemies.ToList();
                LogDebug($"GetDLCEnemies - Calculated {DLCEnemies.Count} DLC enemies from {allEnemiesCount} total enemies");
            }

            return DLCEnemies;
        }
        /// <summary>
        /// Gets a dictionary of DLC enemy IDs to NPCData objects. Caches the result for performance.
        /// </summary>
        /// <returns>Dictionary of DLC enemy ID strings to NPCData objects</returns>
        public static Dictionary<string, NPCData> GetDLCPool()
        {
            LogDebug("GetDLCPool - Called");
            if (DLCPool.Count == 0)
            {
                LogDebug("GetDLCPool - Building DLC pool dictionary");
                List<string> dlcEnemyIds = GetDLCEnemies();
                DLCPool = dlcEnemyIds.ToDictionary(x => x, x => Globals.Instance.GetNPC(x));
                LogDebug($"GetDLCPool - Built pool with {DLCPool.Count} DLC NPCs");
            }

            return DLCPool;
        }

        /// <summary>
        /// Gets the pool of NPCs to choose from based on rarity and mod settings. 
        /// For rare NPCs, returns the named pool. For normal NPCs, applies DLC mode filtering and optional champion/boss additions.
        /// </summary>
        /// <param name="_rare">Whether to get rare (named/champion) NPCs or normal NPCs</param>
        /// <returns>Dictionary of NPC ID strings to NPCData objects</returns>
        public static Dictionary<string, NPCData> GetPoolOfNPCs(bool _rare)
        {
            LogDebug($"GetPoolOfNPCs - Called with _rare={_rare}");
            Dictionary<string, NPCData> NPCs = Traverse.Create(Globals.Instance).Field<Dictionary<string, NPCData>>("_NPCs").Value;

            if (_rare)
            {
                LogDebug("GetPoolOfNPCs - Returning named pool for rare NPCs");
                return GetNamedPool();
            }

            if (EnableDLCMode.Value)
            {
                LogDebug("GetPoolOfNPCs - DLC mode enabled, using DLC pool");
                NPCs = GetDLCPool();
            }

            if (AddChampionsToPool.Value)
            {
                LogDebug("GetPoolOfNPCs - Adding champions to pool");
                NPCs = NPCs.Union(GetNamedPool()).ToDictionary(x => x.Key, x => x.Value);
            }
            else if (AddBossesToPool.Value)
            {
                LogDebug("GetPoolOfNPCs - Adding bosses to pool");
                NPCs = NPCs.Union(GetBossPool()).ToDictionary(x => x.Key, x => x.Value);
            }

            LogDebug($"GetPoolOfNPCs - Returning pool with {NPCs.Count} NPCs");
            return NPCs;

        }

        /// <summary>
        /// Gets the pool of named/champion NPCs from Globals.Instance. Optionally includes bosses if AddBossesToPool is enabled.
        /// </summary>
        /// <returns>Dictionary of named NPC ID strings to NPCData objects</returns>
        public static Dictionary<string, NPCData> GetNamedPool()
        {
            LogDebug("GetNamedPool - Called");
            Dictionary<string, NPCData> NPCsNamed = Traverse.Create(Globals.Instance).Field<Dictionary<string, NPCData>>("_NPCsNamed").Value;
            if (AddBossesToPool.Value)
            {
                LogDebug($"GetNamedPool - Adding bosses to named pool of {NPCsNamed.Count} NPCs");
                NPCsNamed = NPCsNamed.Union(GetBossPool()).ToDictionary(x => x.Key, x => x.Value);
                LogDebug($"GetNamedPool - After Merge, pool has {NPCsNamed.Count} named NPCs");
            }
            return NPCsNamed;
        }

        /// <summary>
        /// Calculates the difficulty level for an NPC based on combat tier and rarity.
        /// Returns -1 if complete randomization is enabled (ignoring difficulty).
        /// </summary>
        /// <param name="_ct">The combat tier of the encounter</param>
        /// <param name="_rare">Whether this is a rare/champion NPC</param>
        /// <returns>Difficulty level (1-16), or -1 if difficulty should be ignored</returns>
        public static int GetDifficulty(Enums.CombatTier _ct, bool _rare)
        {
            int num = 0;
            int num2 = UnityEngine.Random.Range(0, 100);
            if (CompleteRandomization.Value && !EnableDLCMode.Value)
            {
                LogDebug("GetDifficulty - Complete randomization enabled, returning -1 (ignore difficulty)");
                return -1;
            }
            switch (_ct)
            {
                case Enums.CombatTier.T1:
                    num = ((!_rare) ? 1 : 2);
                    break;
                case Enums.CombatTier.T2:
                    num = ((!_rare) ? 3 : 4);
                    break;
                case Enums.CombatTier.T3:
                    num = ((!_rare) ? ((num2 >= 80) ? 7 : 5) : 6);
                    break;
                case Enums.CombatTier.T4:
                    num = ((!_rare) ? ((num2 >= 15) ? ((num2 >= 85) ? 9 : 7) : 5) : 8);
                    break;
                case Enums.CombatTier.T5:
                    num = ((!_rare) ? ((num2 >= 20) ? 9 : 7) : 10);
                    break;
                case Enums.CombatTier.T6:
                    num = ((!_rare) ? ((num2 >= 60) ? 7 : 5) : ((num2 >= 50) ? 8 : 6));
                    break;
                case Enums.CombatTier.T7:
                    num = ((!_rare) ? ((num2 >= 30) ? 9 : 7) : ((num2 >= 50) ? 10 : 8));
                    break;
                case Enums.CombatTier.T8:
                    if (_rare)
                    {

                        num = 15;
                    }
                    else
                    {
                        num = ((num2 >= 70) ? 5 : 3);
                    }
                    break;
                case Enums.CombatTier.T9:
                    if (_rare)
                    {
                        num = 16;
                    }
                    else
                    {
                        num = ((num2 >= 30) ? 9 : 7);
                    }
                    break;
                case Enums.CombatTier.T10:
                    num = ((!_rare) ? ((num2 >= 20) ? 11 : 9) : ((num2 >= 50) ? 12 : 10));
                    break;
                case Enums.CombatTier.T11:
                    num = ((!_rare) ? ((num2 >= 70) ? 13 : 11) : 12);
                    break;
                case Enums.CombatTier.T12:
                    num = ((!_rare) ? ((num2 >= 30) ? 13 : 11) : 12);
                    break;
            }
            return num;
        }

        /// <summary>
        /// Gets 4 random enemies from the enemy pool, shuffled randomly.
        /// </summary>
        /// <param name="_ct">The combat tier (currently unused but kept for API compatibility)</param>
        /// <param name="_rare">Whether to get rare enemies (currently unused but kept for API compatibility)</param>
        /// <returns>Array of 4 randomly selected NPCData objects</returns>
        public static NPCData[] GetRandomEnemies(Enums.CombatTier _ct, bool _rare)
        {
            LogDebug($"GetRandomEnemies - Called with _ct={_ct}, _rare={_rare}");
            // Select 4 random enemies from the pool of enemies
            List<string> enemies = GetAllEnemies();
            LogDebug($"GetRandomEnemies - Shuffling {enemies.Count} enemies");
            enemies.Shuffle();
            NPCData[] result = enemies.Take(4).Select(x => Globals.Instance.GetNPC(x)).ToArray();
            LogDebug($"GetRandomEnemies - Selected {result.Length} random enemies");
            return result;
        }

        /// <summary>
        /// Checks if a combat contains any boss enemies.
        /// </summary>
        /// <param name="_combatData">The combat data to check</param>
        /// <returns>True if any NPC in the combat is a boss, false otherwise</returns>
        public static bool IsBossCombat(CombatData _combatData)
        {
            if (_combatData == null)
            {
                LogDebug("IsBossCombat - Combat data is null, returning false");
                return false;
            }
            bool isBoss = _combatData.NPCList.Any(x => x.IsBoss);
            return isBoss;
        }

        /// <summary>
        /// Gets a dictionary of all boss NPCs from Globals.Instance. Caches the result for performance.
        /// </summary>
        /// <returns>Dictionary of boss NPC ID strings to NPCData objects</returns>
        public static Dictionary<string, NPCData> GetBossPool()
        {
            if (BossPool.Count == 0)
            {
                LogDebug("GetBossPool - Building boss pool from Globals");
                BossPool = Globals.Instance.NPCs.Where(x => x.Value.IsBoss).ToDictionary(x => x.Key, x => x.Value);
                LogDebug($"GetBossPool - Found {BossPool.Count} bosses");
            }
            return BossPool;
        }



    }
}

