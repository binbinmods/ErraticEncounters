using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
// using static Obeliskial_Essentials.Essentials;
using System;
using static ErraticEncounters.Plugin;
using static ErraticEncounters.CustomFunctions;
using static ErraticEncounters.ErraticEncountersFunctions;
using System.Collections.Generic;
using static Functions;
using UnityEngine;
// using Photon.Pun;
using TMPro;
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
// using Unity.TextMeshPro;

// Make sure your namespace is the same everywhere
namespace ErraticEncounters
{

    [HarmonyPatch] // DO NOT REMOVE/CHANGE - This tells your plugin that this is part of the mod

    public class ErraticEncountersPatches
    {
        public static bool devMode = false; //DevMode.Value;
        public static bool bSelectingPerk = false;
        public static bool IsHost()
        {
            return GameManager.Instance.IsMultiplayer() && NetworkManager.Instance.IsMaster();
        }




        [HarmonyPrefix]
        [HarmonyPatch(typeof(Functions), "GetRandomCombat")]
        public static bool GetRandomCombatPrefix(ref NPCData[] __result, Enums.CombatTier combatTier, int seed, string nodeSelectedId, bool forceIsThereRare = false)
        {

            if (DisablePositioning.Value)
            {
                bool hasChampion = MadnessManager.Instance?.IsMadnessTraitActive("randomcombats") ?? false;
                bool hasDoubleChamps = AtOManager.Instance.Sandbox_doubleChampions;
                NPCData champion = Globals.Instance.GetNPCForRandom(hasChampion, 0, combatTier, []);
                NPCData npc1 = Globals.Instance.GetNPCForRandom(hasDoubleChamps, 0, combatTier, []);
                NPCData npc2 = Globals.Instance.GetNPCForRandom(false, 0, combatTier, []);
                NPCData npc3 = Globals.Instance.GetNPCForRandom(false, 0, combatTier, []);

                NPCData[] enemies = [champion, npc1, npc2, npc3];
                __result = enemies.OrderBy(x => UnityEngine.Random.value).ToArray();
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Globals), "GetNPCForRandom")]
        public static bool GetNPCForRandomPrefix(ref NPCData __result, bool _rare, int position, Enums.CombatTier _ct, NPCData[] _teamNPC)
        {
            if (GameManager.Instance.IsWeeklyChallenge())
            {
                return true;
            }

            // NPCs = GetPoolOfNPCs();
            // NPCsNamed = GetPoolOfNamedNPCs();
            Dictionary<string, NPCData> NPCPool = GetPoolOfNPCs(_rare);


            bool flag = false;
            int difficulty = GetDifficulty(_ct, _rare);


            NPCData nPCData = null;
            string text = "";
            int num3 = 0;
            while (!flag)
            {
                num3++;
                if (num3 > 10000)
                {
                    return true;
                }
                text = NPCPool.Keys.ToArray()[UnityEngine.Random.Range(0, NPCPool.Keys.Count)];
                nPCData = NPCPool[text];

                bool npcIsNotDuplicate = true;
                if (_teamNPC != null)
                {
                    int npcIndex = 0;
                    for (int i = 0; i < _teamNPC.Length; i++)
                    {
                        if (_teamNPC[i] != null && _teamNPC[i].SpriteSpeed != null && nPCData.SpriteSpeed != null && _teamNPC[i].SpriteSpeed.name == nPCData.SpriteSpeed.name)
                        {
                            npcIndex++;
                            if (npcIndex >= 1)
                            {
                                npcIsNotDuplicate = false;
                                break;
                            }
                        }
                    }
                }
                if (!npcIsNotDuplicate && !AllowDuplicates.Value)
                {
                    continue;
                }
                if (nPCData.Difficulty != difficulty && difficulty != -1)
                {
                    continue;
                }
                if (EnableDLCMode.Value && OriginalEnemies.Contains(nPCData.Id))
                {
                    continue;
                }
                if (!_rare && !DisablePositioning.Value)
                {
                    switch (position)
                    {
                        case 0:
                            break;
                        case 1:
                            {
                                Enums.CardTargetPosition preferredPosition = nPCData.PreferredPosition;
                                if (preferredPosition == Enums.CardTargetPosition.Anywhere || preferredPosition == Enums.CardTargetPosition.Front)
                                {
                                    flag = true;
                                }
                                continue;
                            }
                        case -1:
                            {
                                Enums.CardTargetPosition preferredPosition = nPCData.PreferredPosition;
                                if (preferredPosition == Enums.CardTargetPosition.Anywhere || preferredPosition == Enums.CardTargetPosition.Back)
                                {
                                    flag = true;
                                }
                                continue;
                            }
                        default:
                            continue;
                    }
                }
                flag = true;
            }
            if (!EnableDLCMode.Value && CompleteRandomization.Value && nPCData.UpgradedMob != null)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    nPCData = nPCData.UpgradedMob;
                }
            }
            else if ((_ct == Enums.CombatTier.T6 || _ct == Enums.CombatTier.T7 || _ct == Enums.CombatTier.T9 || _ct == Enums.CombatTier.T10) && nPCData.UpgradedMob != null)
            {
                nPCData = nPCData.UpgradedMob;
            }


            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EventManager), "FinalResolution")]
        public static void FinalResolutionPostfix(EventManager __instance, ref CombatData ___followUpCombatData)
        {
            if (!RandomizeEventCombat.Value)
            {
                return;
            }
            if (IsBossCombat(___followUpCombatData) && !RandomizeBosses.Value)
            {
                return;
            }
            if (___followUpCombatData == null)
            {
                LogDebug("FinalResolutionPostfix - Event does not have combat");
                return;
            }
            NPCData[] enemies = ___followUpCombatData?.NPCList;
            if (enemies == null || enemies.Length == 0)
            {
                LogDebug("FinalResolutionPostfix - Combat does not have enemies");
                return;
            }
            string nodeId = AtOManager.Instance?.currentMapNode ?? "Nonactive Node";
            NodeData node = Globals.Instance.GetNodeData(AtOManager.Instance.currentMapNode) ?? MapManager.Instance?.nodeActive.nodeData;
            if (node == null)
            {
                LogDebug("FinalResolutionPostfix - Null node");
                return;
            }
            int deterministicHashCode = (AtOManager.Instance.currentMapNode + AtOManager.Instance.GetGameId()).GetDeterministicHashCode();
            Enums.CombatTier ct = ___followUpCombatData?.CombatTier ?? (Enums.CombatTier)10;
            ___followUpCombatData.NPCList = Functions.GetRandomCombat(ct, deterministicHashCode, nodeId, forceIsThereRare: true);
        }


    }
}