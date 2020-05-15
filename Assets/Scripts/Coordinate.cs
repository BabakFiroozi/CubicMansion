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


        public VecTypes CalcVecType(Vector3 vec)
        {
            VecTypes vecType = VecTypes.UP;
            const float limit = 5;
            if (Vector3.Angle(vec, Vector3.up) < limit)
                vecType = VecTypes.UP;
            if (Vector3.Angle(vec, Vector3.down) < limit)
                vecType = VecTypes.DOWN;
            if (Vector3.Angle(vec, Vector3.forward) < limit)
                vecType = VecTypes.FORWARD;
            if (Vector3.Angle(vec, Vector3.back) < limit)
                vecType = VecTypes.BACK;
            if (Vector3.Angle(vec, Vector3.right) < limit)
                vecType = VecTypes.RIGHT;
            if (Vector3.Angle(vec, Vector3.left) < limit)
                vecType = VecTypes.LEFT;
            return vecType;
        }

        public Vector3 CalcVector(VecTypes type)
        {
            Vector3 vec = Vector3.up;
            if(type == VecTypes.UP)
                vec = Vector3.up;
            if(type == VecTypes.DOWN)
                vec = Vector3.down;
            if(type == VecTypes.FORWARD)
                vec = Vector3.forward;
            if(type == VecTypes.BACK)
                vec = Vector3.back;
            if(type == VecTypes.RIGHT)
                vec = Vector3.right;
            if(type == VecTypes.LEFT)
                vec = Vector3.left;
            return vec;
        }
        
        public void Change(Vector3 up, float distance)
        {
            if (PlayerCharacter.Instance.Movement.CoordChanging)
                return;
            
            print("<color=yellow>Gravity Changed...</color>");

            VecTypes vecType = CalcVecType(up);

            if(vecType == UpVecType)
                return;
            
            UpVecType = vecType;

            Vector3 newUp = CalcVector(UpVecType);

            Vector3 forward = PlayerCharacter.Instance.Movement.TurnDirection;

            float diffAngle = -Vector3.SignedAngle(forward, -newUp, UpVec);
            // print("diff: " + diffAngle);

            Opposite = Vector3.Angle(UpVec, newUp) > 175;
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
            
            Vector3 vec = Quaternion.AngleAxis(diffAngle, UpVec) * ForwardVec;
            PlayerCharacter.Instance.Movement.ChangeCoord(vec);
            PlayerCharacter.Instance.Movement.SetTurnDirection(vec);

            Physics.gravity = DownVec * _gravityAmount;
        }
        
        public bool Opposite { get; private set; }
        public float Distance { get; private set; }
        
    }
}