using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointAudioPlayer : MonoBehaviour
{
    public float moveCheckInterval = 5f; // Karakter hareket ederken checkpoint kontrol aralığı
    public float stopCheckInterval = 3f; // Karakter durduğunda checkpoint kontrol aralığı
    public AudioClip[] checkpointClips; // Checkpoint sesleri listesi

    private AudioSource checkpointAudioSource;
    private bool isMoving;
    private float currentCheckInterval;
    private float lastCheckTime; // Son kontrol zamanı

    private CheckPointManager activeCheckpoint;

    void Start()
    {
        checkpointAudioSource = gameObject.AddComponent<AudioSource>(); // Checkpoint sesleri için ayrı bir AudioSource ekleyin
        checkpointAudioSource.loop = false; // Seslerin döngüde olmamasını sağlayın

        if (checkpointAudioSource == null)
        {
            Debug.LogError("AudioSource component not found on " + gameObject.name);
        }

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

        // Aktif checkpoint'i kontrol et
        activeCheckpoint = CheckPointManager.Instance;
    }

    private IEnumerator CheckForNearbyCheckpoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentCheckInterval); // Belirli aralıklarla kontrol et

            if (activeCheckpoint != null)
            {
                Vector2 direction = activeCheckpoint.transform.position - transform.position;
                float panStereo = direction.x / 10f; // Sağdan veya soldan gelen ses

                PlayCheckpointSound(GetRandomCheckpointClip(), panStereo, direction.magnitude);
            }
        }
    }

    private AudioClip GetRandomCheckpointClip()
    {
        if (checkpointClips.Length == 0)
        {
            return null;
        }

        // Rastgele bir ses seç ve oynat
        int index = Random.Range(0, checkpointClips.Length);
        return checkpointClips[index];
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
