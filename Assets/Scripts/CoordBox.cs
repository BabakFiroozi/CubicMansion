using System;
using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class CoordBox : MonoBehaviour
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
            
            if(projectile.SourceUnit != PlayerCharacter.Instance.Movement.Unit)
                return;

            if (projectile.ProjectileType == ProjectileTypes.Graviter)
            {
                float dist = (PlayerCharacter.Instance.Movement.Tr.position - pos).magnitude;
                Coordinate.Instance.Change(norm, dist);
            }

            if (projectile.ProjectileType == ProjectileTypes.BuilderAdd)
            {
                if (PlayerCharacter.Instance.BuildBoxPrefabsCount > 0)
                {
                    var buildPrefab = Levels.Instance.GetBuildBox(PlayerCharacter.Instance.CurrentBuildBoxType);
                    BuildBox.CreateBuild(transform, pos, buildPrefab);
                    PlayerCharacter.Instance.DecreaseBuildBox(buildPrefab);
                }
            }
        }
    }
}