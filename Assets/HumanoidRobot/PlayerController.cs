using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float Speed = 5.0f;
    [SerializeField] private float runSpeed = 10.0f;
    private float curentSpeed = 0.0f;
    Vector3 moveVec;

    //-----ジャンプ-----
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private LayerMask groundLayer;
    bool isGround = false;

    //-----視点移動-----
    private Camera cam;
    private float rotSpeed = 0.1f;
    float maxPitch = 40f;
    float pitch = 0f;

    private Vector2 inputVer, inputlook;

    Rigidbody rb;

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVer = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            curentSpeed = runSpeed;
        }
        if (context.canceled)
        {
            curentSpeed = Speed;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isGround = false;
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        inputlook = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        curentSpeed = Speed;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Move();
        CamereMove();
    }

    void Move()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveVec = Vector3.Scale(forward, new Vector3(1, 0, 1));

        moveVec = (forward * inputVer.y + right * inputVer.x) * curentSpeed * Time.deltaTime;

        rb.MovePosition(rb.position + moveVec);
    }

    void CamereMove()
    {
        float mouseX = inputlook.x * rotSpeed;
        float mouseY = inputlook.y * rotSpeed;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        cam.transform.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);

        transform.Rotate(Vector3.up * mouseX);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGround = true; // 地面に着地したのでフラグをON
        }
    }
}
