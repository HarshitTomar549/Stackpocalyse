using UnityEngine;

public class NodeConnection : MonoBehaviour
{
    public Node nodeA;
    public Node nodeB;
    public LineRenderer lineRenderer;
    public NodeType type = NodeType.Normal;
    void Start()
    {
        nodeA.AddSpring(nodeB, lineRenderer,type );
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
        }
    }
    float currentForce;
    public Color highTensionColor = new Color(0.878f, 0.145f, 0.020f, 1);
    public Color lowTensionColor = new Color(0.875f, 0.7f, 0.020f, 1);
    public bool updateColor = true;

    void Update()
    {
        
        foreach (SpringJoint2D spring in nodeA.springs)
        {
            if (spring != null)
            {
                lineRenderer.SetPosition(0, nodeA.transform.position);
                lineRenderer.SetPosition(1, nodeB.transform.position);
                
                if (!updateColor) return;

                currentForce = spring.GetReactionForce(Time.fixedDeltaTime).magnitude;
                // Normalize
                float t = Mathf.Clamp01(currentForce / (spring.breakForce * 0.75f));

                // Lerp color from green to red
                Color springColor = Color.Lerp(lowTensionColor, highTensionColor, t);

                lineRenderer.material.color = springColor;

                
            }
        }
        
    }
}
