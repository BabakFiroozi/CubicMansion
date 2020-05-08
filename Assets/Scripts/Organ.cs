using System;
using UnityEngine;

namespace CubicMansion
{
    public class Organ : MonoBehaviour
    {
        [SerializeField] float _minHealth = 0;
        [SerializeField] float _maxHealth = 100;

        public Action OnDieEvent { get; set; }
        public Action<float, Vector3, GameObject> OnDamageEvent { get; set; }

        public float Health { get; private set; }

        void Start()
        {
            Health = _maxHealth;
        }

        public void DoDamage(float damage, Vector3 pos, GameObject prejudicial)
        {
            Health -= damage;

            OnDamageEvent?.Invoke(damage, pos, prejudicial);
            
            if(Health < _minHealth)
                Die();
        }

        void Die()
        {
            OnDieEvent?.Invoke();

            Destroy(gameObject);
        }

        public void DoDie()
        {
            Die();
        }
    }
}