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
        public static bool GetRandomCombatPrefix(ref NPC[] __result, Enums.CombatTier combatTier, int seed, string nodeSelectedId, bool forceIsThereRare = false)
        {

            if (DisablePositioning.Value)
            {
                __result = new NPC[0];
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Globals), "GetNPCForRandom")]
        public static bool GetNPCForRandomPrefix(ref NPCData __result, bool _rare, int position, Enums.CombatTier _ct, NPCData[] _teamNPC)
        {



            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EventManager), "FinalResolution")]
        public static void FinalResolutionPostfix(EventManager __instance, ref CombatData ___followUpCombatData)
        {
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

            ___followUpCombatData.NPCList = Functions.GetRandomCombat(ct, deterministicHashCode, nodeId, forceIsThereRare: true);
        }


    }
}