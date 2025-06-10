using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    public Vector3 clickScale = new Vector3(0.95f, 0.95f, 1f);
    public float tweenTime = 0.1f;


    public AudioClip hover;
    public AudioClip click;
    private Tween currentTween;

    private void Start()
    {
        transform.localScale = normalScale;
    }

    private void ScaleTo(Vector3 targetScale, float duration, Ease ease)
    {
        currentTween?.Kill();
        currentTween = transform.DOScale(targetScale, duration).SetEase(ease);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ScaleTo(hoverScale, tweenTime, Ease.OutQuad);
        AudioManager.Instance.PlaySFX(hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ScaleTo(normalScale, tweenTime, Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ScaleTo(clickScale, tweenTime / 2f, Ease.InOutQuad);
        AudioManager.Instance.PlaySFX(click);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ScaleTo(hoverScale, tweenTime, Ease.OutQuad);
    }
}
