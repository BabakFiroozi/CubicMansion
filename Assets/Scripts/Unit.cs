using System;
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
        [SerializeField]  Weapon _weapon;

        public Organ Organ { get; set; }
        
        public Weapon Weapon => _weapon;

        void Start()
        {
            _weapon.SetOwner(this);
            Organ = gameObject.GetComponent<Organ>();
        }

        public void SetWeapon(Weapon weapon)
        {
            _weapon = weapon;
        }
    }
}