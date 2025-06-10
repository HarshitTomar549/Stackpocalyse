using UnityEngine;

public class NodesSpawner : MonoBehaviour
{
    public float spawnRadius = 5f; 
    public NodeBuilder nodeBuilder; 
    public float spawnInterval = 4f; 
    public float indicatorInterval = 4f; 
    public GameObject nodePrefab;
    public GameObject indicator;

    public Animator animator;

    private float spawnTimer = 0f;
    private float indicatorTimer = 0f;
    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && !indicatorActive)
        {
            SpawnIndicator();
        }
        if (indicatorActive)
        {
            indicatorTimer += Time.deltaTime;
            if (indicatorTimer >= indicatorInterval )
            {
                spawnTimer = 0;
                indicatorTimer = 0;
                indicatorActive = false;
                SpawnNode();
            }
        }
    }
    bool indicatorActive;
    Vector2 spawnPosition;
    void SpawnIndicator()
    {
        
        indicatorActive = true;
        if (Random.Range(0f, 1f) > 0.5f)
        {
            spawnPosition = nodeBuilder.rightMostNode.transform.position +
                                (Random.Range(.3f, spawnRadius) * Vector3.right);
        }
        else
        {
            spawnPosition = nodeBuilder.leftMostNode.transform.position +
                                (Random.Range(.3f, spawnRadius) * Vector3.left);
        }

        spawnPosition.y = transform.position.y; 
        indicator.transform.position = new Vector3(spawnPosition.x,indicator.transform.position.y,indicator.transform.position.z); // Update the indicator position
        animator.SetTrigger("Appear");
        animator.SetBool("Alive",true);
    }
    void SpawnNode()
    {
        animator.SetBool("Alive",false);

        GameObject newNode = Instantiate(nodePrefab, spawnPosition, Quaternion.identity);
        newNode.GetComponent<SpawnedNodes>().builder = nodeBuilder;
    }
}
