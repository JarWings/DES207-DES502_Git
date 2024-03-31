using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    public AudioClip[] collideSounds;
    public float forceRequirement = 4f;
    private Rigidbody2D rbody;
    public float minRestTime = .5f;

    private float lastSoundTime = 0f;

    private void Start()
    {
        TryGetComponent(out rbody);
    }

    private void Update()
    {
        if (lastSoundTime > 0f) lastSoundTime -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (lastSoundTime > 0f) return;

        if (rbody == null || rbody.velocity.magnitude > forceRequirement)
        {
            float mult = rbody != null ? 1f * Mathf.Clamp(rbody.velocity.magnitude / 6f, .8f, 1.4f) : 1f; // changes the pitch depending on the force of the impact

            AudioManager.PlayAudio(AudioType.soundFX, null, collideSounds, transform.position, transform, 1f, Random.Range(.9f, 1.1f) * mult, 1, 0, 20);
            lastSoundTime = minRestTime;
        }
    }
}
