using System;
using System.Collections;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField] WeaponTypes _weaponType;
        [SerializeField] WeaponModeInfo[] _modeInfos;
        [SerializeField] Transform _fireTr;

        
        public WeaponTypes WeaponType => _weaponType;
        
        public Unit Owner { get; private set; }

        public Action EquipEvent { get; set; }
        
        public Action<Projectile> FireEvent { get; private set; }

        public bool IsReady { get; private set; } = true;

        public int ActiveMode { get; private set; }
        

        void Start()
        {
        }

        public void SetOwner(Unit unit)
        {
            Owner = unit;
            EquipEvent?.Invoke();
        }

        public Coroutine TryToggleMode()
        {
            var c = StartCoroutine(ToggleModeCoroutine());
            return c;
        }

        IEnumerator ToggleModeCoroutine()
        {
            if(!IsReady)
                yield break;

            if (ActiveMode == 0)
                ActiveMode = 1;
            else if (ActiveMode == 1)
                ActiveMode = 0;
            
            //play animation and change weapon mode
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

            var mode = _modeInfos[ActiveMode];
            
            yield return new WaitForSeconds(mode.fireTime);

            Fire(mode.projectile);
            
            yield return new WaitForSeconds(mode.fireInterval);

            IsReady = true;
        }
        

        void Fire(GameObject projPrefab)
        {
            var obj = Instantiate(projPrefab, _fireTr.position, _fireTr.rotation);
            var proj = obj.GetComponent<Projectile>(); 
            proj.SetSourceUnit(Owner);

            FireEvent?.Invoke(proj);
        }
    }


    [Serializable]
    public class WeaponModeInfo
    {
        public float fireInterval;
        public GameObject projectile;
        public float fireTime;
        public int firesCount;
        public int expense;
    }
    
    
    public enum WeaponTypes
    {
        Graviter,
        BuilderRemove,
        BuilderAdd
    }
}