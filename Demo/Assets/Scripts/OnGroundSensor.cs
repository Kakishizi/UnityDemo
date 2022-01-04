using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capsule;
    public PlayerInput pi;
    private Vector3 foot;
    private Vector3 head;
    private float radius;
    private float offset = 0.1f;
    // Start is called before the first frame update
    void Awake()
    {
        radius = capsule.radius - 0.05f; ;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        foot = transform.position + transform.up * (radius - offset);
        head = transform.position + transform.up * (capsule.height - offset) - transform.up * radius;
        Collider[] outputCols = Physics.OverlapCapsule(foot, head, radius,LayerMask.GetMask("Ground"));
        if (outputCols.Length!=0)
        {
            pi.isOnGround = true;
            SendMessageUpwards("IsGround");
;        }
        else
        {
            pi.isOnGround = false;
            SendMessageUpwards("IsNotGround");
        }
    }
}
