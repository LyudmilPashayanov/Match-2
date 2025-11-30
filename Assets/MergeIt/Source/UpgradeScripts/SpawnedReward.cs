using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpawnedReward : MonoBehaviour
{
    private const float SCALE_UP_ANIMATION_DURATION = 3f;
        
    [SerializeField] private Image _rewardImage;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetImage(Sprite sprite)
    {
        _rewardImage.sprite = sprite;
    }

    public void ScaleUpAnimation()
    {
        transform.DOScale(new Vector3(2,2,2), SCALE_UP_ANIMATION_DURATION);
    }
}
