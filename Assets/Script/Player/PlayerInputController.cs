using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerStateManager stateManager;
    private PlayerMove move;

    private void Awake()
    {
        stateManager = GetComponent<PlayerStateManager>();
        move = GetComponent<PlayerMove>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVeer = context.ReadValue<Vector2>();

        //ステート変更のための入力受け取り
        stateManager.UpdateMoveState(inputVeer);
        //移動処理のための入力受け取り
        move.SetMoveInput(inputVeer);
    }

    public void OnAtatck(InputAction.CallbackContext context)
    {
        if (context.performed)
        {   
            //チャージ開始,ステート変更
            stateManager.SetActionState(ActionState.Charge);
        }
        if (context.canceled)
        {
            //チャージを止め攻撃,ステート変更
            stateManager.SetActionState(ActionState.Attack);
        }
    }
}
