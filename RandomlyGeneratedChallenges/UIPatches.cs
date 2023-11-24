using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RandomlyGeneratedChallenges
{
    [HarmonyPatch(typeof(MainModeButtonController))]
    [HarmonyPatch("OnEnable")]
    public class UIPatches
    {
        public static void Finalizer(MainModeButtonController __instance) //first time ever using a finalizer
        {
            GameObject pickchallenge = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "PickChallenge").First();
            if (pickchallenge.GetComponent<ChallengeUIManager>() == null)
            {
                ChallengeUIManager manager = pickchallenge.AddComponent<ChallengeUIManager>();
                Transform seedInput = __instance.gameObject.transform.Find("SeedInput");
                manager.seedInputTemplate = seedInput.gameObject.GetComponent<SeedInput>();
            }
        }
    }
}
