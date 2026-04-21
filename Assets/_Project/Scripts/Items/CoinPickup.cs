using UnityEngine;
using System;

namespace GhostBeam.Items
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private int coinAmount = 1;
        [SerializeField] private float attractionDistance = 3f;

        private Transform player;
        private bool isCollected = false;

        public int CoinAmount => coinAmount;

        private void Start()
        {
            player = FindAnyObjectByType<Player.LunaController>()?.transform;
        }

        private void Update()
        {
            if (player == null || isCollected)
                return;

            // Atração magnética
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer < attractionDistance)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    15f * Time.deltaTime
                );
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Collect();
            }
        }

        public void Collect()
        {
            if (isCollected)
                return;

            isCollected = true;
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            isCollected = false;
            gameObject.SetActive(true);
        }
    }
}
