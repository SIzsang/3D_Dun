using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    public float dashPower;
    public float itemSpeed;
    public float itemDuration;
    private bool isDashing;
    public int wallSpeed;
    private Vector2 currentMoveInput; // Input Action에서 받아올 값들을 넣어줄 곳
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;



    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool CanLook = true;

    public Action inventory; // 델리게이트
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 Lock, 숨기기
    }
    private void Update()
    {
        if (isDashing)
        {
            float staminaPerSecond = 20f;
            bool canDash = true;

            if (PlayerEvents.Run != null)
            {
                canDash = PlayerEvents.Run.Invoke(staminaPerSecond * Time.deltaTime);
            }

            if (canDash)
            {
                dashPower = 5f;
            }
            else
            {
                isDashing = false;
                dashPower = 0f;
            }
        }
    }
    // 물리연산을 하는 곳은 FixedUpdate
    private void FixedUpdate()
    {
        if (IsWall()) // 벽이 감지되면
        {
            WallMove(); // 벽 위로 이동
        }
        else
        {
            Move();
        }
    }


    private void LateUpdate()
    {
        if (CanLook)
        {
            CameraLook();
        }
        
    }

    private void OnEnable()
    {
        PlayerEvents.ItemSpeedBoost += UseItemSpeed;
    }

    private void OnDisable()
    {
        PlayerEvents.ItemSpeedBoost -= UseItemSpeed;
    }

    void Move()
    {
        Vector3 dir = transform.forward * currentMoveInput.y + transform.right * currentMoveInput.x;
        dir *= moveSpeed + dashPower + itemSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    void UseItemSpeed(float amout, float duration)
    {
        itemSpeed = amout;
        itemDuration = duration;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity; // 마우스Y의 이동량 * 민감도 = X축의 회전값, 즉 상하 각도
        // 마우스 움직임은 Y값이지만 그것을 돌리려면 X값을 넣어야함
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // 회전값 제한
        cameraContainer.localRotation = Quaternion.Euler(-camCurXRot, 0f, 0f);

        transform.rotation *= Quaternion.Euler(0f, mouseDelta.x * lookSensitivity, 0f);

        //cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        //transform.eulerAngles = new Vector3(0, mouseDelta.x + lookSensitivity, 0);

        // 마우스의 움직임이 없다면 0 * 민감도 이므로 회전값이 없는 것
        // +가 들어갈 수 없는 이유는 매 프레임마다 고정된 회전값을 설정하기 때문.
        // 실제로 +를 사용하면 오른쪽으로 계속 회전한다.

        // 회전값
        // 회전값 제한
        // 실제 회전값을 넣어준 것 X
        // 실제 회전값을 넣어준 것 Y
    }
    private void WallMove()
    {
        Vector3 dir = transform.right * currentMoveInput.x + transform.up * currentMoveInput.y;
        dir *= wallSpeed;
        _rigidbody.velocity = dir;
    }

    private bool IsWall()
    {
        float radius = 0.4f;
        Vector3 center = transform.position + transform.up * 1.0f;
        Collider[] hits = Physics.OverlapSphere(center, radius, wallLayerMask);
        if (hits.Length > 0)
        {
            Debug.Log("벽 감지");
            return true;
        }
        return false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            currentMoveInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            currentMoveInput = Vector2.zero;
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isDashing = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isDashing = false;
            dashPower = 0f;
        }
    }
    

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + transform.up * 0.01f, Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + transform.up * 0.01f, Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 1f, groundLayerMask))
                // 캐릭터 position.y 값 0.5 >> Ray 0.6
            {
                return true;
            }
        }
        return false;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        CanLook = !toggle;
    }
}
