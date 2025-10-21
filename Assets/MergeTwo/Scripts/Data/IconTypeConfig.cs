using System;
using System.Collections.Generic;
using UnityEngine;

namespace MergeTwo
{
    [Serializable]
    public class IconTypeConfig
    {
#if UNITY_EDITOR
        [HideInInspector] public string Name;
#endif
        public IconType IconType;
        public List<Sprite> Sprites;
        public int MaxValue => Sprites.Count;
        [Header("For spawner icons")]
        public List<SpawnableIcon> IconsToSpawn;
        public IconType CanBeFoundIn;
        public List<int> SpawnerCapacities;
        public long EnergyDelay = 300;
    } 
}
