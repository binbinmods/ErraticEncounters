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

namespace ErraticEncounters
{
    public class ErraticEncountersFunctions
    {
        public static List<NPCData> AllEnemies = [];
        public static List<NPCData> OCEnemies = [];
        // public static List<NPCData> DLCEnemies = [];

        public static List<NPCData> GetAllEnemies()
        {
            return AllEnemies;
        }
        public static List<NPCData> GetOCEnemies()
        {
            return OCEnemies;
        }
        public static List<NPCData> GetDLCEnemies()
        {
            HashSet<NPCData> enemies = new();
            enemies.UnionWith(GetAllEnemies());
            enemies.ExceptWith(GetOCEnemies());
            return enemies.ToList();
        }

        public static List<NPCData> GetPoolOfEnemies()
        {
            if (EnableDLCMode.Value)
            {
                return GetDLCEnemies();
            }
            else
            {
                return GetAllEnemies();
            }
        }

        public static NPCData[] GetRandomEnemies()
        {
            return new NPCData[0];
        }


    }
}

