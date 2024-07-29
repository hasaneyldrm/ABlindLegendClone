using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public AudioClip[] rightAttackClips; // Sağ saldırı sesleri
    public AudioClip[] leftAttackClips; // Sol saldırı sesleri
    public float attackInterval = 3f; // Saldırılar arasındaki bekleme süresi
    public float reactionTime = 1f; // Kullanıcının tepki vermesi için gereken süre

    private AudioSource audioSource;
    private bool isAttacking = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on " + gameObject.name);
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Rastgele bir sağ veya sol saldırı sesi seç
            AudioClip attackClip = Random.Range(0, 2) == 0 ? leftAttackClips[Random.Range(0, leftAttackClips.Length)] : rightAttackClips[Random.Range(0, rightAttackClips.Length)];
            float panStereo = attackClip == leftAttackClips[0] ? -0.8f : 0.8f;

            // Sesi çal ve kullanıcının tepki vermesi için bekle
            PlayAttackSound(attackClip, panStereo);
            yield return new WaitForSeconds(reactionTime);

            // Kullanıcıya saldırı yap
            isAttacking = true;
            yield return new WaitForSeconds(attackInterval);
            isAttacking = false;
        }
    }

    private void PlayAttackSound(AudioClip clip, float panStereo)
    {
        audioSource.panStereo = panStereo;
        audioSource.PlayOneShot(clip);
    }

    void Update()
    {
        if (isAttacking)
        {
            // Kullanıcının girişlerini kontrol et ve saldırıdan kaçınmasını sağla
            if (Input.GetKeyDown(KeyCode.LeftArrow) && audioSource.panStereo == -0.8f)
            {
                Debug.Log("Saldırıdan kaçınıldı (Sol)!");
                isAttacking = false;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && audioSource.panStereo == 0.8f)
            {
                Debug.Log("Saldırıdan kaçınıldı (Sağ)!");
                isAttacking = false;
            }
            else
            {
                Debug.Log("Saldırıya yakalandı!");
            }
        }
    }
}
