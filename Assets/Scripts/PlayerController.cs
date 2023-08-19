using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed=0.3f;
    private float thrusterFuelAmount = 1f;
    public float GetThrusterFuelAmount ()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring settings:")]
    [SerializeField]
    private JointProjectionMode jointMode = JointProjectionMode.PositionAndRotation;
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start ()
    {
        motor = GetComponent<PlayerMotor>();
        //Cica daca zicem aia cu require comp mai sus putem sa getcomp fara nicio grija
        //Nush dc dar ok o sa fac
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        SetJointSettings(jointSpring);
    }

    void Update ()
    {
        if(PauseMenu.IsOn)
        {
            if(Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);

            return;
        }
        
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            
        }

        //Setting target position for spring
        //This makes the physics act right when it comes to
        //Some object above ground level
        RaycastHit _hit;
        if(Physics.Raycast(transform.position, Vector3.down, out _hit, 100f,environmentMask))
        {
            joint.targetPosition = new Vector3 (0f, -_hit.point.y+1f , 0f);
        }
        else
        {
            joint.targetPosition = new Vector3 (0f, 1f , 0f);
        }
        
        
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");
        //Obtinem -1 sau 1 pe axe in functie de ce ap pe tastatura
        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;
        //Transformam vectorii cu -1 sau 1 (prob folosim transf ca sa bagam float in vec)
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;
        //Calculam velocitatea pe baza vectorilor pe care i-am facut si inm cu speed

        //Animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        motor.Move(_velocity);
        //Folosim o functie ca sa transmitem velocitatea aflata

        float _yRot = Input.GetAxisRaw("Mouse X");
        //E pe Y rotatia doar pt ca nu vrem sa ne uitam in sus ca se fute miscarea
        
        Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * lookSensitivity;
        motor.Rotate(_rotation);

        float _xRot = Input.GetAxisRaw("Mouse Y");
        //Acum calculam rotatia pentru X
        float _cameraRotationX = _xRot * lookSensitivity;
        motor.RotateCamera(_cameraRotationX);

        //calculam thruster force
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump") && thrusterFuelAmount>0f)
        {
            thrusterFuelAmount-=thrusterFuelBurnSpeed * Time.deltaTime;
            if(thrusterFuelAmount >= 0.01f)
            {
            _thrusterForce= Vector3.up * thrusterForce;
            SetJointSettings(0f);
            }
        } else 
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }
        thrusterFuelAmount =Mathf.Clamp(thrusterFuelAmount,0f,1f);
        //aplicam thruster force
        motor.ApplyThruster(_thrusterForce);

    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive= new JointDrive{  
        positionSpring = _jointSpring, 
        maximumForce = jointMaxForce
        };///Aici s-ar putea sa fie o problema ca nu am schimbat modul sau cv de genu dar ma rog sper ca merge
    }
}
