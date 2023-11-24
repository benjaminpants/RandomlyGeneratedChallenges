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
    [BepInPlugin("mtm101.rulerp.baldiplus.randomchallenges", "Randomly Generated Challenges", "1.0.0.1")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class RandomChallengesPlugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        public static SceneObject stealthyScene;
        public static SceneObject speedyScene;
        public static SceneObject grappleScene;

        public static StealthyOfficeBuilder stealthyBuilder;

        void Awake()
        {
            Harmony harmony = new Harmony("mtm101.rulerp.baldiplus.randomchallenges");
            harmony.PatchAll();

            Log = base.Logger;
        }
    }

    [HarmonyPatch(typeof(NameManager))]
    [HarmonyPatch("Awake")]
    public class AddGeneratorsBack
    {
        static void FixOldMap(List<Texture2D> textures, LevelObject obj, LevelObject reference)
        {
            obj.hallWallTexs = new WeightedTexture2D[] {
                new WeightedTexture2D()
                {
                    selection = textures.Find(x => x.name == "Wall"),
                    weight = 100
                }
            };
            obj.classWallTexs = obj.hallWallTexs;
            obj.facultyWallTexs = new WeightedTexture2D[] {
                new WeightedTexture2D()
                {
                    selection = textures.Find(x => x.name == "WallWithMolding"),
                    weight = 100
                }
            };
            obj.hallFloorTexs = new WeightedTexture2D[] {
                new WeightedTexture2D()
                {
                    selection = textures.Find(x => x.name == "TileFloor"),
                    weight = 100
                }
            };

            obj.classFloorTexs = reference.classFloorTexs;
            obj.classCeilingTexs = reference.classCeilingTexs;
            obj.facultyCeilingTexs = reference.facultyCeilingTexs;
            obj.facultyFloorTexs = reference.facultyFloorTexs;
            obj.hallLights = reference.hallLights;
            obj.hallCeilingTexs = reference.hallCeilingTexs;
            obj.officeLights = reference.officeLights;
            obj.officeLights = reference.officeLights;
            obj.facultyBuilders = reference.facultyBuilders.Where(x => x.selection.name == "FacultyBuilder_Standard 1").ToArray();
            obj.suppliesDoorMat = reference.suppliesDoorMat;
            obj.classLights = reference.classLights;
            obj.facultyLights = reference.facultyLights;
        }

        static void Prefix()
        {
            // Set up the office builder
            OfficeBuilderStandard stanBuildNew = GameObject.Instantiate<OfficeBuilderStandard>(Resources.FindObjectsOfTypeAll<OfficeBuilderStandard>().First());
            stanBuildNew.gameObject.SetActive(false);
            stanBuildNew.name = "StealthyOfficeBuilder";
            GameObject obj = stanBuildNew.gameObject;
            StealthyOfficeBuilder stf = obj.AddComponent<StealthyOfficeBuilder>();
            // you should only use ReflectionSetVariable and ReflectionGetVariable for tiny use cases like this.
            // anything major and things will start lagging due to the FieldInfos created by these not being cached anywhere.
            stf.ReflectionSetVariable("decorations",stanBuildNew.ReflectionGetVariable("decorations"));
            stf.ReflectionSetVariable("decorSpawnChance", stanBuildNew.ReflectionGetVariable("decorSpawnChance"));
            stf.ReflectionSetVariable("tapePlayerPref", stanBuildNew.ReflectionGetVariable("tapePlayerPref"));
            stf.ReflectionSetVariable("deskPlacer", stanBuildNew.ReflectionGetVariable("deskPlacer"));
            stf.ReflectionSetVariable("deskSpawner", stanBuildNew.ReflectionGetVariable("deskSpawner"));
            stf.ReflectionSetVariable("windowObject", stanBuildNew.ReflectionGetVariable("windowObject"));
            GameObject.Destroy(stanBuildNew);
            GameObject.DontDestroyOnLoad(stanBuildNew.gameObject);
            RandomChallengesPlugin.stealthyBuilder = stf;

            SceneObject grappleScene = Resources.FindObjectsOfTypeAll<SceneObject>().Where(x => x.name == "GrappleChallenge").First();
            SceneObject stealthyScene = Resources.FindObjectsOfTypeAll<SceneObject>().Where(x => x.name == "StealthyChallenge").First();
            SceneObject speedScene = Resources.FindObjectsOfTypeAll<SceneObject>().Where(x => x.name == "SpeedyChallenge").First();
            LevelObject[] levelObjects = Resources.FindObjectsOfTypeAll<LevelObject>();
            LevelObject endlessObject = levelObjects.Where(x => x.name == "Endless1").First();
            List<Material> materials = Resources.FindObjectsOfTypeAll<Material>().ToList();
            List<Texture2D> textures = Resources.FindObjectsOfTypeAll<Texture2D>().ToList();
            grappleScene.extraAsset = null;
            grappleScene.levelAsset = null;
            grappleScene.levelObject = levelObjects.Where(x => x.name == "GrappleChallenge").First();
            LevelObject challengeobj = grappleScene.levelObject;
            FixOldMap(textures, challengeobj, endlessObject);
            stealthyScene.extraAsset = null;
            stealthyScene.levelAsset = null;
            stealthyScene.levelObject = levelObjects.Where(x => x.name == "StealthyChallenge").First();
            LevelObject stealthyobj = stealthyScene.levelObject;
            FixOldMap(textures, stealthyobj, endlessObject);
            stealthyobj.officeBuilders = new WeightedRoomBuilder[]
            {
                new WeightedRoomBuilder()
                {
                    selection = RandomChallengesPlugin.stealthyBuilder,
                    weight = 100
                }
            };
            speedScene.extraAsset = null;
            speedScene.levelAsset = null;
            speedScene.levelObject = levelObjects.Where(x => x.name == "Speedy Challenge").First();
            LevelObject speedobj = speedScene.levelObject;
            FixOldMap(textures, speedobj, endlessObject);
            // is marking these as neverUnload and storing a reference to them redundant? yes
            // but i'm trying to get into the habit of using MarkAsNeverUnload so...
            stealthyScene.MarkAsNeverUnload();
            speedScene.MarkAsNeverUnload();
            grappleScene.MarkAsNeverUnload();
            RandomChallengesPlugin.stealthyScene = stealthyScene;
            RandomChallengesPlugin.grappleScene = grappleScene;
            RandomChallengesPlugin.speedyScene = speedScene;
        }
    }
}
