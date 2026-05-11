using UnityEngine;
using System;
using GhostBeam.Gameplay;
using GhostBeam.Utilities;

namespace GhostBeam.Items
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class BatteryPickup : MonoBehaviour
    {
        [SerializeField] private float rechargeAmount = 50f;
        [SerializeField] private float attractionDistance = 3f;

        private Transform player;
        private bool isCollected = false;

        public float RechargeAmount => rechargeAmount;

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
                    10f * Time.deltaTime
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
            var batterySystem = FindAnyObjectByType<BatterySystem>();
            if (batterySystem != null)
                batterySystem.Recharge(rechargeAmount);

            var pooled = GetComponent<PooledObject>();
            if (pooled != null)
                pooled.ReleaseToPool();
            else
                gameObject.SetActive(false);
        }

        public void Reset()
        {
            isCollected = false;
        }
    }
}
