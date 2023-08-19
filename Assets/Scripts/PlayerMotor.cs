using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    
    private Vector3 velocity=Vector3.zero;
    private Vector3 rotation=Vector3.zero;
    private float camerarotationX=0f;
    private float currentCameraRotationX=0f;
    private Rigidbody rb;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move (Vector3 _velocity)
    {
        velocity=_velocity;
    }
    public void Rotate (Vector3 _rotation)
    {
        rotation=_rotation;
    }
    public void RotateCamera(float _cameraRotationX)
    {
        camerarotationX=_cameraRotationX;
    }

    //Get a force vector for thruster
    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce=_thrusterForce;
    }

    void FixedUpdate ()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if(velocity!=Vector3.zero)
        {
            rb.MovePosition(rb.position+velocity*Time.fixedDeltaTime);
        }

        if(thrusterForce !=Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);//Ultima chestie e ca sa ignore masa
        }
    }
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if(cam!=null)
        {
            //Set our rotation and clamp it
            currentCameraRotationX -=camerarotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            //Apply rotation to cam;
            cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0f, 0f);
        }

    }
    
}
