using System.Collections.Generic;
using UnityEngine;

namespace MergeTwo
{
    public class Grabber : MonoBehaviour
    {
        [SerializeField] InputController _input;
        [SerializeField] GrabberIconView _movebleIcon;
        [SerializeField] FlyIconAnimator _flyIconAnimator;

        public List<IconView> Icons;
        public List<IconView> CollidedIcons = new();
        
        public static Grabber Instance;

        IconView _startIcon;
        Config _config;
        State _state;
        EventBus _eventBus;
        IconView _collidedIcon;
        float _iconWidth;
        Vector3 _startPos;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _config = GameContext.GetInstance<Config>();
            _state = GameContext.GetInstance<State>();
            _eventBus = GameContext.GetInstance<EventBus>();
            _input.OnDoubleClick += OnDoubleClick;
            _iconWidth = _movebleIcon.Width;
            _startPos = _movebleIcon.transform.position;
        }

        private void Update()
        {
            if (IsCanGrab())
            {
                IdentifyCilidedIcon();

                if (!_movebleIcon.gameObject.activeInHierarchy)
                {
                    for (int i = 0; i < Icons.Count; i++)
                    {
                        IconView view = Icons[i];
                        if (view.Icon.IconType != IconType.None && view.IsInside(_input.CursorPos))
                        {
                            if (Logic.IsSpawner(view.Icon.IconType) && !Logic.IsCanSpawn(view.Icon, _state)) 
                            {
                                break;
                            }

                            _movebleIcon.transform.position = new Vector3(_input.CursorPos.x, _input.CursorPos.y, -1);
                            _movebleIcon.gameObject.SetActive(true);
                            _movebleIcon.Init(view.Icon);

                            _startIcon = view;
                            _startIcon.SetVisible(false);
                            break;
                        }
                    }
                }
                else
                {
                    _movebleIcon.transform.position = new Vector3(_input.CursorPos.x, _input.CursorPos.y, -1);
                }
            }
            else
            {
                if (_collidedIcon != null)
                {
                    if (_movebleIcon.Icon != null)
                    {
                        IconTypeConfig iconTypeConfig = _config.GetTypeConfig(_movebleIcon.Icon.IconType);
                        if (_collidedIcon.Icon != null && Logic.IsCanMerge(_movebleIcon.Icon, _collidedIcon.Icon, iconTypeConfig))
                        {
                            Pos moveblePos = _movebleIcon.Icon.Pos;
                            Icon merged = Logic.Merge(_movebleIcon.Icon, _collidedIcon.Icon, _state);

                            _collidedIcon.Init(merged);
                            _collidedIcon?.SetColor(Color.white);
                            _collidedIcon = null;
                            _movebleIcon.gameObject.SetActive(false);

                            _startIcon.Init(_state.ListField[moveblePos.x].Icons[moveblePos.y]);
                            _startIcon.SetVisible(true);
                            _startIcon = null;

                            _eventBus.Emmit<IEventIconMerged>(e => e.IconMerged(merged));
                        }
                        else
                        {
                            UpdateToDefault();
                            _collidedIcon?.SetColor(Color.white);
                            _collidedIcon = null;
                        }
                    }
                    else
                    {
                        UpdateToDefault();
                        _collidedIcon?.SetColor(Color.white);
                        _collidedIcon = null;
                    }
                }
                else
                {
                    UpdateToDefault();
                }
            }


        }

        private void IdentifyCilidedIcon()
        {
            _collidedIcon?.SetColor(Color.white);

            float minDist = float.MaxValue;
            List<IconView> iconsToRemove = new();

            for (int i = 0; i < CollidedIcons.Count; i++)
            {
                IconView icon = CollidedIcons[i];
                float dist = Vector3.Distance(icon.transform.position, _movebleIcon.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    _collidedIcon = icon;
                }

                if (dist > _iconWidth * 1.5f)
                {
                    icon.SetColor(Color.white);
                    iconsToRemove.Add(icon);
                }
            }

            if (_collidedIcon != null && _collidedIcon.Icon != null)
            {
                IconTypeConfig iconTypeConfig = _config.GetTypeConfig(_movebleIcon.Icon.IconType);
                Color color = Logic.IsCanMerge(_movebleIcon.Icon, _collidedIcon.Icon, iconTypeConfig) ? Color.green : Color.red;
                if (!(Logic.IsSpawner(_collidedIcon.Icon.IconType) && !Logic.IsCanSpawn(_collidedIcon.Icon, _state)))
                {
                    _collidedIcon.SetColor(color);
                }
            }

            iconsToRemove.ForEach(icon => CollidedIcons.Remove(icon));
        }

        public void AddCollidedIcon(IconView icon)
        {
            if (!CollidedIcons.Contains(icon))
            {
                CollidedIcons.Add(icon);
            }
        }

        void UpdateToDefault()
        {
            _movebleIcon.gameObject.SetActive(false);
            _movebleIcon.transform.position = _startPos;
            _startIcon?.SetVisible(true);
            _startIcon = null;
            CollidedIcons.Clear();
        }

        bool IsCanGrab()
        {
            if (_input.IsTap)
            {
                return true;
            }
            return false;
        }

        private void OnDoubleClick()
        {
            if (_flyIconAnimator.IsAnimated)
                return;

            for (int i = 0; i < Icons.Count; i++)
            {
                IconView view = Icons[i];
                if (Logic.IsSpawner(view.Icon.IconType) && view.IsInside(_input.CursorPos) && Logic.IsCanAddIcon(_state) && Logic.IsCanSpawn(view.Icon, _state))
                {
                    Logic.SpawnIcon(view.Icon, _state, _config.IconTypeConfigs);
                    AnimationItem item = _state.FlyIconAnimation;
                    _flyIconAnimator.StartAnimation(item.From, item.To, item.IconType, item.Value);
                    _eventBus.Emmit<IEventUpdateTopPanel>(e => e.UpdateTopPanel());
                    return;
                }

                if (Logic.IsEnergyIcon(view.Icon) && view.IsInside(_input.CursorPos))
                {
                    Logic.UseEnergyIcon(view.Icon, _config.EnergyIconValues, _state);
                    _eventBus.Emmit<IEventFieldUpdate>(e => e.UpdateView());
                    _eventBus.Emmit<IEventUpdateTopPanel>(e => e.UpdateTopPanel());
                    return;
                }
            }
        }
    }
}