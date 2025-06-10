using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpawnedNodes : MonoBehaviour
{
    public NodeBuilder builder;
    public ParticleSystem bloodEffect;
    public DecalProjector bloodSplatter;

    public AudioSource screemAudioSource;
    public AudioClip hitSfx;
    void Start()
    {
        screemAudioSource.Play();
    }
    void Update()
    {
        int nodesInRange = 0;
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, builder.autoConnectRadius);
        foreach (Collider2D collider in hit)
        {
            if (collider.TryGetComponent<Node>(out Node node))
            {
                nodesInRange++;
            }
        }
        if (nodesInRange >= builder.minConnectionsOnSpawn)
        {
            screemAudioSource.Stop();
            AudioManager.Instance.PlaySFX(hitSfx);
            CameraShaker.Instance.ShakeCamera(1f, 0.3f);
            builder.CreateNodeWithConnections(transform.position,NodeType.Human);

            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {

            screemAudioSource.Stop();
            AudioManager.Instance.PlaySFX(hitSfx);

            CameraShaker.Instance.ShakeCamera(1.25f, 0.4f);
            screemAudioSource.Play();
            
            Instantiate(bloodEffect, transform.position, Quaternion.identity);

            Vector3 bloodEffectPosition = new Vector3(transform.position.x, transform.position.y, -2f);
            DecalProjector bloodSplash = Instantiate(bloodSplatter, bloodEffectPosition, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
