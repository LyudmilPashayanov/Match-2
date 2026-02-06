using System.Collections.Generic;
using MergeIt.Core.Messages;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
   [SerializeField] private List<Tutorial> _tutorialsToSpawn;
   private List<string> _tutorialKeys = new List<string>() 
      { 
         "firstMergeTutorial", 
         /*"uncoverFieldTutorial",
         "ClickGeneratorTutorial",
         "FirstOrderCompletedTutorial",
         "ShowLevelUpTutorial",
         "ShowRewardedItemsTutorial",*/
      };
   
   private void Start()
   {
      for (int i = 0; i < _tutorialKeys.Count; i++)
      {
         if (PlayerPrefs.HasKey(_tutorialKeys[i]) == false)
         {
            if (_tutorialsToSpawn[i].gameObject)
            {
               _tutorialsToSpawn[i].gameObject.SetActive(true);
            }
         }
      }
   }
}
