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

        public Weapon Weapon => _weapon;

        void Start()
        {
            _weapon.SetOwner(this);
        }

        public void SetWeapon(Weapon weapon)
        {
            _weapon = weapon;
        }
    }
}