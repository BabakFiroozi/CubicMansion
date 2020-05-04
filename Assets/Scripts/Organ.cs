using System;
using UnityEngine;

namespace CubicMansion
{
    public class Organ : MonoBehaviour
    {
        [SerializeField] float _minHealth = 0;
        [SerializeField] float _maxHealth = 100;

        public Action OnDie { get; set; }
        public Action<float, Vector3, GameObject> OnDamage { get; set; }

        public float Health { get; private set; }

        void Start()
        {
            Health = _maxHealth;
        }

        public void DoDamage(float damage, Vector3 pos, GameObject prejudicial)
        {
            Health -= damage;

            OnDamage?.Invoke(damage, pos, prejudicial);
            
            if(Health < _minHealth)
                Die();
        }

        void Die()
        {
            OnDie?.Invoke();

            Destroy(gameObject);
        }

        public void DoDie()
        {
            Die();
        }
    }
}