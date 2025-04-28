using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;

    [Header("애니메이션")]
    public Animator animator;

    private Rigidbody rb;
    public Camera playerCamera;
    public Camera miniMapCamera;
    private float xRotation = 0f; // 카메라 상하 회전을 위한 변수

    public bool gameStartChk = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody가 회전을 물리적으로 제어하지 않도록 고정
        rb.freezeRotation = true;

        // 플레이어 자식이나 메인 카메라를 사용 (필요에 따라 수정)
        playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (gameStartChk)
        {
            HandleMouseLook();
            HandleMovement();
        }
    }

    // 마우스 입력에 따른 시야(카메라) 회전 처리
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 플레이어 몸통은 수평 회전만, 카메라는 상하 회전
        transform.Rotate(Vector3.up * mouseX);
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    // WASD 입력에 따른 이동 처리
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 플레이어의 앞/뒤, 좌/우 방향 벡터 계산
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * moveSpeed;

        // 현재 y축 속도(중력이나 점프 영향)를 유지하면서 x, z축만 업데이트
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        // 애니메이터: 이동 여부에 따라 IsMoving bool을 업데이트
        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }

}