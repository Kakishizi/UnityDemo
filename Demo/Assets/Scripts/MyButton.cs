using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool IsExtending = false;
    public bool IsDelaying = false;
    public bool HoldPressing = false;

    public float extendingDuration = 0.15f;
    public float delayingDuration = 0.2f;
    public float holdDelayingDuration = 0.5f;

    private bool curState = false;
    private bool lastState = false;

    private MyTimer extTimer = new MyTimer();
    private MyTimer delayTimer = new MyTimer();
    private MyTimer holdDelayTimer = new MyTimer();
    public void Tick(bool input)
    {
        extTimer.Tick();
        delayTimer.Tick();

        curState = input;
        IsPressing = curState;

        OnPressed = false;
        OnReleased = false;
        IsDelaying = false;
        if (curState != lastState)
        {
            if (curState == true)
            {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);
            }
            else
            {
                OnReleased = true;
                StartTimer(extTimer, extendingDuration);
            }
        }
        lastState = curState;

        IsExtending = (extTimer.state == MyTimer.STATE.RUN);
        IsDelaying = (delayTimer.state == MyTimer.STATE.RUN);
    }

    private void StartTimer(MyTimer timer, float duration)
    {
        timer.duration = duration;
        timer.Go();
    }
}
