using System;
using UnityEngine;

namespace CubicMansion
{
    public class Organ : MonoBehaviour
    {
        [SerializeField] float _minHealth = 0;
        [SerializeField] float _maxHealth = 100;

        public Action DieEvent { get; set; }
        public Action<GameObject, float, Vector3, Vector3> DamageEvent { get; set; }

        public float Health { get; private set; }

        void Start()
        {
            Health = _maxHealth;
        }

        public void DoDamage(GameObject prejudicial, float damage, Vector3 pos, Vector3 norm)
        {
            Health -= damage;

            DamageEvent?.Invoke(prejudicial, damage, pos, norm);
            
            if(Health < _minHealth)
                Die();
        }

        void Die()
        {
            DieEvent?.Invoke();

            Destroy(gameObject);
        }

        public void DoDie()
        {
            Die();
        }
    }
}