using System.Collections;
using UnityEngine;

public class CheckpointAudioPlayer : MonoBehaviour
{
    public float moveCheckInterval = 5f; // Karakter hareket ederken checkpoint kontrol aralığı
    public float stopCheckInterval = 3f; // Karakter durduğunda checkpoint kontrol aralığı
    public AudioClip[] checkpointClips; // Checkpoint sesleri listesi

    private AudioSource checkpointAudioSource;
    private CheckPointManager[] checkpoints;
    private bool isMoving;
    private float currentCheckInterval;
    private float lastCheckTime; // Son kontrol zamanı

    void Start()
    {
        checkpointAudioSource = gameObject.AddComponent<AudioSource>(); // Checkpoint sesleri için ayrı bir AudioSource ekleyin
        checkpointAudioSource.loop = false; // Seslerin döngüde olmamasını sağlayın

        if (checkpointAudioSource == null)
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
                Vector2 direction = checkpoint.transform.position - transform.position;
                float panStereo = direction.x / 10f; // Sağdan veya soldan gelen ses

                PlayCheckpointSound(checkpointClips[Random.Range(0, checkpointClips.Length)], panStereo, direction.magnitude);

                break; // İlk uygun checkpoint'te dur
            }
        }
    }

    private void PlayCheckpointSound(AudioClip clip, float panStereo, float distance)
    {
        if (clip == null)
        {
            return;
        }

        // Checkpoint sesleri için pan değerlerini ayarla, diğer sesler etkilenmez
        checkpointAudioSource.panStereo = Mathf.Clamp(panStereo, -1f, 1f);

        // Sesin uzaklığa göre azaltılması (yankı etkisi)
        float distanceFactor = Mathf.Clamp01(1 - (distance / 10f));
        checkpointAudioSource.volume = 0.5f + 0.5f * distanceFactor; // Minimum 0.5, maksimum 1.0

        checkpointAudioSource.clip = clip;
        checkpointAudioSource.Play();
    }
}
