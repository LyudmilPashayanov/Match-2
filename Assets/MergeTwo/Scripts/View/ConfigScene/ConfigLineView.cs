using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class ConfigLineView : MonoBehaviour
    {
        [SerializeField] ConfigButtonView _button;
        [SerializeField] Transform _content;

        public void Init(IconType iconType, List<Sprite> sprites)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                ConfigButtonView button = Instantiate(_button, _content);
                button.Init(sprites[i], i, iconType);
            }
        }
    } 
}
