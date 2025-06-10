using UnityEngine;

public class NodesSpawner : MonoBehaviour
{
    public float spawnRadius = 1.85f; 
    public NodeBuilder nodeBuilder; 
    public float spawnInterval = 6f; 
    public float indicatorInterval = 3f; 
    public float minSpawnInterval = 2f; 
    public float spawnAccelerationDuration = 60f; 

    public GameObject nodePrefab;
    public GameObject indicator;
    public Animator animator;

    private float spawnTimer = 0f;
    private float indicatorTimer = 0f;
    private bool indicatorActive;
    private Vector2 spawnPosition;

    private float elapsedTime = 0f; 

    void Update()
    {
        elapsedTime += Time.deltaTime;

        
        float t = Mathf.Clamp01(elapsedTime / spawnAccelerationDuration);
        spawnInterval = Mathf.Lerp(4f, minSpawnInterval, t);

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && !indicatorActive)
        {
            SpawnIndicator();
        }

        if (indicatorActive)
        {
            indicatorTimer += Time.deltaTime;

            if (indicatorTimer >= indicatorInterval)
            {
                spawnTimer = 0;
                indicatorTimer = 0;
                indicatorActive = false;
                SpawnNode();
            }
        }
    }

    void SpawnIndicator()
    {
        indicatorActive = true;

        if (Random.Range(0f, 1f) > 0.5f)
        {
            spawnPosition = nodeBuilder.rightMostNode.transform.position +
                            (Random.Range(0.3f, spawnRadius) * Vector3.right);
        }
        else
        {
            spawnPosition = nodeBuilder.leftMostNode.transform.position +
                            (Random.Range(0.3f, spawnRadius) * Vector3.left);
        }

        spawnPosition.y = transform.position.y;
        indicator.transform.position = new Vector3(spawnPosition.x, indicator.transform.position.y, indicator.transform.position.z);
        animator.SetTrigger("Appear");
        animator.SetBool("Alive", true);
    }

    void SpawnNode()
    {
        animator.SetBool("Alive", false);

        GameObject newNode = Instantiate(nodePrefab, spawnPosition, Quaternion.identity);
        newNode.GetComponent<SpawnedNodes>().builder = nodeBuilder;
    }
}
