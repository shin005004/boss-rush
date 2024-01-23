using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : BossController
{
    protected int BossHP;


    protected override void Start()
    {
        base.Start();


    }


    private enum BossAction
    {
        Idle = 0,
        Move = 1,
        Attack = 2,
        JumpAttack = 3,
    }

    private int currentActionId = 0;
    private float currentActionTime = 0f;
    
    private void HandleBrain()
    {
        switch (currentActionId)
        {
            case 0:

                if (currentActionTime > 1f)
                {
                    currentActionTime = 1;
                }
                break;

            case 1:
                if (currentActionTime > 0f)
                {

                }
                break;
        }

    }
}


public class BossStatsSO
{
    public int BossMaxHP;

}