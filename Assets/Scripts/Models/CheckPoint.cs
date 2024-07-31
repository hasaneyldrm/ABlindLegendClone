using UnityEngine;

namespace Models
{
    public class Checkpoint : MonoBehaviour
    {
        public AudioClip customSound; // Bu checkpoint için özel bir ses (opsiyonel)

        public void ActivateCheckpoint()
        {
            // Checkpoint aktif olduğunda yapılacak işlemler
            gameObject.SetActive(true);
        }

        public void DeactivateCheckpoint()
        {
            // Checkpoint devre dışı olduğunda yapılacak işlemler
            gameObject.SetActive(false);
        }
    }
}
