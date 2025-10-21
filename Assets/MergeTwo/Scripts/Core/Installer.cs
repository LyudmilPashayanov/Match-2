using System.Collections;
using UnityEngine;

namespace MergeTwo
{
    public class Installer : MonoBehaviour
    {
        [SerializeField] Config _config;

        public static Installer Instance;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _config.Init();
            GameContext.Register<Config>(_config);
            State state = FileManager.GetState();
            GameContext.Register<State>(state);
            GameContext.Register<EventBus>(new EventBus());
        }
    }
}