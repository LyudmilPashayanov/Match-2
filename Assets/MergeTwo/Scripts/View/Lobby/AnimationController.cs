using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MergeTwo
{
    public class AnimationController : MonoBehaviour, IEventPiecePurchased
    {
        [SerializeField] Transform _start;
        [SerializeField] Transform _end;
        [SerializeField] Transform _target;
        [SerializeField][Range(0, 1.0f)] float _value;
        [SerializeField] float _starAnimationDuration;
        [SerializeField] TopPanel _topPanel;
        [SerializeField] GameObject _blocker;
        [SerializeField] PurchasePiecesPopup _purchasePiecesPopup;
        [SerializeField] PieceView[] _pieces;
        [SerializeField] AreaButton _areaButton;
        [SerializeField] RewardPopup _rewardPopup;
        [SerializeField] List<GameObject> _rooms;

        EventBus _eventBus;
        State _state;
        Config _config;

        private void Start()
        {
            _eventBus = GameContext.GetInstance<EventBus>();
            _eventBus.Subscribe<IEventPiecePurchased>(this);
            _state = GameContext.GetInstance<State>();
            _config = GameContext.GetInstance<Config>();

            _rooms[_state.CurrentRoom].SetActive(true);
           
        }

        void IEventPiecePurchased.PiecePurchased(int id, Transform star)
        {
            if (Logic.Room.IsShouldShowAnimation(id, _state))
            {
                StartCoroutine(Animation(id, star));
            }
        }

        private IEnumerator Animation(int id, Transform star)
        {
            _blocker.SetActive(true);
            _topPanel.Display();
            yield return StarMovement(id, star);
            yield return _purchasePiecesPopup.ProgressAnimation();
            yield return new WaitForSeconds(0.5f);
            _purchasePiecesPopup.HidePopup();
            PieceView pieceView = _pieces.First(x => x.ID == id);
            _areaButton.Display();
            yield return pieceView.ShowAnimations();
            Logic.Room.CheckFotRoomComplition(_state, _config.Rooms);
            if (_state.IsRoomCompleted)
            {
                _rewardPopup.Show();
                Logic.GiveReward(_state, _config, _state.CurrentRoom - 1);
                _blocker.SetActive(false);
                yield return new WaitWhile(() => _rewardPopup.Body.activeInHierarchy);
                _blocker.SetActive(true);
                Logic.Room.RoomAnimationPlayed(_state);
                _areaButton.Display();
            }
            _rooms.ForEach(x => x.SetActive(false));
            _rooms[_state.CurrentRoom].SetActive(true);
            _blocker.SetActive(false);
        }

        private IEnumerator StarMovement(int id, Transform star)
        {
            _target.gameObject.SetActive(true);
            float time = 0;
            while (time < _starAnimationDuration)
            {
                float value = time / _starAnimationDuration;
                Move(value, star.transform.position);
                time += Time.deltaTime;
                yield return null;
            }
            _target.gameObject.SetActive(false);
        }

        private void Move(float value, Vector3 endPos)
        {
            Vector2 pos = _start.position + (endPos - _start.position) * value;
            _target.position = pos;
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventPiecePurchased>(this);
        }
    }
}
