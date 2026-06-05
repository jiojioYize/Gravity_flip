using GravityFlip.Core;

using GravityFlip.Player;

using UnityEngine;



namespace GravityFlip.Level

{

    [RequireComponent(typeof(Collider2D))]

    public sealed class KillZone : MonoBehaviour

    {

        [SerializeField] private GameManager gameManager;



        private void Awake()

        {

            if (gameManager == null)

            {

                gameManager = FindObjectOfType<GameManager>();

            }



            GetComponent<Collider2D>().isTrigger = true;

        }



        private void OnTriggerEnter2D(Collider2D other)

        {

            if (other.GetComponent<PlayerController2D>() == null)

            {

                return;

            }



            gameManager?.RespawnPlayer();

        }

    }

}


