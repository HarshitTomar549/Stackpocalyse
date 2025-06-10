using Unity.VisualScripting;
using UnityEngine;

public class PositionLerper : MonoBehaviour
{
    public Transform lerpTo;
    public float speed;

    void Update()
    {

        float y = Mathf.Lerp(transform.position.y, lerpTo.position.y, Time.deltaTime * speed);

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
