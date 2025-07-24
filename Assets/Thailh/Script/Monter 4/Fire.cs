using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
    {
        public int damage = 5;
        public float lifeTime = 5f;

        void Start()
        {
            Destroy(gameObject, lifeTime); // Tự hủy sau 5 giây (tránh tồn tại vô hạn)
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);

                }

                Destroy(gameObject);
            }
        }
    

}

