using UnityEngine;

public class UIParallax : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public RectTransform rectTransform;
        public Transform worldTransform;
        public float parallaxMultiplier = 0.1f;
    }

    public ParallaxLayer[] layers;
    public float smoothSpeed = 5f;

    private Vector2 screenCenter;

    void Start()
    {
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        Vector2 mouseDelta = (Vector2)Input.mousePosition - screenCenter;
        mouseDelta /= screenCenter; // Normalize from -1 to 1

        foreach (var layer in layers)
        {
            Vector2 offset = mouseDelta * -layer.parallaxMultiplier * 100f;

            if (layer.rectTransform != null)
            {
                Vector2 targetUIPos = offset;
                layer.rectTransform.anchoredPosition = Vector2.Lerp(
                    layer.rectTransform.anchoredPosition,
                    targetUIPos,
                    Time.deltaTime * smoothSpeed
                );
            }
            else if (layer.worldTransform != null)
            {
                Vector3 currentPos = layer.worldTransform.localPosition;
                Vector3 targetWorldPos = new Vector3(offset.x, offset.y, currentPos.z);

                layer.worldTransform.localPosition = Vector3.Lerp(
                    currentPos,
                    targetWorldPos,
                    Time.deltaTime * smoothSpeed
                );
            }

        }
    }
}
