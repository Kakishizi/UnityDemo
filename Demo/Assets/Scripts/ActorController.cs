using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{

    [Space(10)]
    [Header("===== Friction Settings =====")]
    public PhysicMaterial firctionOne;
    public PhysicMaterial frictionZero;

    public GameObject player;
    public CameraController camcon;
    public float walkSpeed = 2.0f;
    public float runSpeed = 2.5f;
    public float jumpVelocity = 1.0f;
    public float backVelocity = 5.0f;

    private bool lockPlanar = false;
    private bool lockRotate = false;
    private bool trackDirection = false;
    private bool canAttack;
    private bool canDodge;
    private Vector3 playerVec;
    private Vector3 thrustVec;
    private Vector3 deltaPos;
    private float lerpTarget;
    private float targetRunMulti = 1;
    private Rigidbody rig;
    private PlayerInput pi;
    private Animator anim;
    private CapsuleCollider col;

    private void Awake()
    {
        player = transform.Find("Reno").gameObject;
        rig = GetComponent<Rigidbody>();
        anim = player.GetComponent<Animator>();
        pi = GetComponent<PlayerInput>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {   
        //视角锁定
        if (pi.lockon)
        {
            camcon.LockOrUnlock();
        }
        //自由视角和锁定视角移动
        if (camcon.lockState == false)
        {
            targetRunMulti = Mathf.Lerp(targetRunMulti, pi.run ? 4.0f : 1.0f, 0.05f);
            anim.SetFloat("forward", pi.magnitude * targetRunMulti);
            anim.SetFloat("right", 0);
        }
        else
        {
            Vector3 localDvec = transform.InverseTransformVector(pi.dirVec);
            anim.SetFloat("forward", localDvec.z * ((pi.run) ? 4.0f : 1.0f));
            anim.SetFloat("right", localDvec.x * ((pi.run) ? 4.0f : 1.0f));
        }
        //落地翻滚
        if (rig.velocity.magnitude > 7.2f)
        {
            anim.SetTrigger("roll");
        }

        if (pi.jump)
        {
            anim.SetTrigger("Jump");
            canAttack = false;
        }

        //角色转向和移动
        //if (pi.magnitude > 0.1f && !lockRotate)
        //{
        //    float rotateTime = 0.5f;
        //    float time = 0;
        //    time += Time.deltaTime / rotateTime;
        //    player.transform.forward = Vector3.Slerp(player.transform.forward, pi.dirVec, time);//转向
        //}
        //if (lockPlanar == false)
        //{
        //    playerVec = pi.dirVec * walkSpeed * ((pi.run) ? runSpeed : 1.0f);//移动量储存
        //}

        if (pi.dodge)
        {
            anim.SetFloat("hDodge", pi.hDodge);
            anim.SetFloat("vDodge", pi.vDodge);
            anim.SetTrigger("Dodge");
            canDodge = true;
        }
        if (pi.attack && CheckState("OnGround") && canAttack)
        {
            anim.SetTrigger("attack");
        }
        if (camcon.lockState == false)
        {
            if (pi.magnitude > 0.1f && !lockRotate)
            {
                float rotateTime = 0.5f;
                float time = 0;
                time += Time.deltaTime / rotateTime;
                player.transform.forward = Vector3.Slerp(player.transform.forward, pi.dirVec, time);//转向
            }
        }
        else
        {
            if (trackDirection == false)
            {
                player.transform.forward = transform.forward;
            }
            else
            {
                player.transform.forward = (pi.magnitude * player.transform.forward).normalized;
            }
        }
    }
    private void FixedUpdate()
    {
        //if (camcon.lockState == false || anim.GetLayerWeight(1) >= 0.9f || canDodge)
        //{
        rig.position += deltaPos;
        rig.velocity = new Vector3(0, rig.velocity.y, 0) + thrustVec;
        //}
        //if (camcon.lockState == true)
        //{
        //    rig.velocity = new Vector3(playerVec.x, rig.velocity.y, playerVec.z) + thrustVec;
        //}
        canDodge = false;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }
    private bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }
    /// <summary>
    /// Message Processing block
    /// </summary>
    public void OnJumpEnter()
    {
        StartCoroutine(StartJump());
    }
    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }
    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnable = true;
        lockPlanar = false;
        canAttack = true;
        trackDirection = false;
        col.material = firctionOne;
    }
    public void OnGroundExit()
    {
        col.material = frictionZero;
    }
    public void OnDodgeEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
        lockRotate = true;
        trackDirection = true;
    }
    public void OnDodgeUpdate()
    {
        thrustVec = player.transform.forward * backVelocity * pi.vDodge + player.transform.right * backVelocity * pi.hDodge;
    }
    public void OnDodgeExit()
    {
        pi.inputEnable = true;
        lockPlanar = false;
        lockRotate = false;
    }
    public void OnAttack1hAEnter()
    {
        pi.inputEnable = false;
        lerpTarget = 1.0f;

    }

    public void OnAttack1hAUpdate()
    {
        //thrustVec = player.transform.forward * anim.GetFloat("attack1hAVelocity");
        //float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("Attack"));
        //currentWeight = Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.1f);
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.01f));
    }
    public void OnAttack1hCEnter()
    {
        pi.inputEnable = false;
    }
    //public void OnAttack1hCUpdate()
    //{
    //    //thrustVec = player.transform.up * anim.GetFloat("attack1hCVelocity");
    //    print(thrustVec);
    //}
    public void OnAttackIdleEnter()
    {
        pi.inputEnable = true;
        //lockPlanar =false;
        lerpTarget = 0;
    }
    public void OnAttackIdleUpdate()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.01f));
    }

    public void OnUpdateRM(object _deltaPos)
    {
        deltaPos += (Vector3)_deltaPos;
    }
    IEnumerator StartJump()
    {
        yield return new WaitForSeconds(0.12f);
        pi.inputEnable = false;
        lockPlanar = true;
        thrustVec = new Vector3(0, jumpVelocity, 0);
    }
}
