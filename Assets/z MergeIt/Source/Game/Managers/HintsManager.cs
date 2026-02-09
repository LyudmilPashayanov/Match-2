// Copyright (c) 2024, Awessets

using System;
using System.Collections.Generic;
using DG.Tweening;
using MergeIt.Core.Configs.Elements;
using MergeIt.Core.FieldElements;
using MergeIt.Core.Helpers;
using MergeIt.Core.Messages;
using MergeIt.Core.Services;
using MergeIt.Game.Field;
using MergeIt.Game.Helpers;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using MergeIt.SimpleDI.ReservedInterfaces;
using UnityEngine;

namespace MergeIt.Game.Managers
{
    public class HintsManager : IHintsManager, IInitializable, IDisposable, IUpdatable
    {
        [Introduce] private IMessageBus _messageBus;

        [Introduce] private FieldLogicModel _fieldLogicModel;

        [Introduce] private IConfigsService _configsService;
        
        private RectTransform _tutorialHand;
        private bool _useHand;
        
        private float _time;
        private bool _animationInProcess;
        private bool _active;

        private IFieldElementPresenter _candidate1;
        private IFieldElementPresenter _candidate2;
        private IList<int> _randomHeight;
        private IList<int> _randomWidth;
        private Dictionary<ElementConfig, List<IFieldElement>> _sameElements = new();
        
        private Tween _handTween;
        private const float HAND_ANIMATION_DURATION = 1f;
        private bool Available => _active && !_animationInProcess;

        public void Initialize()
        {
            _messageBus.AddListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
            _messageBus.AddListener<ActivateHintsMessage>(OnActivateHintsMessageHandler);
            _messageBus.AddListener<ResetHintsMessage>(OnResetHintsMessageHandler);
            _messageBus.AddListener<EnableTutorialHandMessage>(HandTutorialEnableMessageHandler);
        }

        public void Dispose()
        {
            _messageBus.RemoveListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
            _messageBus.RemoveListener<ActivateHintsMessage>(OnActivateHintsMessageHandler);
            _messageBus.RemoveListener<ResetHintsMessage>(OnResetHintsMessageHandler);
            _messageBus.RemoveListener<EnableTutorialHandMessage>(HandTutorialEnableMessageHandler);
        }
        
        private void HandTutorialEnableMessageHandler(EnableTutorialHandMessage message)
        {
            if (message.Enabled)
            {
                _useHand = true;
                _tutorialHand = message.TutorialHand;
            }
            else
            {
                _useHand = false;
            }
        }
        
        public void Update()
        {
            if (Available && _candidate1 == null && _candidate2 == null)
            {
                _time += Time.deltaTime;

                if (_time >= _configsService.HintsConfig.Interval)
                {
                    _time = 0f;
                    FindCandidates();
                }
            }
            else if (_candidate1 != null && _candidate2 != null)
            {
                if (_candidate1.State == FieldElementState.Idle &&
                    _candidate2.State == FieldElementState.Idle)
                {
                    //StopHandLoop();
                    _candidate1 = null;
                    _candidate2 = null;

                    _animationInProcess = false;
                }
            }
        }

        private void OnLoadedGameMessageHandler(LoadedGameMessage _)
        {
            _randomHeight = ListExtensions.GenerateShuffledArray(_fieldLogicModel.FieldHeight);
            _randomWidth = ListExtensions.GenerateShuffledArray(_fieldLogicModel.FieldWidth);

            _active = true;
            _animationInProcess = false;
        }

        private void OnActivateHintsMessageHandler(ActivateHintsMessage message)
        {
            _active = message.Active;

            if (!_active)
            {
                ResetHint();
            }
        }

        private void OnResetHintsMessageHandler(ResetHintsMessage message)
        {
            ResetHint();
        }

        private void ResetHint()
        {
            _candidate1?.SetState(FieldElementState.Idle);
            _candidate2?.SetState(FieldElementState.Idle);

            _candidate1 = null;
            _candidate2 = null;

            _animationInProcess = false;
            //StopHandLoop();
        }

        private void FindCandidates()
        {
            _sameElements.Clear();

            _randomHeight.Shuffle();
            _randomWidth.Shuffle();

            List<IFieldElement> candidates = null;

            for (int i = 0; i < _randomHeight.Count; i++)
            {
                for (int j = 0; j < _randomWidth.Count; j++)
                {
                    int row = _randomHeight[i];
                    int column = _randomWidth[j];

                    GridPoint point = GridPoint.Create(row, column);
                    if (_fieldLogicModel.FieldElements.TryGetValue(point, out IFieldElement fieldElement))
                    {
                        FieldCellComponent cell =
                            _fieldLogicModel.CellComponents[fieldElement.InfoParameters.LogicPosition];
                        IFieldElementPresenter candidate = cell.FieldElementPresenter;
                        if (candidate.IsInvisibleLocked)
                        {
                            continue;
                        }
                        
                        ElementConfig config = fieldElement.ConfigParameters.ElementConfig;
                        if (fieldElement.GetNextInEvolution() != null)
                        {
                            if (!_sameElements.ContainsKey(config))
                            {
                                _sameElements[config] = new List<IFieldElement>();
                            }

                            var list = _sameElements[config];

                            if (list.Exists(x => x.InfoParameters.IsBlocked) &&
                                fieldElement.InfoParameters.IsBlocked)
                            {
                                continue;
                            }

                            list.Add(fieldElement);

                            if (list.Count > 1)
                            {
                                candidates = list;
                                break;
                            }
                        }
                    }
                }
            }

            if (candidates?.Count > 1 )
            {
               
                IFieldElement fieldElement = candidates[0];
                FieldCellComponent cell =
                    _fieldLogicModel.CellComponents[fieldElement.InfoParameters.LogicPosition];
                _candidate1 = cell.FieldElementPresenter;
                _candidate1.SetState(FieldElementState.Hint);

                IFieldElement fieldElement2 = candidates[1];
                FieldCellComponent cell2 =
                    _fieldLogicModel.CellComponents[fieldElement2.InfoParameters.LogicPosition];
                _candidate2 = cell2.FieldElementPresenter;
                _candidate2.SetState(FieldElementState.Hint);
                
                _animationInProcess = true;
                if (_useHand)
                {
                    if (_candidate1.IsLocked)
                    {
                        StartHandLoop(_candidate2.RectTransform, _candidate1.RectTransform);
                    }
                    else
                    {
                        StartHandLoop(_candidate1.RectTransform, _candidate2.RectTransform);
                    }
                }        
            }
        }
        
        private void StartHandLoop(RectTransform cand1, RectTransform cand2)
        {
            _tutorialHand.gameObject.SetActive(true);
            _tutorialHand.position = cand1.position;
            _handTween = _tutorialHand.DOMove(cand2.position, HAND_ANIMATION_DURATION).OnComplete(StopHandLoop);
            //_handTween.SetLoops(1, LoopType.Restart).OnComplete(StopHandLoop);
        }

        private void StopHandLoop()
        {
            _tutorialHand.gameObject.SetActive(false);
            _handTween?.Kill();
            _handTween = null;
        }
    }
}