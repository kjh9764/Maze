using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;

    [Header("�ִϸ��̼�")]
    public Animator animator;

    private Rigidbody rb;
    public Camera playerCamera;
    public Camera miniMapCamera;
    private float xRotation = 0f; // ī�޶� ���� ȸ���� ���� ����

    public bool gameStartChk = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody�� ȸ���� ���������� �������� �ʵ��� ����
        rb.freezeRotation = true;

        // �÷��̾� �ڽ��̳� ���� ī�޶� ��� (�ʿ信 ���� ����)
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

    // ���콺 �Է¿� ���� �þ�(ī�޶�) ȸ�� ó��
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // �÷��̾� ������ ���� ȸ����, ī�޶�� ���� ȸ��
        transform.Rotate(Vector3.up * mouseX);
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    // WASD �Է¿� ���� �̵� ó��
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // �÷��̾��� ��/��, ��/�� ���� ���� ���
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * moveSpeed;

        // ���� y�� �ӵ�(�߷��̳� ���� ����)�� �����ϸ鼭 x, z�ุ ������Ʈ
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        // �ִϸ�����: �̵� ���ο� ���� IsMoving bool�� ������Ʈ
        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }

}