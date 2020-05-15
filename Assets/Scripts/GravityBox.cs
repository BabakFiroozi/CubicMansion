using System;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class GravityBox : MonoBehaviour
    {
        Organ _organ;

        void Start()
        {
            _organ = gameObject.GetComponent<Organ>();
            _organ.DamageEvent += OnDamage;
        }

        void OnDamage(GameObject prej, float dmg, Vector3 pos, Vector3 norm)
        {
            var projectile  = prej.GetComponent<Projectile>();
            
            if(projectile == null)
                return;

            if (projectile.WeaponType != WeaponTypes.Graviter)
                return;

            var unit = projectile.SourceUnit;
            
            if(unit != PlayerCharacter.Instance.Movement.Unit)
                return;

            float dist = (PlayerCharacter.Instance.Movement.Tr.position - pos).magnitude;
            Coordinate.Instance.Change(norm, dist);
        }
    }
}