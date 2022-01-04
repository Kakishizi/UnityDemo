using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float magnitude;
    public Vector3 dirVec;
    public Quaternion rotation;
    public float xView;
    public float yView;
    public float lookUp;
    public float lookRight;
    public float hDodge;
    public float vDodge;

    [Header("===== Key Setting =====")]
    public string keyA;
    public string keyB;
    public string keyC;
    public string keyD;
    public string keyTab;

    public string keyRight;
    public string keyLeft;
    public string keyUp;
    public string keyDown;

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonC = new MyButton();
    public MyButton buttonD = new MyButton();
    public MyButton buttonTab=new MyButton();
    public MyButton buttonUp = new MyButton();
    public MyButton buttonDown = new MyButton();
    public MyButton buttonLeft = new MyButton();


    public bool inputEnable;
    public bool run;
    public bool isOnGround;
    public bool jump;
    public bool dodge;
    public bool attack;
    public bool lockon;
    public bool changeTarget;

    public MyButton btnXXX = new MyButton();

    private float stopTime;
    private float longPressTriggerTime;

    [Header("===== Mouse Settings =====")]
    public bool mouseEnable = false;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;
    private bool isMove
    {
        get
        {
            return (horizontal != 0 || vertical != 0) ? true : false;
        }
    }
    void Update()
    {
        buttonA.Tick(Input.GetKey(keyA));//控制跑步，闪避
        buttonB.Tick(Input.GetKey(keyB));//跳跃
        buttonC.Tick(Input.GetKey(keyC));//攻击
        buttonD.Tick(Input.GetKey(keyD));//锁定
        buttonTab.Tick(Input.GetKey(keyTab));//切换

        if (mouseEnable == true)
        {
            lookUp = Input.GetAxis("Mouse Y") * mouseSensitivityY * 2;
            lookRight = Input.GetAxis("Mouse X") * mouseSensitivityX * 2;
        }
        else
        {
            lookUp = (Input.GetKey(keyUp) ? 1 : 0) - (Input.GetKey(keyDown) ? 1 : 0);
            lookRight = (Input.GetKey(keyRight) ? 1 : 0) - (Input.GetKey(keyLeft) ? 1 : 0);
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        hDodge = Input.GetAxisRaw("Horizontal");
        vDodge = Input.GetAxisRaw("Vertical");

        if (inputEnable == false)
        {
            horizontal = 0;
            vertical = 0;
        }
        Vector3 target = new Vector3(horizontal, 0, vertical);

        //奔跑
        //run = Input.GetKeyDown(KeyCode.R);
        //run = buttonA.HoldPressing;
        LeftShiftLongPress();
        //跳跃
        jump = buttonB.OnPressed;
        //闪避
        dodge = buttonA.OnPressed && buttonA.IsDelaying && (vDodge + hDodge != 0);
        //攻击
        attack = buttonC.OnPressed;
        //锁定目标
        lockon = buttonD.OnPressed;
        //切换目标
        changeTarget = buttonTab.OnPressed;
        //修正斜向速度
        Vector2 temAxis = SquareToCircle(new Vector2(horizontal, vertical));
        magnitude = Mathf.Sqrt(temAxis.x * temAxis.x + temAxis.y * temAxis.y);//求得向量幅度
        dirVec = temAxis.x * transform.right + temAxis.y * transform.forward;//方向
        //rotation = Quaternion.LookRotation(dirVec);
    }
    /// <summary>
    /// UV展开平面
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) * 0.5f);//x=x*根号下1-y^2*0.5
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) * 0.5f);//y=y*根号下1-x^2*0.5
        return output;
    }

    private void LeftShiftLongPress()
    {
        if (Input.GetKey(keyA) && isMove)
        {
            longPressTriggerTime += Time.deltaTime;
        }
        if (Input.GetKeyUp(keyA))
            longPressTriggerTime = 0;
        if (longPressTriggerTime >= 0.2f && isMove)
        {
            run = true;
            longPressTriggerTime = 0;
        }
        if (horizontal == 0 && vertical == 0 && isOnGround)
        {
            stopTime += Time.deltaTime;
            if (stopTime >= 1.2f)
            {
                run = false;
                stopTime = 0;
            }
        }
        if (horizontal != 0 || vertical != 0)
        {
            stopTime = 0;
        }
    }
}
