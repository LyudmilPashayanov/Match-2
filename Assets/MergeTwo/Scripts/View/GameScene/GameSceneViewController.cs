using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MergeTwo
{
    public class GameSceneViewController : MonoBehaviour
    {
        [SerializeField] Transform[] _uiItems;
        [SerializeField] IconView _iconPrefab;
        [SerializeField] Transform _content;
        [SerializeField] FieldView _fieldView;
        [SerializeField] Grabber _grabber;
        [SerializeField] Button _homeButton;

        private void Start()
        {
            _homeButton.onClick.AddListener(LoadLobby);
        }

        [ContextMenu("CreateFieldIconsInEditor")]
        public void CreateFieldIconsInEditor()
        {
            List<IconView> icons = new List<IconView>();
            Camera mainCamera = Camera.main;
            for (int i = 0; i < _uiItems.Length; i++)
            {
                Transform item = _uiItems[i];
                IconView view = Instantiate(_iconPrefab, _content);
                Vector3 pos = mainCamera.ScreenToWorldPoint(item.position);
                view.transform.position = new Vector3(pos.x, pos.y, 0);
                icons.Add(view);
                Pos posInMatrix = new Pos( i / 7, i % 7);
                view.PosAtMatrix = posInMatrix;
            }
            _fieldView.IconViews = icons;
            _grabber.Icons = icons;
        }

        [ContextMenu("SaveState")]
        public void SaveState()
        {
            var state = GameContext.GetInstance<State>();
            FileManager.SaveState(state, Constant.SavePath);            
        }

        public void LoadLobby() 
        {
            SceneManager.LoadScene(Constant.Lobby);
        }
    } 
}
