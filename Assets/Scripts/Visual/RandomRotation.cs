using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [Header("Min and Max Rotation (Degrees)")]
    public Vector3 minRotation = new Vector3(0f, 0f, 0f);
    public Vector3 maxRotation = new Vector3(360f, 360f, 360f);

    void Start()
    {
        float randomX = Random.Range(minRotation.x, maxRotation.x);
        float randomY = Random.Range(minRotation.y, maxRotation.y);
        float randomZ = Random.Range(minRotation.z, maxRotation.z);

        transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);
    }
}
