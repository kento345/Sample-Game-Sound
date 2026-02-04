using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMove : MonoBehaviour
{
    [Header("à⁄ìÆê›íË")]
    [SerializeField] private float Speed = 5.0f;
    [SerializeField] private float chargingSpeed = 2.0f;
    private float curentSpeed = 0f;
    Vector2 inputVer;

    Rigidbody rb;

    //-----ScriptéQè∆-----
    private PlayerStateManager stateManager;

    private void Awake()
    {
        stateManager = GetComponent<PlayerStateManager>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        curentSpeed = Speed;
    }

    public void SetMoveInput(Vector2 input)
    {
        inputVer = input;
    }

    // Update is called once per frame
    void Update()
    {
        if(stateManager.MoveState != MoveState.Walk) { return; }

        Vector3 move = new Vector3(inputVer.x, 0, inputVer.y) * curentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + move);
    }
}
