// Copyright (c) 2024, Awessets

using DG.Tweening;
using MergeIt.Core.Configs.Elements;
using MergeIt.Core.ElementsStock;
using MergeIt.Core.FieldElements;
using MergeIt.Core.Messages;
using MergeIt.Core.Services;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;
using UnityEngine.UI;

namespace MergeIt.Game.ElementsStock
{
    public class ElementsStockComponent : MonoBehaviour, IElementsStockComponent
    {
        [SerializeField]
        private Button _elementButton;
        
        [SerializeField]
        private Transform _elementContainer;
       
        [SerializeField] private RectTransform _hand;
        [SerializeField] private Material _handMaterial;
        private Tween _handTween;

        private IMessageBus _messageBus;
        private IGameFieldService _fieldService;
        private IElementsStockService _stockService;

        private ElementConfig _currentElement;
        private GameObject _iconPrefab;
        private Sequence _handLoopSequence;
        
        public void Initialize()
        {
            _elementButton.onClick.AddListener(OnElementClick);

            _messageBus = DiContainer.Get<IMessageBus>();
            _fieldService = DiContainer.Get<IGameFieldService>();
            _stockService = DiContainer.Get<IElementsStockService>();
            
            _messageBus.AddListener<UpdateStockMessage>(UpdateStockMessageHandler);

            _handMaterial.color = new Color(0,0,0,0);
            SetupElement(_stockService.GetNext());
        }
        
        public void OnDestroy()
        {
            _elementButton.onClick.RemoveListener(OnElementClick);
            _messageBus.RemoveListener<UpdateStockMessage>(UpdateStockMessageHandler);
        }

        public void SetupElement(ElementConfig elementConfig)
        {
            if (elementConfig != null)
            {
                gameObject.SetActive(true);
                _currentElement = elementConfig;
                UpdateView();
                StartHandLoop();
            }
            else
            {
                Hide();
                StopHandLoop();
            }
        }

        private void StartHandLoop()
        {
            _handLoopSequence = DOTween.Sequence();
            _handLoopSequence.Append(_handMaterial.DOFade(1,0.4f));
            _handLoopSequence.Append(_hand.DOScale(1.2f, 1f).SetLoops(3, LoopType.Yoyo).SetEase(Ease.InSine));
            _handLoopSequence.Append(_handMaterial.DOFade(0, 0.4f));
            _handLoopSequence.AppendInterval(5f);
            _handLoopSequence.SetLoops(-1);
        }

        private void StopHandLoop()
        {
            _handLoopSequence?.Kill();
            _handLoopSequence = null;
        }
        
        public void PopElement()
        {
            if (_currentElement != null)
            {
                GridPoint? pointContainer = _fieldService.GetFreeCell();

                if (pointContainer != null)
                {
                    GridPoint point = pointContainer.Value;

                    IFieldElement newElement = _fieldService.CreateNewElement(_currentElement, point);
                    
                    var message = new CreateElementMessage
                    {
                        NewElement = newElement,
                        FromPosition = _elementContainer.position,
                        ToPoint = point,
                    };
                    _messageBus.Fire(message);
                    
                    if (_iconPrefab)
                    {
                        Destroy(_iconPrefab);
                    }

                    if (_stockService.Remove())
                    {
                        SetupElement(_stockService.GetNext());
                    }
                    else
                    {
                        Hide();
                    }
                }
            }
        }

        private void OnElementClick()
        {
           PopElement();
        }

        private void Hide()
        {
            _currentElement = null;
            gameObject.SetActive(false);
        }

        private void UpdateView()
        {
            if (_currentElement != null)
            {
                _iconPrefab = Instantiate(_currentElement.CommonSettings.Icon.gameObject);
                if (_iconPrefab)
                {
                    _iconPrefab.transform.SetParent(_elementContainer);
                    _iconPrefab.transform.localScale = Vector3.one;
                }
            }
        }

        private void UpdateStockMessageHandler(UpdateStockMessage _)
        {
            if (!_currentElement)
            {
                SetupElement(_stockService.GetCurrent());
            }
        }
    }
}