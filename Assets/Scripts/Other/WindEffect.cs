using UnityEngine;

public class WindEffect : MonoBehaviour
{
    public SpringJoint2D[] springJoints; 
    public Vector2 windDirection = new Vector2(1f, 0f); 
    public float windStrength = 10f; 
    public float windVariationFrequency = 1f; 
    public float windVariationAmount = 5f; 

    private Rigidbody2D[] rigidbodies;

    void Start()
    {
        rigidbodies = new Rigidbody2D[springJoints.Length];
        for (int i = 0; i < springJoints.Length; i++)
        {
            rigidbodies[i] = springJoints[i].GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {
        float variableStrength = windStrength + Mathf.Sin(Time.time * windVariationFrequency) * windVariationAmount;
        Vector2 windForce = windDirection.normalized * variableStrength;

        
        foreach (var rb in rigidbodies)
        {
            if (rb != null)
            {
                rb.AddForce(windForce);
            }
        }
    }
}
