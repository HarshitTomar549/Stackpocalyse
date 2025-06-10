using UnityEngine;

public class CloudAnim : MonoBehaviour
{
    public Vector2 driftDirection = new Vector2(0.01f, 0f);
    public float driftSpeed = 0.1f;
    public float noiseStrength = 0.05f; 
    public float noiseFrequency = 0.2f; 

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float time = Time.time;
        Vector2 noise = new Vector2(
            Mathf.PerlinNoise(time * noiseFrequency, 0.0f),
            Mathf.PerlinNoise(0.0f, time * noiseFrequency)
        );

        noise -= Vector2.one * 0.5f;

        Vector2 movement = driftDirection * driftSpeed + noise * noiseStrength;
        transform.position = initialPosition + new Vector3(movement.x, movement.y, 0f) * time;
    }
}
