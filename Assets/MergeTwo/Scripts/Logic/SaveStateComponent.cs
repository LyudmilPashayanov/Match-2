using UnityEngine;

namespace MergeTwo
{
    public class SaveStateComponent : MonoBehaviour,
        IEventIconMerged,
        IEventOrderClaimed,
        IEventPiecePurchased
    {
        [SerializeField] Config _config;

        EventBus _eventBus;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            _eventBus = GameContext.GetInstance<EventBus>();
            _eventBus.Subscribe<IEventIconMerged>(this, 100);
            _eventBus.Subscribe<IEventOrderClaimed>(this, 100);
            _eventBus.Subscribe<IEventPiecePurchased>(this, 100);
        }

        void IEventIconMerged.IconMerged(Icon icon)
        {
            SaveState();
        }

        public void OrderClaimed(int id)
        {
            SaveState();
        }

        public void PiecePurchased(int id, Transform star)
        {
            SaveState();
        }

        private void SaveState()
        {
            if(_config.IsSaveState)
                FileManager.SaveState();
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventIconMerged>(this);
            _eventBus.UnSubscribe<IEventOrderClaimed>(this);
            _eventBus.UnSubscribe<IEventPiecePurchased>(this);
        }
    }
}
