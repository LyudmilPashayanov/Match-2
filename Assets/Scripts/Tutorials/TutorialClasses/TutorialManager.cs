using System.Collections.Generic;
using MergeIt.Core.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
   private const string STOP_HAND_HINTS_PLAYER_PREFS_KEY = "stopTutorialHand";
   
   [SerializeField] private List<Tutorial> _tutorialsToSpawn; // Tutorials have to be in correct order from first to last!!!
   [SerializeField] private RectTransform _tutorialHand;

   private List<string> _tutorialKeys = new List<string>() 
      { 
         "firstMergeTutorial", 
         "uncoverFieldTutorial",
         "ClickGeneratorTutorial",
         "FirstOrderCompletedTutorial",/*
         "ShowLevelUpTutorial",
         "ShowRewardedItemsTutorial",*/
      };
   
   private IMessageBus _messageBus;
   
   private void Start()
   {
      _messageBus = DiContainer.Get<IMessageBus>();
      
      _messageBus.AddListener<TutorialFinishedMessage>(OnTutorialFinished);
      
      EnableHandHints();
      
      if (PlayerPrefs.GetInt(STOP_HAND_HINTS_PLAYER_PREFS_KEY) == 1)
      {
         DisableHandHints();
      }

      for (int i = 0; i < _tutorialKeys.Count; i++)
      {
         if (PlayerPrefs.HasKey(_tutorialKeys[i]) == false)
         {
            if (_tutorialsToSpawn[i].gameObject)
            {
               _tutorialsToSpawn[i].SetTutorialName(_tutorialKeys[i]);
               _tutorialsToSpawn[i].gameObject.SetActive(true);
            }
         }
      }
   }

   private void OnTutorialFinished(TutorialFinishedMessage message)
   {
      message.TutorialFinished.gameObject.SetActive(false);
      PlayerPrefs.SetInt(message.TutorialFinished.TutorialName, 1);

      if (message.TutorialFinished.TutorialName == _tutorialKeys[2] || message.TutorialFinished.TutorialName == _tutorialKeys[3])
      {
         EnableHandHints();
      }
   }

   private void EnableHandHints()
   {
      EnableTutorialHandMessage tutorialHandMessage = new EnableTutorialHandMessage { Enabled = true, TutorialHand = _tutorialHand};
      _messageBus.Fire(tutorialHandMessage);
   }
   
   private void DisableHandHints()
   {
      EnableTutorialHandMessage tutorialHandMessage = new EnableTutorialHandMessage { Enabled = false, TutorialHand = _tutorialHand};
      _messageBus.Fire(tutorialHandMessage);
   }
   
}
