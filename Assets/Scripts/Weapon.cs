using System;
using System.Collections;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField] WeaponTypes _weaponType;
        [SerializeField] GameObject _projectilePrefab;
        [SerializeField] float _fireTime;
        [SerializeField] float _fireInterval;
        [SerializeField] Transform _fireTr;
        
        public Unit Owner { get; private set; }

        public void SetOwner(Unit unit)
        {
            Owner = unit;
        }

        public bool IsReady { get; private set; } = true;
        
        public Action OnFire { get; private set; }
        

        void Start()
        {
        }

        public Coroutine TryFire()
        {
            var c = StartCoroutine(TryFireCoroutine());
            return c;
        }

        IEnumerator TryFireCoroutine()
        {
            if(!IsReady)
                yield break;

            IsReady = false;
            
            yield return new WaitForSeconds(_fireTime);
            
            Fire();
            
            yield return new WaitForSeconds(_fireInterval);

            IsReady = true;
        }
        

        void Fire()
        {
            var obj = Instantiate(_projectilePrefab, _fireTr.position, _fireTr.rotation);
            obj.GetComponent<Projectile>().SetSourceUnit(Owner);
            
            OnFire?.Invoke();
        }
    }
    
    
    public enum WeaponTypes
    {
        Ranged,
        Melee
    }
}