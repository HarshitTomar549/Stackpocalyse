using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Node : MonoBehaviour
{
    public List<NodeConnection> connections = new List<NodeConnection>();
    private float startYPosition;
    public List<SpringJoint2D> springs = new List<SpringJoint2D>();
    public AudioClip[] breakSfx;
    void Start()
    {
        GameState.instance?.RegisterNode(this);
        startYPosition = transform.position.y;
    }
    public void AddSpring(Node nodeB, LineRenderer connection, NodeType type = NodeType.Normal)
    {
        SpringJoint2D spring = gameObject.AddComponent<SpringJoint2D>();
        spring.connectedBody = nodeB.GetComponent<Rigidbody2D>();
        spring.autoConfigureDistance = false;
        spring.distance = Vector2.Distance(transform.position, nodeB.transform.position);
        spring.dampingRatio = 0.7f;
        spring.frequency = 5f;
        spring.breakAction = JointBreakAction2D.Destroy;
        spring.breakForce = 250f;

        springs.Add(spring);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (startYPosition >= 6.5 && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GameState.instance.GameOver();
        }
    }
    private void OnJointBreak2D(Joint2D brokenJoint)
    {
        CameraShaker.Instance.ShakeCamera(2f, 0.5f);
        AudioManager.Instance.PlaySFX(breakSfx);
        foreach (NodeConnection connection in connections)
        {
            if (connection != null)
            {
                if (connection.nodeB.gameObject == brokenJoint.connectedBody.gameObject)
                {
                    Destroy(connection.gameObject);
                }
            }
        }
    }
    void BreakConnections()
    {
        foreach (var connection in connections)
        {
            if (connection != null && connection.nodeA == this) Destroy(connection.gameObject);
        }
    }
    
}
