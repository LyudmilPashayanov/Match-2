// Copyright (c) 2024, Awessets

using MergeIt.Core.Configs.Elements;
using MergeIt.Core.Messages;
using MergeIt.Core.MVP;
using MergeIt.Core.Services;
using MergeIt.Core.WindowSystem;
using MergeIt.Game.Effects;
using MergeIt.Game.Effects.Controllers;
using MergeIt.Game.Effects.Parameters;
using MergeIt.Game.Enums;
using MergeIt.Game.Factories.ElementInfo;
using MergeIt.Game.Messages;
using MergeIt.Game.Services;
using MergeIt.Game.Windows.ElementInfo;
using MergeIt.SimpleDI;
using UnityEngine;

namespace MergeIt.Game.HUD
{
    public class HudComponent : MonoBehaviour
    {
        [SerializeField]
        private ProgressComponent _progressComponent;

        [SerializeField]
        private SoftCurrencyComponent _softCurrencyComponent;

        [SerializeField]
        private HardCurrencyComponent _hardCurrencyComponent;

        [SerializeField]
        private EnergyComponent _energyComponent;

        private UserServiceModel _userServiceModel;
        private IUserProgressService _userProgressService;
        private IMessageBus _messageBus;
        private IElementsStockService _elementsStockService;
        private IElementInfoFactory _elementInfoFactory;
        private IEffectsFactory _effectsFactory;

        private void Start()
        {
            _messageBus = DiContainer.Get<IMessageBus>();
            _messageBus.AddListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
            _messageBus.AddListener<LevelUpdatedMessage>(OnLevelUpdatedMessageHandler);
            
            _messageBus.AddListener<MenuStartedMessage>(OnMenuStartedMessage);


        }

        private void OnDestroy()
        {
            _messageBus.RemoveListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
            _messageBus.RemoveListener<LevelUpdatedMessage>(OnLevelUpdatedMessageHandler);

            _messageBus.RemoveListener<MenuStartedMessage>(OnMenuStartedMessage);
        }

        private void OnLoadedGameMessageHandler(LoadedGameMessage message)
        {
            _userServiceModel = DiContainer.Get<UserServiceModel>();
            var windowSystem = DiContainer.Get<IWindowSystem>();
            
            _userProgressService = DiContainer.Get<IUserProgressService>();
            _elementsStockService = DiContainer.Get<IElementsStockService>();
            _effectsFactory = DiContainer.Get<IEffectsFactory>();
            _elementInfoFactory = DiContainer.Get<IElementInfoFactory>();

            int maxExp = _userProgressService.GetCurrentLevelMaxExp();
            _progressComponent.Initialize(windowSystem);   
            _progressComponent.SetMaxProgress(maxExp);
            _progressComponent.ApplyModel(_userServiceModel);
            _progressComponent.UpdateProgress();

            _energyComponent.ApplyModel(_userServiceModel);
            _softCurrencyComponent.ApplyModel(_userServiceModel);
            _hardCurrencyComponent.ApplyModel(_userServiceModel);
        }
        
        private void OnMenuStartedMessage(MenuStartedMessage message)
        {
            _userServiceModel = DiContainer.Get<UserServiceModel>();
            _userProgressService = DiContainer.Get<IUserProgressService>();
            _elementInfoFactory = DiContainer.Get<IElementInfoFactory>();

            var windowSystem = DiContainer.Get<IWindowSystem>();

            int maxExp = _userProgressService.GetCurrentLevelMaxExp();
            _progressComponent.Initialize(windowSystem);   
            _progressComponent.SetMaxProgress(maxExp);
            _progressComponent.ApplyModel(_userServiceModel);
            _progressComponent.UpdateProgress();

            _energyComponent.ApplyModel(_userServiceModel);
            _softCurrencyComponent.ApplyModel(_userServiceModel);
            _hardCurrencyComponent.ApplyModel(_userServiceModel);
        }

        private void OnLevelUpdatedMessageHandler(LevelUpdatedMessage message)
        {
            _progressComponent.SetMaxProgress(message.NextLevelExp);
            _progressComponent.UpdateProgress();
            
            //////
            
            var prizes = _userProgressService.GetLevelUpPrizes();
            int effectsCount = prizes.Length;

            if (prizes is {Length: > 0})
            {
                for (int i = 0; i < prizes.Length; i++)
                {
                    ElementConfig prize = prizes[i];
                    ElementInfoItemComponent prizeIcon = _elementInfoFactory.CreateElementWindowItem(prize, ElementInfoType.UserProgressWindow);

                    if (prizeIcon != null)
                    {
                        prizeIcon.transform.SetParent(transform);
                        prizeIcon.transform.localScale = Vector3.one;
                    }

                    _elementsStockService.Add(prize);

                    RectTransform elementTransform = prizeIcon.RectTransform;

                    var parameters = new MoveEffectParameters(elementTransform.position, default,
                        HudPanelType.ElementStock, EffectName.MoveToStock);
                    _effectsFactory.CreateEffect<MoveToUiEffect>(prizeIcon.RectTransform, parameters, () =>
                    {
                        effectsCount--;
                        Destroy(elementTransform.gameObject);

                        CheckEffects();
                    });
                }

                void CheckEffects()
                {
                    if (effectsCount == 0)
                    {
                        _messageBus.Fire<UpdateStockMessage>();
                    }
                }
            }
        }
    }
}