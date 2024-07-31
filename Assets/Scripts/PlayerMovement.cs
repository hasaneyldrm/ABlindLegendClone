using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    public AudioClip[] footstepClips; // Adım seslerini içeren dizi
    private AudioSource audioSource;
    public float stepInterval = 0.5f; // Adımlar arasındaki süre
    private float stepTimer;

    private bool wasMoving = false; // Önceki hareket durumu

    // Arka plan sesi için değişkenler
    public AudioSource backgroundAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on " + gameObject.name);
        }

        // Arka plan sesini çal
        if (backgroundAudioSource != null && backgroundAudioSource.clip != null)
        {
            backgroundAudioSource.loop = true;
            backgroundAudioSource.Play();
        }

        // Karakterin pozisyonunu yükle
        LoadPosition();
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        bool isMoving = moveInput != Vector2.zero;

        if (isMoving) // Karakter hareket ediyorsa
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else if (wasMoving) // Karakter durduysa ve önceki durumda hareket ediyorduysa
        {
            SavePosition(); // Pozisyonu kaydet
        }

        wasMoving = isMoving; // Hareket durumunu güncelle

        // Karakter durduğunda stepTimer'ı sıfırla
        if (!isMoving)
        {
            stepTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogError("Footstep clips are not assigned or empty in the inspector.");
            return;
        }

        if (audioSource != null)
        {
            int index = Random.Range(0, footstepClips.Length);
            audioSource.PlayOneShot(footstepClips[index]);
        }
    }

    // Karakter pozisyonunu kaydetme
    public void SavePosition()
    {
        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        PlayerPrefs.Save();
        Debug.Log("Position saved: " + transform.position);
    }

    // Karakter pozisyonunu yükleme
    private void LoadPosition()
    {
        if (PlayerPrefs.HasKey("PlayerX") && PlayerPrefs.HasKey("PlayerY"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            transform.position = new Vector2(x, y);
            Debug.Log("Position loaded: " + transform.position);
        }
    }

    void OnApplicationQuit()
    {
        SavePosition();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePosition();
        }
    }
}
