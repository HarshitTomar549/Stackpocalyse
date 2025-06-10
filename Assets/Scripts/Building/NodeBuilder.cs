using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public enum NodeType
{
    Normal,
    Human
}
[System.Serializable]
public struct Nodes
{
    public GameObject nodePrefab;
    public GameObject connectionPrefab;
}
public class NodeBuilder : MonoBehaviour
{
    public Nodes normalConnections;
    public Nodes humanConnections;
    public GameObject normalParticles;
    public GameObject humanParticles;
    public float autoConnectRadius = 2.5f;
    public int maxConnectionsPerNode = 4;
    public int minConnectionsOnSpawn = 2;
    public float minEdgeSpacing = 0.5f;

    private Node selectedNode;

    private Vector2 startMousePosition;
    private Vector2 mousePos;
    public List<NodeConnection> allExistingConnections;
    public Node rightMostNode;
    public Node upMostNode;
    public Node leftMostNode;

    public float minReach = 0.5f; 
    float towerTop;

    public Node[] visibleNodes;
    [Header("Preview Visuals")]
    public GameObject ghostNodePrefab;
    private GameObject ghostNodeInstance;
    private LineRenderer minRangeCircle;
    private LineRenderer maxRangeCircle;
    private List<LineRenderer> previewConnections = new();
    public AudioClip[] woodPlace;
    public AudioClip[] startPlacing;
    public bool active;
    void Start()
    {
        towerTop = upMostNode.transform.position.y;
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        foreach (Node node in allNodes)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(node.transform.position, autoConnectRadius);
            List<Node> nearbyNodes = new();

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Node otherNode))
                {
                    if (otherNode.connections.Count < maxConnectionsPerNode)
                        nearbyNodes.Add(otherNode);
                }
            }
            // Sort by distance
            nearbyNodes.Sort((a, b) =>
                Vector2.Distance(node.transform.position, a.transform.position)
                .CompareTo(Vector2.Distance(node.transform.position, b.transform.position)));


            // Connect to closest N nodes
            int connectionsMade = 0;
            foreach (Node target in nearbyNodes)
            {

                if (!AreAlreadyConnected(node, target))
                {
                    if (IsConnectionValid(node, target, allExistingConnections))
                    {
                        if(visibleNodes != null &&visibleNodes.Contains(target) && visibleNodes.Contains(node))
                            CreateConnection(node, target, true);
                        else
                            CreateConnection(node, target, false);
                    }
                    connectionsMade++;
                }
            }
            Debug.Log(connectionsMade);
        }
        if (!active) return;

        ghostNodeInstance = Instantiate(ghostNodePrefab);
        ghostNodeInstance.SetActive(false);

        // Create range rings
        minRangeCircle = CreateCircleRenderer(Color.red);
        maxRangeCircle = CreateCircleRenderer(Color.cyan);
    }
    void Update()
    {
        if (!active) return;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // pressedthis frame
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Collider2D hit = Physics2D.OverlapPoint(currentMousePos);
                Debug.Log(hit);
                startMousePosition = currentMousePos;
                if (hit && hit.TryGetComponent<Node>(out Node node))
                {
                    selectedNode = node;
                    AudioManager.Instance.PlaySFX(startPlacing);
                }
                else
                {
                    selectedNode = null;
                }
            }
            if (selectedNode != null)
            {
                
                mousePos = currentMousePos;

                // Activate ghost node and rings
                ghostNodeInstance.SetActive(true);
                ghostNodeInstance.transform.position = mousePos;

                DrawCircle(minRangeCircle, mousePos, minReach);
                DrawCircle(maxRangeCircle, mousePos, autoConnectRadius);

                ShowPreviewConnections(mousePos);
            }
            else
            {
                ghostNodeInstance.SetActive(false);
                minRangeCircle.enabled = false;
                maxRangeCircle.enabled = false;
                ClearPreviewConnections();
            }

        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && selectedNode != null)
        {
            ghostNodeInstance.SetActive(false);
            minRangeCircle.enabled = false;
            maxRangeCircle.enabled = false;
            ClearPreviewConnections();
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(startMousePosition, currentMousePos) > minReach)
            {

                CreateNodeWithConnections(currentMousePos);
                
            }
            else
            {
                Debug.Log("Click too short to create a node");
            }
        }
        

    }
    void ShowPreviewConnections(Vector2 pos)
    {
        ClearPreviewConnections();

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, autoConnectRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Node node))
            {
                float distance = Vector2.Distance(pos, node.transform.position);
                if (distance > minReach && node.connections.Count < maxConnectionsPerNode)
                {
                    LineRenderer lr = new GameObject("PreviewLine").AddComponent<LineRenderer>();
                    lr.startWidth = lr.endWidth = 0.05f;
                    lr.positionCount = 2;
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                    lr.startColor = lr.endColor = Color.yellow;
                    lr.useWorldSpace = true;
                    lr.SetPosition(0, pos);
                    lr.SetPosition(1, node.transform.position);
                    previewConnections.Add(lr);
                }
            }
        }
    }
    LineRenderer CreateCircleRenderer(Color color)
    {
        var go = new GameObject("CircleRenderer");
        var lr = go.AddComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = 64;
        lr.startWidth = lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;
        return lr;
    }

    void DrawCircle(LineRenderer lr, Vector2 center, float radius)
    {
        lr.enabled = true;
        for (int i = 0; i < lr.positionCount; i++)
        {
            float angle = i * Mathf.PI * 2f / lr.positionCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius + (Vector3)center;
            lr.SetPosition(i, pos);
        }
    }

    void ClearPreviewConnections()
    {
        foreach (var lr in previewConnections)
        {
            if (lr != null) Destroy(lr.gameObject);
        }
        previewConnections.Clear();
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (startMousePosition != null) Gizmos.DrawSphere(startMousePosition, 0.1f);
        if (mousePos != null) Gizmos.DrawSphere(mousePos, 0.1f);

        Gizmos.DrawLine(startMousePosition, mousePos);
    }
    public void CreateNodeWithConnections(Vector2 position, NodeType type = NodeType.Normal)
    {
        
        // Find nearby nodes
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, autoConnectRadius);
        List<Node> nearbyNodes = new();

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Node otherNode))
            {
                if (otherNode.connections.Count < maxConnectionsPerNode)
                    nearbyNodes.Add(otherNode);
            }
        }
        if (nearbyNodes.Count < minConnectionsOnSpawn)
        {
            return;
        }
        GameObject newNodeObject = CreateNode(position,type);
        Node newNode = newNodeObject.GetComponent<Node>();
        
        if (type != NodeType.Human)
        {
            AudioManager.Instance.PlaySFX(woodPlace);
            if (newNode.transform.position.x > rightMostNode.transform.position.x)
            {
                rightMostNode = newNode;
            }
            else if (newNode.transform.position.x < leftMostNode.transform.position.x)
            {
                leftMostNode = newNode;
            }
            else if (newNode.transform.position.y > upMostNode.transform.position.y)
            {
                upMostNode = newNode;
                towerTop = upMostNode.transform.position.y;
            }
        }
        

        // Sort by distance
        nearbyNodes.Sort((a, b) =>
            Vector2.Distance(position, a.transform.position)
            .CompareTo(Vector2.Distance(position, b.transform.position)));


        // Connect to closest N nodes
        int connectionsMade = 0;
        foreach (Node target in nearbyNodes)
        {

            if (!AreAlreadyConnected(newNode, target))
            {
                if (IsConnectionValid(newNode, target, allExistingConnections))
                {
                    CreateConnection(newNode, target, true, type);
                }
                connectionsMade++;
            }
        }
    }
    bool IsConnectionValid(Node newNode, Node target, List<NodeConnection> allConnections)
    {
        Vector2 a = newNode.transform.position;
        Vector2 b = target.transform.position;

        foreach (var conn in allConnections)
        {
            Vector2 c = conn.nodeA.transform.position;
            Vector2 d = conn.nodeB.transform.position;

            // Avoid checking the same nodes
            if (conn.nodeA == newNode || conn.nodeB == newNode ||
                conn.nodeA == target || conn.nodeB == target)
                continue;

            if (DoLinesIntersect(a, b, c, d))
                return false;

            if (SegmentsTooClose(a, b, c, d, minEdgeSpacing))
                return false;

        }
        return true;
    }
    bool DoLinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float o1 = Orientation(p1, p2, p3);
        float o2 = Orientation(p1, p2, p4);
        float o3 = Orientation(p3, p4, p1);
        float o4 = Orientation(p3, p4, p2);

        return (o1 != o2 && o3 != o4);
    }

    float Orientation(Vector2 a, Vector2 b, Vector2 c)
    {
        float val = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
        if (val == 0) return 0; // colinear
        return (val > 0) ? 1 : 2; // clockwise or counterclockwise
    }
    bool SegmentsTooClose(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, float threshold)
    {
        float d1 = DistancePointToSegment(a1, b1, b2);
        float d2 = DistancePointToSegment(a2, b1, b2);
        float d3 = DistancePointToSegment(b1, a1, a2);
        float d4 = DistancePointToSegment(b2, a1, a2);

        return Mathf.Min(d1, d2, d3, d4) < threshold;
    }

    float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        Vector2 projection = a + t * ab;
        return Vector2.Distance(p, projection);
    }

    GameObject CreateNode(Vector2 position, NodeType type = NodeType.Normal)
    {
        if(type == NodeType.Human)
        {
            Instantiate(humanParticles, position, quaternion.identity);
            return Instantiate(humanConnections.nodePrefab, position, Quaternion.identity);
        }
        return Instantiate(normalConnections.nodePrefab, position, Quaternion.identity);
    }

    void CreateConnection(Node a, Node b, bool lineRenderer,NodeType type = NodeType.Normal)
    {
        GameObject conn;
        if (type == NodeType.Human)
        {
            conn = Instantiate(humanConnections.connectionPrefab);
        }
        else
        {
            conn = Instantiate(normalConnections.connectionPrefab);
        }
        
        conn.GetComponent<LineRenderer>().enabled = lineRenderer;
        NodeConnection connection = conn.GetComponent<NodeConnection>();
        allExistingConnections.Add(connection);
        connection.nodeA = a;
        connection.nodeB = b;
        a.connections.Add(connection);
        b.connections.Add(connection);
    }
    bool AreAlreadyConnected(Node a, Node b)
    {
        return a.connections.Any(c => c.nodeA == b || c.nodeB == b);
    }

}
