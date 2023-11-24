using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Public.Patching;
using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace RandomlyGeneratedChallenges
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch("SpawnNPCs")]
    public class SpawnNPCsPatch
    {
        static void Postfix(EnvironmentController __instance)
        {
            if (Singleton<BaseGameManager>.Instance is StealthyChallengeManager)
            {
                RandomChallengesPlugin.Log.LogInfo("Starting missing arrows coroutine!");
                __instance.StartCoroutine(AddPrincipalArrows(__instance));
            }
        }
        private static IEnumerator AddPrincipalArrows(EnvironmentController ec)
        {
            while (ec.npcsLeftToSpawn.Count > 0)
            {
                foreach (NPC npc in ec.Npcs)
                {
                    if (npc.Character == Character.Principal)
                    {
                        if (!ec.map.arrowTargets.Contains(npc.transform))
                        {
#if DEBUG
                            RandomChallengesPlugin.Log.LogInfo("Added missing arrow!");
#endif
                            ec.map.AddArrow(npc.transform, Color.gray);
                        }
                    }
                }
                yield return null;
            }
            foreach (NPC npc in ec.Npcs)
            {
                if (npc.Character == Character.Principal)
                {
                    if (!ec.map.arrowTargets.Contains(npc.transform))
                    {
#if DEBUG
                        RandomChallengesPlugin.Log.LogWarning("Added missing arrow post-loop...");
#endif
                        ec.map.AddArrow(npc.transform, Color.gray);
                    }
                }
            }
            yield break;
        }
    }

    [HarmonyPatch(typeof(LevelGenerator))]
    [HarmonyPatch("StartGenerate")]
    public class BeginWaiting
    {
        static void Postfix(LevelGenerator __instance)
        {
            Principal tempChange = Resources.FindObjectsOfTypeAll<Principal>().Where(x => x.name == "Principal").First();
            Baldi tempChange2 = Resources.FindObjectsOfTypeAll<Baldi>().Where(x => x.name == "Baldi").First();
            // for principal
            RoomCategory[] tempArray = new RoomCategory[tempChange.spawnableRooms.Count];
            tempChange.spawnableRooms.CopyTo(tempArray);
            tempChange.spawnableRooms.Clear();
            tempChange.spawnableRooms.Add(RoomCategory.Hall);
            // for baldi
            RoomCategory[] tempArray2 = new RoomCategory[tempChange2.spawnableRooms.Count];
            tempChange2.spawnableRooms.CopyTo(tempArray2);
            tempChange2.spawnableRooms.Clear();
            tempChange2.spawnableRooms.Add(RoomCategory.Office);
            __instance.StartCoroutine(WaitForGeneratorRevert(__instance, tempChange, tempArray.ToList()));
            __instance.StartCoroutine(WaitForGeneratorRevert(__instance, tempChange2, tempArray2.ToList()));
        }

        public static IEnumerator WaitForGeneratorRevert(LevelBuilder lb, NPC toRevert, List<RoomCategory> revertTo)
        {
            while (lb.levelInProgress && !lb.levelCreated)
            {
                yield return null;
            }
            toRevert.spawnableRooms = revertTo;
            yield break;
        }
    }
}
