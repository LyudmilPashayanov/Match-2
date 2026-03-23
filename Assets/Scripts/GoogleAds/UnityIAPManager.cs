using UnityEngine;
using Unity.Services.Core;
using UnityEngine.Purchasing;
using System.Collections.Generic;

public class UnityIAPManager : MonoBehaviour
{
    public const string NoAdsProductId = "no_ads";

    public static UnityIAPManager Instance { get; private set; }

    private StoreController _storeController;
    private bool _isInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeIAP();
    }

    private async void InitializeIAP()
    {
        try
        {
            await UnityServices.InitializeAsync();

            _storeController = UnityIAPServices.StoreController();

            // Attach event handlers BEFORE calls
            _storeController.OnPurchasePending += OnPurchasePending;
            _storeController.OnPurchaseFailed += OnPurchaseFailed;
            _storeController.OnProductsFetched += OnProductsFetched;
            _storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            _storeController.OnPurchasesFetched += OnPurchasesFetched;
            _storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

            // Step 1: Connect
            await _storeController.Connect();

            Debug.Log("IAP Connected");

            // Step 2: Fetch products
            var products = new List<ProductDefinition>
            {
                new ProductDefinition(NoAdsProductId, ProductType.NonConsumable)
            };

            _storeController.FetchProducts(products);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"IAP Init failed: {e.Message}");
            Invoke(nameof(InitializeIAP), 5f);
        }
    }

    // Step 3: Products fetched
    private void OnProductsFetched(List<Product> products)
    {
        Debug.Log("Products fetched");

        _isInitialized = true;

        // Step 4: Fetch purchases (restore)
        _storeController.FetchPurchases();
    }

    private void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        Debug.LogError($"Product fetch failed: {failure.FailureReason}");
    }

    // Restore purchases
    private void OnPurchasesFetched(Orders orders)
    {
        Debug.Log("Purchases fetched (restore)");

        foreach (var order in orders.ConfirmedOrders)
        {
            ProcessOrder(order);
        }
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        Debug.LogError($"Purchases fetch failed: {failure.FailureReason}");
    }

    // New purchases
    private void OnPurchasePending(PendingOrder order)
    {
        Debug.Log("Purchase pending received");

        ProcessOrder(order);

        // REQUIRED: confirm purchase
        _storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseFailed(FailedOrder order)
    {
        foreach (var item in order.CartOrdered.Items())
        {
            var productId = item.Product.definition.id;

            Debug.LogError($"Purchase failed: {productId}, Reason: {order.FailureReason}");
        }
    }

    // 🔥 Centralized processing (PRO-level pattern)
    private void ProcessOrder(Order order)
    {
        if (order?.CartOrdered == null)
        {
            Debug.LogError("Invalid order (no cart)");
            return;
        }

        foreach (var item in order.CartOrdered.Items())
        {
            var productId = item?.Product?.definition?.id;

            if (string.IsNullOrEmpty(productId))
            {
                Debug.LogError("Invalid product in order");
                continue;
            }

            Debug.Log($"Processing product: {productId}");

            if (productId == NoAdsProductId)
            {
                GrantNoAds();
            }
        }
    }

    // UI calls this
    public void BuyNoAds()
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("IAP not initialized yet.");
            return;
        }

        if (AdsEntitlement.HasNoAds())
        {
            Debug.Log("Already owns No Ads.");
            return;
        }

        Debug.Log("Starting purchase flow...");
        _storeController.PurchaseProduct(NoAdsProductId);
    }

    private void GrantNoAds()
    {
        if (AdsEntitlement.HasNoAds())
            return;

        AdsEntitlement.SetNoAds(true);

        Debug.Log("No Ads entitlement granted");

        var panel = FindObjectOfType<RewardedAdGatePanel>();
        if (panel != null && panel.IsOpen)
        {
            panel.SendMessage("Close", SendMessageOptions.DontRequireReceiver);
        }
    }

    public bool CanPurchaseNoAds()
    {
        return _isInitialized && !AdsEntitlement.HasNoAds();
    }
}