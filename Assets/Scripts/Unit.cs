using System;
using System.Collections.Generic;
using UnityEngine;

namespace CubicMansion
{
    public enum UnitFactions
    {
        Good,
        Bad
    }
    
    [RequireComponent(typeof(Organ))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] UnitFactions _faction;

        [SerializeField] Transform _weaponTr; 
        
        public Organ Organ { get; set; }

        public Weapon CurrentWeapon { get; private set; }
        
        public List<Weapon> Weapons { get; } = new List<Weapon>();


        void Start()
        {
            Organ = gameObject.GetComponent<Organ>();
        }

        public void AddWeapon(Weapon weapon)
        {
            Weapons.Add(weapon);
            var tr = weapon.transform;
            tr.parent = _weaponTr;
            tr.localPosition = Vector3.zero;
            weapon.SetOwner(this);
            SetCurrentWeapon(weapon);
        }

        void SetCurrentWeapon(Weapon weapon)
        {
            if (CurrentWeapon != null)
                CurrentWeapon.gameObject.SetActive(false);
            CurrentWeapon = weapon;
            CurrentWeapon.gameObject.SetActive(true);
        }
        
        public void ChangeWeapon(int num)
        {
            var weapon = Weapons[num - 1];
            SetCurrentWeapon(weapon);
        }
    }
}