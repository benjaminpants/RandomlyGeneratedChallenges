using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RandomlyGeneratedChallenges
{
    public class ChallengeUIManager : MonoBehaviour
    {
        private static FieldInfo gameLoaderseedInput = AccessTools.Field(typeof(GameLoader), "seedInput");
        public SeedInput seedInputTemplate;
        public SeedInput mySeedInput;
        public GameLoader gameLoader;
        public ElevatorScreen elevator;
        public StandardMenuButton speedyButton;
        public StandardMenuButton stealthyButton;
        public StandardMenuButton grappleButton;

        void AddListenerFor(StandardMenuButton smb, SceneObject obj)
        {
            smb.OnPress = new UnityEngine.Events.UnityEvent();
            smb.OnPress.AddListener(() =>
            {
                gameLoader.gameObject.SetActive(true);
                // temporarily change the seed input to our seed input
                SeedInput oldInput = (SeedInput)gameLoaderseedInput.GetValue(gameLoader);
                gameLoaderseedInput.SetValue(gameLoader, mySeedInput);
                gameLoader.CheckSeed();
                gameLoaderseedInput.SetValue(gameLoader, oldInput);
                gameLoader.Initialize(0);
                gameLoader.SetMode((int)Mode.Main);
                gameLoader.AssignElevatorScreen(elevator);
                elevator.gameObject.SetActive(true);
                gameLoader.LoadLevel(obj);
                elevator.Initialize();
            });
        }

        public void Start()
        {
            mySeedInput = GameObject.Instantiate<SeedInput>(seedInputTemplate, gameObject.transform);
            mySeedInput.transform.SetAsFirstSibling();
            transform.Find("BG").SetAsFirstSibling();
            mySeedInput.gameObject.SetActive(true);
            gameLoader = Resources.FindObjectsOfTypeAll<GameLoader>().First();
            elevator = SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.name == "ElevatorScreen").First().GetComponent<ElevatorScreen>();
            speedyButton = transform.Find("Speedy").GetComponent<StandardMenuButton>();
            stealthyButton = transform.Find("Stealthy").GetComponent<StandardMenuButton>();
            grappleButton = transform.Find("Grapple").GetComponent<StandardMenuButton>();
            AddListenerFor(grappleButton,RandomChallengesPlugin.grappleScene);
            AddListenerFor(stealthyButton, RandomChallengesPlugin.stealthyScene);
            AddListenerFor(speedyButton, RandomChallengesPlugin.speedyScene);
        }
    }
}
