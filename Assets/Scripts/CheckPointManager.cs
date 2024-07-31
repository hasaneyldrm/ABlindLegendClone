using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    private static CheckPointManager _instance;

    public static CheckPointManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CheckPointManager>();
            }
            return _instance;
        }
    }

    public AudioSource audioSource;
    public Collider2D checkpointCollider;
    public AudioClip customSound; // Bu checkpoint için özel bir ses (opsiyonel)
    public CheckPointManager nextCheckpoint; // Bir sonraki checkpoint

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // Sesin başlangıçta çalmaması için
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayCheckpointSound();
            Debug.Log("Checkpoint reached!");

            // Bir sonraki checkpoint'i aktif hale getir
            if (nextCheckpoint != null)
            {
                CheckPointManager.Instance.SetActiveCheckpoint(nextCheckpoint);
            }

            // Mevcut checkpoint'i yok et
            Destroy(gameObject);
        }
    }

    private void PlayCheckpointSound()
    {
        if (customSound != null)
        {
            audioSource.PlayOneShot(customSound);
        }
    }

    public void SetActiveCheckpoint(CheckPointManager checkpoint)
    {
        // Mevcut aktif checkpoint'i yok et
        if (_instance != null && _instance != checkpoint)
        {
            Destroy(_instance.gameObject);
        }

        // Yeni checkpoint'i aktif hale getir
        _instance = checkpoint;
        _instance.ActivateCheckpoint();
    }

    public void ActivateCheckpoint()
    {
        // Checkpoint aktif olduğunda yapılacak işlemler
        gameObject.SetActive(true);
    }
}
