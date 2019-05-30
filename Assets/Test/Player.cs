using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MoveType { WASD, MouseClick}

public class Player : MonoBehaviour,ICameraFollowable
{
    public CameraFollow cameraFollow { get; set; }

    public MoveType moveType;

    public float moveSpeed = 10;
    public float jumpPower = 0.5f;
    private float rayLength = 100;
    private int floorMask;

    private Rigidbody body;
    private Animator anim;
    private NavMeshAgent nav;
    private Light light;
    private LineRenderer line;

    private Transform shotPos;

    private Vector3 movement = Vector3.zero;
    private Vector3 xzPlaneVec = new Vector3(1, 0, 1);

    private int shootTimerHandler = 1;
    private float lastFireTime;
    private float fireInterval = 0.1f;

    private bool isOnGround;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        light = GetComponentInChildren<Light>(true);
        line = GetComponentInChildren<LineRenderer>(true);
        floorMask = LayerMask.GetMask("Floor");

        shotPos = transform.Find("GunBarrelEnd");
    }

    private void Update()
    {
        if(moveType == MoveType.MouseClick)
        {
            Turing(false);
            if (Input.GetMouseButtonDown(1))
            {
                MouseClickMove();
            }
        }
    }

    void FixedUpdate()
    {
        if(moveType == MoveType.WASD)
        {
            Move();
            //Turing();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }
    
    void Shoot()
    {
        if (Time.realtimeSinceStartup - lastFireTime < fireInterval) return;

        lastFireTime = Time.realtimeSinceStartup;

        light.enabled = true;
        line.enabled = true;

        Transform camPos = Camera.main.transform;

        RaycastHit hit;

        if(Physics.Raycast(camPos.position, camPos.forward, out hit, 100))
        {
            NavTest target = hit.transform.GetComponent<NavTest>();

            if (target)
            {
                target.Die();
            }

            line.SetPosition(1, transform.InverseTransformPoint(hit.point));
        }
        else
        {
            line.SetPosition(1, transform.InverseTransformPoint(shotPos.position + shotPos.forward * 20));
        }

        Scheduler.instance.Schedule(fireInterval/2, false, TurnOffGunEffect);
    }

    void MouseClickMove()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit floortHit;

        if (Physics.Raycast(camRay, out floortHit, rayLength, floorMask))
        {
            nav.SetDestination(floortHit.point);

            Vector3 playerToMove = floortHit.point - transform.position;

            playerToMove.y = 0f;

            Quaternion newRotatation = Quaternion.LookRotation(playerToMove);

            transform.rotation = newRotatation;
        }
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Vector3 cam_forward = Vector3.Scale(Camera.main.transform.forward, xzPlaneVec).normalized;
        Vector3 cam_forward = cameraFollow.transform.forward;
        Vector3 cam_right = Camera.main.transform.right;
        movement = (cam_forward * v + cam_right * h).normalized;

        if (movement.magnitude > 0) anim.SetBool("move", true);
        else anim.SetBool("move",false);

        body.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);

        body.MoveRotation(Quaternion.LookRotation(cam_forward));
    }

    void Jump()
    {
        if (!isOnGround) return;
        Vector3 jump = Vector3.up * jumpPower;
        body.AddForce(jump,ForceMode.Impulse);
        isOnGround = false;
    }

    private void Turing(bool useRigidBody = true)
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit floortHit;

        if(Physics.Raycast(camRay,out floortHit, rayLength, floorMask))
        {
            Vector3 playerToMouse = floortHit.point - transform.position;

            playerToMouse.y = 0f;

            Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

            if (useRigidBody)
            {
                body.MoveRotation(newRotatation);
            }
            else
            {
                transform.rotation = newRotatation;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Stage")
        {
            isOnGround = true;
        }
    }

    void TurnOffGunEffect()
    {
        light.enabled = false;
        line.enabled = false;
    }
}
