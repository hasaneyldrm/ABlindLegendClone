using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public AudioClip checkpointSound;
    public AudioSource audioSource;
    public Collider2D checkpointCollider;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // Sesin başlangıçta çalmaması için
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (checkpointSound != null)
            {
                audioSource.PlayOneShot(checkpointSound);
            }
            Debug.Log("Checkpoint reached!");
        }
    }
}
