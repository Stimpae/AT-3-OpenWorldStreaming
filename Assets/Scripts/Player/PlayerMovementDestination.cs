using UnityEngine;

namespace Player
{
    public class PlayerMovementDestination : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Destroy(gameObject); 
            }
        }
    }
}
