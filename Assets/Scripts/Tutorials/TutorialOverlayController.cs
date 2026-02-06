using UnityEngine;
using UnityEngine.UI;

public class TutorialOverlayController : MonoBehaviour
{
    [SerializeField] private Image overlayImage;

    [SerializeField] private RectTransform targetrt;
    [SerializeField] private Vector2 targetPadding;
    [Header("UI References")]
    [SerializeField] private Canvas canvas;       // The Canvas this overlay belongs to

    [Header("Optional Settings")]
    private Material _material;

   private void Awake()
    {
        if (overlayImage == null)
            overlayImage = GetComponent<Image>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        // Instantiate material to avoid shared material issues
        _material = Instantiate(overlayImage.material);
        overlayImage.material = _material;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            FocusOn(targetrt, targetPadding);
        }
    }
    
    /// <summary>
    /// Focus the tutorial overlay on a UI element (RectTransform).
    /// </summary>
    /// <param name="target">Target RectTransform to highlight</param>
    /// <param name="padding">Optional padding around the target</param>
    public void FocusOn(RectTransform target, Vector2 padding)
    {
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        // Convert world → screen
        Vector2 screenMin = RectTransformUtility.WorldToScreenPoint(cam, corners[0]) - padding;
        Vector2 screenMax = RectTransformUtility.WorldToScreenPoint(cam, corners[2]) + padding;

        // Convert screen → local overlay space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            overlayImage.rectTransform, screenMin, cam, out Vector2 localMin);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            overlayImage.rectTransform, screenMax, cam, out Vector2 localMax);

        Vector2 localCenter = (localMin + localMax) * 0.5f;
        Vector2 localSize = localMax - localMin;

        Rect overlayRect = overlayImage.rectTransform.rect;

        // Normalize to UV space (0–1)
        Vector2 uvCenter = new Vector2(
            (localCenter.x - overlayRect.xMin) / overlayRect.width,
            (localCenter.y - overlayRect.yMin) / overlayRect.height
        );

        Vector2 uvSize = new Vector2(
            localSize.x / overlayRect.width,
            localSize.y / overlayRect.height
        );

        _material.SetVector("_FocusCenter", new Vector4(uvCenter.x, uvCenter.y, 0, 0));
        _material.SetVector("_FocusSize", new Vector4(uvSize.x, uvSize.y, 0, 0));
    }


    /// <summary>
    /// Clear the focus (remove highlight)
    /// </summary>
    public void ClearFocus()
    {
        _material.SetVector("_FocusSize", Vector4.zero);
    }
}