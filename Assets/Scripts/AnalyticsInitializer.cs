using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsInitializer : MonoBehaviour
{
    private static bool _initialized = false;

    private async void Awake()
    {
        if (_initialized)
        {
            Destroy(gameObject);
            return;
        }

        _initialized = true;
        DontDestroyOnLoad(gameObject);

        await Initialize();
    }

    private async Task Initialize()
    {
        await UnityServices.InitializeAsync();
    }

}
