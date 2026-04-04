using UnityEngine;

public class MechaTeleportSkill : MonoBehaviour
{
    public PlayerMove playerMove;
    public float teleportDistance = 10f; // 순간이동 거리 (10m)
    public float teleportCooldown = 1.5f; // 스킬 쿨타임
    public int maxTeleportCount = 2;
    public int currentTeleportCount = 2;
    [SerializeField]private float teleportCooldownTimer=0;
    void Start()
    {
        teleportCooldownTimer = teleportCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        TeleportCoolTime();
        if(Input.GetKeyDown(KeyCode.F)&&currentTeleportCount>0) Teleport();
    }
    void TeleportCoolTime()
    {
        if (teleportCooldownTimer > 0&&currentTeleportCount < maxTeleportCount)
        {
            teleportCooldownTimer -= Time.deltaTime;
        }
        else if(teleportCooldownTimer <= 0&&currentTeleportCount < maxTeleportCount)
        {
            teleportCooldownTimer = teleportCooldown;
            currentTeleportCount++;
        }
    }
    void Teleport()
    {
        Vector3 inputDir = playerMove.WASD();

        Vector3 teleportDir;

        // 2. 방향키를 누르고 있다면 '누른 방향 + 카메라 시점' 계산, 안 누르고 있다면 '캐릭터 정면'
        if (inputDir.magnitude >= 0.1f)
        {
            // 이동 함수(Move)와 똑같이 카메라 기준의 방향을 구합니다.
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + playerMove.cam.eulerAngles.y;
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
        playerMove.controller.Move(teleportDir.normalized * teleportDistance);

        currentTeleportCount--;
    }
}
