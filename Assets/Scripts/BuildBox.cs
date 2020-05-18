using UnityEngine;

namespace CubicMansion
{
    [RequireComponent(typeof(Organ))]
    public class BuildBox : MonoBehaviour
    {
        [SerializeField] GameObject _buildPrefab;

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
            
            if (projectile.SourceUnit != PlayerCharacter.Instance.Movement.Unit)
                return;

            if (projectile.ProjectileType == ProjectileTypes.BuilderRemove)
            {
                PlayerCharacter.Instance.IncreaseBuildBox(_buildPrefab);
                RemoveBuild();
            }
            
            if (projectile.ProjectileType == ProjectileTypes.BuilderAdd)
            {
                if (PlayerCharacter.Instance.BuildBoxPrefabsCount > 0)
                {
                    CreateBuild(transform, pos, PlayerCharacter.Instance.CurrentBuildBoxPrefab);
                    PlayerCharacter.Instance.DecreaseBuildBox();
                }
            }
        }

        void RemoveBuild()
        {
            Destroy(gameObject);
        }
        
        public static GameObject CreateBuild(Transform tr, Vector3 pos, GameObject prefab)
        {
            Vector3 center = tr.position;
            Vector3 scl = tr.localScale;
            var sides = new Vector3[6];
            sides[0] = center + Vector3.forward * scl.z;
            sides[1] = center + Vector3.back * scl.z;
            sides[2] = center + Vector3.up * scl.y;
            sides[3] = center + Vector3.down * scl.y;
            sides[4] = center + Vector3.right * scl.x;
            sides[5] = center + Vector3.left * scl.x;

            float minsDist = 1000;
            Vector3 nearPos = center;
            foreach (var s in sides)
            {
                float dist = (s - pos).magnitude;
                if (dist < minsDist)
                {
                    minsDist = dist;
                    nearPos = s;
                }
            }

            var obj = Instantiate(prefab, nearPos, tr.rotation, tr.parent);
            return obj;
        }
    }
}