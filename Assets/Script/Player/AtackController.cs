using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEditorInternal;
using UnityEngine;

public class AtackController : MonoBehaviour
{
    [SerializeField] private float curentForce;
    private float duration = 0.5f;
    private float cooldown = 1.0f; //攻撃クールダウン
    public float lastTime = 0f;

    private float t = 0f;
    public float chargeMax = 5.0f;
    private bool isMax = false;
  /*  private bool isShot = false;
    private bool isStart =false;*/
    private bool isRigid = false;

    [Header("ノックバック,無敵設定")]
    [SerializeField] private float WeakKnockbackForce = 2.5f; //弱ノックバック
    [SerializeField] private float StrongKnockbackForce = 5.0f;//強ノックバック
    private float curentknockbackForce = 0f;//現在のノックバック力


    [Header("当たり判定設定")]
    [SerializeField] private SphereCollider searchArea;
    [SerializeField] private float angle = 45f;

    Rigidbody rb;
    PlayerStateManager stateManager;
   

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<PlayerStateManager>();
    }

    void Update()
    {
        if (stateManager.ActionState == ActionState.Charge)
        {
            if (t < chargeMax)
            {
                t += Time.deltaTime;
            }
            if (t > chargeMax)
            {
                isMax = true;
            }
        }
        else if (stateManager.ActionState == ActionState.Attack)
        {
            t = 0f;
        }
    }

    public void Shot(int x)
    {
        if (x == 1)
        {
            if(stateManager.ActionState == ActionState.Charge || stateManager.ActionState == ActionState.Attack) {return; }
            isRigid = false;

            if (stateManager.ActionState != ActionState.Attack && Time.time > lastTime + cooldown)
            {
                //チャージ開始,ステート変更
                stateManager.SetActionState(ActionState.Charge);
            }
        }
        if (x == 2)
        {
            if (stateManager.ActionState != ActionState.Attack && Time.time > lastTime + cooldown)
            {
                if (isRigid) { return; }
                //チャージを止め攻撃,ステート変更
                stateManager.SetActionState(ActionState.Attack);

                lastTime = Time.time;

                if (isMax)
                {
                    curentknockbackForce = StrongKnockbackForce;
                }
                else
                {
                    curentknockbackForce = WeakKnockbackForce;
                }
                rb.AddForce(transform.forward * curentForce, ForceMode.Impulse);
                Invoke("EndAttack", duration);
            }
        }  
    }

    void EndAttack()
    {
        rb.linearVelocity = Vector3.zero;
        if (isMax)
        {
            isRigid = true;
        }
        //ステートをNoneに
        stateManager.SetActionState(ActionState.None);
        isMax = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 posDir = other.transform.position - this.transform.position;
            float target_angle = Vector3.Angle(this.transform.forward, posDir);

            var dist = Vector3.Distance(other.transform.position, transform.position);

            if (target_angle > angle) { return; }

            if (target_angle <= angle)
            {
                if (Physics.Raycast(this.transform.position + Vector3.up * 0.5f, posDir, out RaycastHit hit))
                {
                    if (hit.collider == other)
                    {
                        Debug.Log("Hit");
                        if (stateManager.ActionState == ActionState.Attack)
                        {
                            Reception p = other.gameObject.GetComponent<Reception>();
                            //if (p.isHit) { return; }
                            p.KnockBack(rb.linearVelocity.normalized, curentknockbackForce);
                            Debug.Log("攻撃Hit");
                            //当たった時点でInvokeをキャンセルしてタックルを止める
                            CancelInvoke("EndAttack");
                            EndAttack();
                        }
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        pos.y = 1.0f;
        Handles.color = Color.red;
        Handles.DrawSolidArc(pos, Vector3.up, Quaternion.Euler(0.0f, -angle, 0f) * transform.forward, angle * 2f, searchArea.radius);
    }
#endif
}