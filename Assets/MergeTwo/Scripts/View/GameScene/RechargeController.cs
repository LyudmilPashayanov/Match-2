using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MergeTwo
{
    public class RechargeController : MonoBehaviour
    {
        [SerializeField] RechargeView _rechargePrefab;
        [SerializeField] IconView[] _iconViews;
        [SerializeField] Transform _canvas;

        State _state;
        Config _config;
        Dictionary<Pos, RechargeView> _viewByPos = new();

        private void Start()
        {
            _state = GameContext.GetInstance<State>();
            _config = GameContext.GetInstance<Config>();
        }

        private void Update()
        {
            for (int i = 0; i < _state.ListField.Count; i++)
            {
                for (int j = 0; j < _state.ListField[i].Icons.Count; j++)
                {
                    Icon icon = _state.ListField[i].Icons[j];
                    if (Logic.IsSpawner(icon.IconType))
                    {
                        if (icon.Capacity == 0 && icon.RechargeTime > 0)
                        {
                            Vector3 iconWorldPos = _iconViews.First(i => i.PosAtMatrix.x == icon.Pos.x && i.PosAtMatrix.y == icon.Pos.y).transform.position;
                            if (!_viewByPos.ContainsKey(icon.Pos))
                            {
                                RechargeView rechargeView = Instantiate<RechargeView>(_rechargePrefab, _canvas);
                                rechargeView.DisplayTimer(icon.RechargeTime, icon.IconType);
                                rechargeView.transform.position = Camera.main.WorldToScreenPoint(iconWorldPos);
                                _viewByPos[icon.Pos] = rechargeView;
                            }
                            else
                            {
                                _viewByPos[icon.Pos].DisplayTimer(icon.RechargeTime, icon.IconType);
                            }
                        }
                    }
                }
            }

            List<Pos> recharges = new();
            Logic.TryRechargeIcons(_state, _config.IconTypeConfigs, recharges);

            foreach (Pos pos in recharges) 
            {
                if (_viewByPos.ContainsKey(pos))
                {
                    Destroy(_viewByPos[pos].gameObject);
                    _viewByPos.Remove(pos);
                }
            }
        }
    }
}
