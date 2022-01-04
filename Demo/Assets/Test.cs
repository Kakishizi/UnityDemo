using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 jumpTrush;
    private Rigidbody rig;
    private Animator anim;
    bool isJump=false;
    void Start()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetInteger("jump", 1);
            isJump = true;
        }
        print(rig.velocity.y);
        anim.SetFloat("Blend", rig.velocity.y);
    }
    private void FixedUpdate()
    {
        if (isJump)
        {
            jumpTrush = new Vector3(0, 4.0f, 0);
            rig.velocity = new Vector3(transform.position.x, rig.velocity.y, transform.position.z) + jumpTrush;
            jumpTrush = Vector3.zero;
            isJump = false;
        }
    }

}
