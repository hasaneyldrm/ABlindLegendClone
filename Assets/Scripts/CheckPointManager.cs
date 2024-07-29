using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public AudioClip leftAudioClip;
    public AudioClip rightAudioClip;
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
            if (rightAudioClip != null)
            {
                audioSource.PlayOneShot(rightAudioClip);
            }
            Debug.Log("Checkpoint reached!");
        }
    }
}
