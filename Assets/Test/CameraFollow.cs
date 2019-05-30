using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTarget;

    private Transform rig;
    private Transform offsetTrans;
    private Transform camTrans;
    private Camera cam;

    public float smooth = 5;

    public float mouseSensitivity_X = 10;
    public float mouseSensitivity_Y = 2;

    public float offset = 1.5f;

    private float camDistance = 6;

    private float yaw_root;
    private float pitch_root;

    private Quaternion targetRotYaw_root;
    private Quaternion targerRotPitch_root;

    private Vector3 newOffsetPos;

    private Vector3 springArmEndPos;

    private float springArmLengthMin = 2;
    private float springArmLengthMax = 5;
    private float springArmLength;

    void Start()
    {
        rig = transform.Find("Rig");
        offsetTrans = rig.Find("OffsetPos");
        camTrans = offsetTrans.Find("Main Camera");
        cam = camTrans.GetComponent<Camera>();

        SetFollowTarget(followTarget);
        SetRigOffset();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2)) SetRigOffset();

        MoveCameraByMouse();

        transform.position = Vector3.Lerp(transform.position, followTarget.position, smooth * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotYaw_root, smooth * Time.deltaTime);
        rig.localRotation = Quaternion.Lerp(rig.localRotation, targerRotPitch_root, smooth * Time.deltaTime);

        offsetTrans.localPosition = Vector3.Lerp(offsetTrans.localPosition, newOffsetPos, smooth * Time.deltaTime);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 45, smooth * Time.deltaTime);

        SpringArm();
    }

    void SetFollowTarget(Transform target)
    {
        followTarget = target;

        targetRotYaw_root = Quaternion.LookRotation(followTarget.forward);

        yaw_root = targetRotYaw_root.eulerAngles.y;

        ICameraFollowable followable = followTarget.GetComponent<ICameraFollowable>();

        if (followable != null)
        {
            followable.cameraFollow = this;
        }
    }

    void MoveCameraByMouse()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float deltaYaw = mouseX * mouseSensitivity_X;
        float deltaPitch = -mouseY * mouseSensitivity_Y;

        SetRootYawAngle(yaw_root + deltaYaw);
        SetRootPitchAngle(pitch_root + deltaPitch);
    }

    void SetRigOffset()
    {
        offset = -offset;
        newOffsetPos = offset * Vector3.right;
    }

    void SetRootYawAngle(float yaw)
    {
        this.yaw_root = yaw;
        targetRotYaw_root = Quaternion.Euler(0, yaw_root, 0);
    }

    void SetRootPitchAngle(float pitch)
    {
        this.pitch_root = pitch;
        targerRotPitch_root = Quaternion.Euler(pitch_root, 0, 0);
    }

    void SpringArm()
    {
        RaycastHit hit;

        Vector3 lookDirection = followTarget.position - camTrans.position;
        Vector3 springArmDirection = (camTrans.position - offsetTrans.position).normalized;

        if(Physics.Raycast(followTarget.position, -lookDirection, out hit, springArmLengthMax))
        {
            springArmLength -= 5 * Time.deltaTime;
            springArmLength = Mathf.Max(springArmLength, springArmLengthMin);
        }
        else
        {
            springArmLength += 5 * Time.deltaTime;
            springArmLength = Mathf.Min(springArmLength, springArmLengthMax);
        }

        camTrans.position = offsetTrans.position + springArmDirection * springArmLength;

        Debug.DrawLine(offsetTrans.position, offsetTrans.position + springArmDirection * springArmLength, Color.green);
    }
}
