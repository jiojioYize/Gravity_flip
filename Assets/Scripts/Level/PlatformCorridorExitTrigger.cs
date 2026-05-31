using UnityEngine;

namespace GravityFlip.Level
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlatformCorridorExitTrigger : MonoBehaviour
    {
        [SerializeField] private ShuttlePlatformController shuttleController;

        private void Awake()
        {
            if (shuttleController == null)
            {
                shuttleController = FindObjectOfType<ShuttlePlatformController>();
            }

            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<MovingPlatform2D>() == null)
            {
                return;
            }

            shuttleController?.NotifyCorridorExited();
        }
    }
}
