using UnityEngine;

public class EnergySphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDrone drone = other.GetComponent<PlayerDrone>();
            if (drone != null)
            {
                drone.health = Mathf.Min(drone.health + 10, 100);
            }

            Destroy(gameObject);
        }
    }
}