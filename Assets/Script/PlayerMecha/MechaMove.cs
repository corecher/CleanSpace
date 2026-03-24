using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MechaMove : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    [Header("Movement")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Gravity & Jump")]
    public float gravity = -9.81f;
    public float jumpHeight = 1f;
    Vector3 velocity;
    bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    [Header("Teleport Skill")]
    public float teleportDistance = 10f; // 순간이동 거리 (10m)
    public float teleportCooldown = 1.5f; // 스킬 쿨타임
    public int maxTeleportCount = 2;
    public int currentTeleportCount = 2;
    [SerializeField]private float teleportCooldownTimer=0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        teleportCooldownTimer = teleportCooldown;
    }

    void Update()
    {
        // 1. 쿨타임 감소
        if (teleportCooldownTimer > 0&&currentTeleportCount < maxTeleportCount)
        {
            teleportCooldownTimer -= Time.deltaTime;
        }
        else if(teleportCooldownTimer <= 0&&currentTeleportCount < maxTeleportCount)
        {
            teleportCooldownTimer = teleportCooldown;
            currentTeleportCount++;
        }

        // 2. 쉬프트 키를 누르면 순간이동 실행
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentTeleportCount>0)
        {
            Teleport();
        }

        // 순간이동은 1프레임 만에 끝나므로 대쉬처럼 다른 움직임을 멈출 필요가 없습니다.
        GroundCheck();
        Move();
        Jump();
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void Move()
    {
        Vector3 direction = WASD();
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    Vector3 WASD()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector3(horizontal, 0f, vertical).normalized;
    }
    void Teleport()
    {
        Vector3 inputDir = WASD();

        Vector3 teleportDir;

        // 2. 방향키를 누르고 있다면 '누른 방향 + 카메라 시점' 계산, 안 누르고 있다면 '캐릭터 정면'
        if (inputDir.magnitude >= 0.1f)
        {
            // 이동 함수(Move)와 똑같이 카메라 기준의 방향을 구합니다.
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            teleportDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // 순간이동과 동시에 캐릭터가 해당 방향을 쳐다보게 즉시 회전시킵니다.
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        else
        {
            // 가만히 서 있을 때 쉬프트를 누르면 보고 있는 앞쪽으로 이동
            teleportDir = transform.forward;
        }

        // 3. 순간이동 실행!
        // CharacterController.Move를 사용하면 10m를 이동하더라도 중간에 벽이 있으면 뚫지 않고 벽 앞에 멈춰서 안전합니다.
        controller.Move(teleportDir.normalized * teleportDistance);

        currentTeleportCount--;
    }
}
