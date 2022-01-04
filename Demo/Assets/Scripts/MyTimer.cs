using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer
{
    public enum STATE
    {
        IDLE,
        RUN,
        FINISHED
    }
    public STATE state;
    public float duration = 1.0f;
    private float elaspedTime = 0;
    public void Tick ()
    {
        switch (state)
        {
            case STATE.IDLE:
                break;
            case STATE.RUN:
                elaspedTime += Time.deltaTime;
                if(elaspedTime >= duration)
                {
                    state = STATE.FINISHED;
                }
                break;
            case STATE.FINISHED:
                break;
        }
    }
    public void Go()
    {
        elaspedTime = 0; 
        state = STATE.RUN;
    }
}
