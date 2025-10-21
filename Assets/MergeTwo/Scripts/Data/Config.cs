using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MergeTwo
{
    [CreateAssetMenu(fileName = "Config", menuName = "SO/Config")]
    public class Config : ScriptableObject
    {
        public List<IconTypeConfig> IconTypeConfigs;
        public State InitialState;
        public List<Order> Orders;
        public List<ConfigRoom> Rooms;
        public TextAsset InitialStateFile;
        public GameObject LightPrefab;
        public List<int> EnergyIconValues;
        public int IconEnergyDelay = 300;
        public bool IsSaveState = true;

        Dictionary<IconType, IconTypeConfig> _configByType;

        public void Init() 
        {
            _configByType = new Dictionary<IconType, IconTypeConfig>();
            foreach (var config in IconTypeConfigs)
            {
                _configByType.Add(config.IconType, config);
            }
        }

        public Sprite GetSprite(IconType iconType, int index)
        {
            IconTypeConfig iconTypeConfig = IconTypeConfigs.First(x => x.IconType == iconType);
            if (index >= iconTypeConfig.Sprites.Count) 
            {
                Debug.LogError($"There is no sprite for {iconType} {index}");
            }
            return iconTypeConfig.Sprites[index];
        }

        public IconTypeConfig GetTypeConfig(IconType iconType) => _configByType[iconType];        
    } 
}
