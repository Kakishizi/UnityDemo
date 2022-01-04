using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraController : MonoBehaviour
{
    public PlayerInput pi;
    public float horizontalSpeed;
    public float verticalSpeed;
    public Image lockDot;
    public bool lockState;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject model;
    private GameObject camera;
    private Queue<Collider> enemyList;
    [SerializeField]
    private LockTarget lockTarget;
    private Vector3 cameraDampVelocity;
    private float tempEulerX;

    private void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;//最外层的父物体
        tempEulerX = cameraHandle.transform.eulerAngles.x;
        model = playerHandle.GetComponent<ActorController>().player;
        camera = Camera.main.gameObject;
        lockDot.enabled = false;
        lockState = false;
        enemyList = new Queue<Collider>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (lockTarget != null)
        {
            lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position +
                                                                            new Vector3(0, lockTarget.halfHeight, 0));
            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 8.0f)
            {
                lockTarget = null;
                lockDot.enabled = false;
                lockState = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (lockTarget == null)
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, pi.lookRight * horizontalSpeed * Time.deltaTime);

            tempEulerX -= pi.lookUp * verticalSpeed * Time.deltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -20, 40);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);
            model.transform.eulerAngles = tempModelEuler;
        }
        else
        {
            //lockDot.transform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position);
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
        }
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, 0.1f);
        camera.transform.LookAt(cameraHandle.transform.position);
    }
    public void LockOrUnlock()
    {
        //Vector3 modleOrigin1 = model.transform.position;
        //Vector3 modleOrigin2 = model.transform.position + new Vector3(0, 1, 0);
        Collider[] cols = EnemyBox();
        if (cols.Length == 0)
        {
            lockTarget = null;
            lockDot.enabled = false;
            lockState = false;
        }
        else
        {
            foreach (var col in cols)
            {
                enemyList.Enqueue(col);
                if (lockTarget != null && lockTarget.obj == col.gameObject)
                {
                    lockTarget = null;
                    lockDot.enabled = false;
                    lockState = false;
                    break;
                }
                lockTarget = new LockTarget(col.gameObject, col.bounds.extents.y);
                lockDot.enabled = true;
                lockState = true;
                break;
            }
        }
    }
    public void ChangeLock()
    {
        Collider[] cols = EnemyBox();
        GameObject currentTarget = lockTarget.obj;
        GameObject nextTarget;
        foreach (var col in cols)
        {
            enemyList.Enqueue(col);
        }
        if (pi.changeTarget && enemyList.Count > 0)
        {
            GameObject tem=enemyList.Dequeue().gameObject;
            if (tem == currentTarget)
            {
                enemyList.Enqueue(tem.GetComponent<Collider>());
            }
            else
            {

            }
        }
    }
    public Collider[] EnemyBox()
    {
        Vector3 boxCenter = model.transform.position + new Vector3(0, 1, 0) + model.transform.forward * 5.0f;
        return Physics.OverlapBox(boxCenter, new Vector3(1f, 1f, 5f), model.transform.rotation, LayerMask.GetMask("Enemy"));
    }

    private class LockTarget
    {
        public GameObject obj;
        public float halfHeight;
        public LockTarget() { }
        public LockTarget(GameObject _obj, float _halfHeight)
        {
            obj = _obj;
            halfHeight = _halfHeight;
        }
    }
}