using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;
    private float applySpeed; // walk or run

    [SerializeField]
    private float jumpForce;

    // 상태 변수
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    // 앉았을 때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;

    // 카메라의 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라의 회전제한
    [SerializeField]
    private float cameraRotationLimit; 
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;

    void Start()
    {
        // theCamera = FindObjectOfType<Camera>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        CameraRotation();
        CharacterRotation();
    }

    private void TryCrouch()
    {
        // 토글식을 Crouch 구현
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        isCrouch = !isCrouch;

        if(isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        // theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
        StartCoroutine(CoCrouch());
    }

    // 부드러운 앉기 동작
    IEnumerator CoCrouch()
    {
        float posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(posY !=  applyCrouchPosY)
        {
            count++;
            posY = Mathf.Lerp(posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0f, posY, 0f);
            if(count > 15)
                break;
            yield return null; // 매프레임 단위로 실행
        }
        theCamera.transform.localPosition = new Vector3(0f, applyCrouchPosY, 0f);
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if(isCrouch)
            Crouch();
        myRigid.velocity = transform.up * jumpForce;
    }

    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        if (isCrouch)
            Crouch();

        isRun = true;
        applySpeed = runSpeed;
    }
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        // A D, -1 0 1
        float moveDirX = Input.GetAxisRaw("Horizontal");
        // W S
        float moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    private void CharacterRotation()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        // Quaternion
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));
    }

    private void CameraRotation()
    {
        // UP DOWN, 1 0 -1
        float xRotation =  Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}
