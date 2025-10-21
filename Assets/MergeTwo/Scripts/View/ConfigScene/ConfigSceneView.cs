using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MergeTwo
{
    public class ConfigSceneView : MonoBehaviour,
        IEventConfigButtonClick,
        IEventSetIconOnField
    {
        [SerializeField] Transform _content;
        [SerializeField] Config _config;
        [SerializeField] ConfigLineView _linePrefab;
        [SerializeField] bool b_isUseInitialState = true;
        [SerializeField] List<Icon> _randomIcons;

        EventBus _eventBus;
        ConfigButtonViewField _selectedButton;
        State _currentState;


        private void Awake()
        {
            _eventBus = new EventBus();
            GameContext.Register(_eventBus);
            GameContext.Register(_config);
            _config.Init();
        }

        private void Start()
        {
            _eventBus.Subscribe<IEventConfigButtonClick>(this);
            _eventBus.Subscribe<IEventSetIconOnField>(this);

            for (int i = 0; i < _config.IconTypeConfigs.Count; i++)
            {
                IconTypeConfig iconTypeConfig = _config.IconTypeConfigs[i];
                ConfigLineView line = Instantiate(_linePrefab, _content);
                line.Init(iconTypeConfig.IconType, iconTypeConfig.Sprites);
            }

            _currentState = new State();
            if (b_isUseInitialState)
            {
                _currentState = _config.InitialState;
            }
            else
            {
                Logic.FillFieldIcons(_currentState);
            }
        }

        void IEventConfigButtonClick.Click(ConfigButtonViewField button, Vector2Int pos)
        {
            if (_selectedButton != null && _selectedButton != button)
                _selectedButton.Deselect();
            _selectedButton = button;
        }

        void IEventSetIconOnField.Click(IconType iconType, int index)
        {
            if (_selectedButton != null)
            {
                _selectedButton.SetIcon(iconType, index);
                Vector2Int pos = _selectedButton.Pos;
                _currentState.ListField[pos.x].Icons[pos.y].IconType = iconType;
                _currentState.ListField[pos.x].Icons[pos.y].Value = index;

                if (Logic.IsSpawner(iconType))
                {
                    var iconConfig = _config.GetTypeConfig(iconType);
                    _currentState.ListField[pos.x].Icons[pos.y].Capacity = iconConfig.SpawnerCapacities[index];
                }
            }
        }

        #region call from UI
        public void ApplyState()
        {
            FileManager.SaveFieldToResources(_currentState);
            _config.InitialState = _currentState;
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public void RandomizeState() 
        {
            Logic.RandomizeState(_currentState, _config.IconTypeConfigs, _randomIcons);
            _eventBus.Emmit<IEventRepaintField>(e => e.Repaint());
        }
        #endregion


        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventConfigButtonClick>(this);
            _eventBus.UnSubscribe<IEventSetIconOnField>(this);
        }
    }
}
