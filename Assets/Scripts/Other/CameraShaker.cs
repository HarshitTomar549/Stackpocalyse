using UnityEngine;
using Unity.Cinemachine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;

    private CinemachineCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float shakeDuration;
    private float initialIntensity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        virtualCam = GetComponent<CinemachineCamera>();
        if (virtualCam != null)
        {
            noise = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        }
    }

    private void Update()
    {
        if (noise == null) return;

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                StopShake();
            }
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
        if (noise == null) return;

        noise.AmplitudeGain = intensity;
        initialIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }

    public void StopShake()
    {
        if (noise == null) return;
        noise.AmplitudeGain = 0f;
    }
}
