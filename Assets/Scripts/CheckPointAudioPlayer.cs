using System.Collections;
using UnityEngine;

public class CheckpointAudioPlayer : MonoBehaviour
{
    public float moveCheckInterval = 5f; // Karakter hareket ederken checkpoint kontrol aralığı
    public float stopCheckInterval = 3f; // Karakter durduğunda checkpoint kontrol aralığı
    public float checkDistance = 10f; // Checkpoint'e olan maksimum mesafe
    public AudioSource backgroundAudioSource; // Arka plan sesleri için
    public AudioSource footstepAudioSource; // Ayak sesleri için
    public AudioClip[] checkpointClips; // Checkpoint sesleri listesi

    private AudioSource audioSource;
    private CheckPointManager[] checkpoints;
    private bool isMoving;
    private float currentCheckInterval;
    private float lastCheckTime; // Son kontrol zamanı

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null || backgroundAudioSource == null || footstepAudioSource == null)
        {
            Debug.LogError("AudioSource component not found on " + gameObject.name);
        }

        // Sahnede bulunan tüm CheckPointManager bileşenlerini bul
        checkpoints = FindObjectsOfType<CheckPointManager>();

        // Başlangıçta karakterin hareket halinde olup olmadığını kontrol et
        isMoving = false;
        currentCheckInterval = stopCheckInterval;
        lastCheckTime = Time.time;

        // Checkpoint'leri kontrol etmeye başla
        StartCoroutine(CheckForNearbyCheckpoints());
    }

    void Update()
    {
        // Karakterin hareket edip etmediğini kontrol et
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool wasMoving = isMoving;
        isMoving = moveInput != Vector2.zero;

        // Hareket durumu değiştiyse kontrol aralığını güncelle
        if (isMoving != wasMoving)
        {
            currentCheckInterval = isMoving ? moveCheckInterval : stopCheckInterval;
            lastCheckTime = Time.time; // Zamanlayıcıyı sıfırla
        }
    }

    private IEnumerator CheckForNearbyCheckpoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentCheckInterval); // Belirli aralıklarla kontrol et

            foreach (var checkpoint in checkpoints)
            {
                float distance = Vector2.Distance(transform.position, checkpoint.transform.position);

                if (distance <= checkDistance)
                {
                    Vector2 direction = checkpoint.transform.position - transform.position;
                    float panStereo = direction.x > 0 ? 0.6f : -0.6f; // Sağdan veya soldan gelen ses

                    PlayCheckpointSound(checkpointClips[Random.Range(0, checkpointClips.Length)], panStereo, distance);

                    break; // İlk uygun checkpoint'te dur
                }
            }
        }
    }

    private void PlayCheckpointSound(AudioClip clip, float panStereo, float distance)
    {
        if (clip == null)
        {
            return;
        }

        // Checkpoint sesi çalarken pan değerlerini ayarla
        audioSource.panStereo = panStereo;

        // Sesin uzaklığa göre azaltılması (yankı etkisi)
        float distanceFactor = Mathf.Clamp01(1 - (distance / checkDistance));
        audioSource.volume = 0.5f + 0.5f * distanceFactor; // Minimum 0.5, maksimum 1.0

        audioSource.PlayOneShot(clip);
    }
}
