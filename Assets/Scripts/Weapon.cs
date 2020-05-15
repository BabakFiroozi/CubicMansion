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

        
        public WeaponTypes WeaponType => _weaponType;
        
        public Unit Owner { get; private set; }

        public Action EquipEvent { get; set; }
        public Action FireEvent { get; private set; }
        

        public bool IsReady { get; private set; } = true;
        

        void Start()
        {
        }

        public void SetOwner(Unit unit)
        {
            Owner = unit;
            EquipEvent?.Invoke();
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
            
            FireEvent?.Invoke();
        }
    }
    
    
    public enum WeaponTypes
    {
        Graviter,
        Builder
    }
}