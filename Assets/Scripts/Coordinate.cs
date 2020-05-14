using UnityEngine;

namespace CubicMansion
{
    
    public class Coordinate : MonoBehaviour
    {
        public static Coordinate Instance { get; private set; }

        [SerializeField] float _gravityAmount = 9.8f;

        public VecTypes UpVecType { get; private set; }
        
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Init()
        {
            ForwardVec = Vector3.forward;
            RightVec = Vector3.right;
            UpVec = Vector3.up;
            UpVecType = VecTypes.UP;
        }


        public Vector3 ForwardVec { get; private set; } 
        public Vector3 RightVec { get; private set; }
        public Vector3 UpVec { get; private set; }

        public Vector3 BackVec => -ForwardVec;
        public Vector3 LeftVec => -RightVec;
        public Vector3 DownVec => -UpVec;

        
        public void Change(VecTypes vecType, float distance)
        {
            print("<color=yellow>Gravity Changed...</color>");

            Vector3 forward = PlayerCharacter.Instance.Movement.TurnDirection;

            Vector3 newUp = UpVec;
            if(vecType == VecTypes.UP)
                newUp = Vector3.up;
            if(vecType == VecTypes.DOWN)
                newUp = Vector3.down;
            if(vecType == VecTypes.RIGHT)
                newUp = Vector3.right;
            if(vecType == VecTypes.LEFT)
                newUp = Vector3.left;
            if(vecType == VecTypes.FORWARD)
                newUp = Vector3.forward;
            if(vecType == VecTypes.BACK)
                newUp = Vector3.back;
            
            float diffAngle = -Vector3.SignedAngle(forward, -newUp, UpVec);
            // print("diff: " + diffAngle);

            Opposite = Vector3.Angle(UpVec, newUp) > 170;
            Distance = distance;
            
            if (Opposite)
            {
                ForwardVec = -forward;
                diffAngle = 0;
            }
            else
            {
                ForwardVec = UpVec;
            }
            
            UpVec = newUp;
            RightVec = Vector3.Cross(ForwardVec, UpVec);
            
            // tr.forward = forward;
            // tr.right = right;
            // tr.up = up;

            // Vector3 vec = Quaternion.AngleAxis(-diffAngle, UpVec) * forward;
            // PlayerCharacter.Instance.Movement.SetTurnDirection(vec);

            Vector3 vec = Quaternion.AngleAxis(diffAngle, UpVec) * ForwardVec;
            PlayerCharacter.Instance.Movement.ChangeCoord(vec);
            PlayerCharacter.Instance.Movement.SetTurnDirection(vec);

            Physics.gravity = DownVec * _gravityAmount;
        }
        
        public bool Opposite { get; private set; }
        public float Distance { get; private set; }
        
    }
}