using System.Collections.Generic;
using MergeIt.Core.Messages;
using MergeIt.SimpleDI;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
   [SerializeField] private List<Tutorial> _tutorialsToSpawn; // Tutorials have to be in correct order from first to last!!!

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
      Debug.Log("OnTutorialFinished: " + message.TutorialFinished.TutorialName);
      message.TutorialFinished.gameObject.SetActive(false);
      PlayerPrefs.SetInt(message.TutorialFinished.TutorialName, 1);
   }
}
