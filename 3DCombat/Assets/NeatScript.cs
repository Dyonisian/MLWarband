using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpNeat.Phenomes;

public class NeatScript : UnitController
{
    [SerializeField]
    bool IsRunning;
    IBlackBox box;
    public EnemyScriptNeat MyEnemyScript;
    PlayerScript.State PState, LastPState;
    public int MyHits, MyAttacks, MyBlocks;
    public int OpponentHits, OpponentAttacks, OpponentBlocks;
    ISignalArray InputArr;
    ISignalArray OutputArr;
    int[] DataArray;
    EnemyScriptNeat.LearningStateNeat LS;

    void FixedUpdate()
    {
        if (IsRunning)
        {
            //Check hit - Done by EnemyScriptNeat
            //Check cooldown
            //If not on cooldown, take action

            
            if (PState != PlayerScript.State.Hit && MyEnemyScript.ActionCooldown <= 0.0f)
            {
                LS = MyEnemyScript.BuildLearningState();
                PState = LS.PState;
                UpdateArray();
                box.Activate();
                OutputArr = box.OutputSignalArray;

                MyEnemyScript.UpdateForNeat(FindMaxActivation());

                LastPState = PState;

            }
        }
    }
    public override void Stop()
    {
        this.IsRunning = false;
    }

    public override void Activate(IBlackBox box)
    {
        this.box = box;
        this.IsRunning = true;
        InitArray();
    }

    public override float GetFitness()
    {
        // Implement a meaningful fitness function here, for each unit.
        float fit, dodge, miss, runHitRatio, myHitsNew, modifier, modulus, myDodgeNew, myHitsCopy, myDodgeCopy ;
        myHitsNew = 0;
        myDodgeNew =0;
        modifier = 1.0f;
        modulus = 5;
        OpponentAttacks = MyEnemyScript.OpponentAttacks;
        MyAttacks = MyEnemyScript.MyAttacks;
        OpponentHits = MyEnemyScript.OpponentHits;
        MyHits = MyEnemyScript.MySword.MyHits;
        OpponentBlocks = MyEnemyScript.MySword.OpponentBlocks;
        MyBlocks = MyEnemyScript.OpponentScript.MySword.MyBlocks;
        if (MyHits == 0)
            return 0;

        myHitsCopy = MyHits;
        

        while((float)(myHitsCopy / 2.0f)>0)
        {
            modulus = myHitsCopy % 2;
            if (modulus == 0)
                modulus = 2;
            myHitsNew += modulus * modifier;
            modifier *= 0.5f;
            myHitsCopy -= 2;
        }

        

        if (OpponentAttacks == 0)
            OpponentAttacks = 1;
        if (OpponentHits == 0)
            OpponentHits = 1;
        dodge = OpponentAttacks - OpponentHits;

        if (dodge == 0)
            dodge = 1;

        myDodgeCopy = dodge * 0.5f;
        modifier = 1.0f;
        while ((float)(myDodgeCopy / 2.0f) > 0)
        {
            modulus = myDodgeCopy % 2;
            if (modulus == 0)
                modulus = 2;
            myDodgeNew += modulus * modifier;
            modifier *= 0.5f;
            myDodgeCopy -= 2;
        }

        miss = MyAttacks - MyHits;
        miss *= 0.5f;

        fit = (myHitsNew + myDodgeNew + MyBlocks - 0.25f *(miss + OpponentBlocks));
        if(MyHits<1.5f)
        {
            fit *= 0.75f;
        }
        

       

        

        if (Random.Range(0.0f, 1.0f) <= 0.05f)
        Debug.Log("My hits: " + myHitsNew + " Dodge: " + myDodgeNew + " Blocks: " + (MyBlocks - OpponentBlocks) + " Miss: " + miss + " Fitness: "+fit);

        //fit *= runHitRatio;
        /*
        if (runHitRatio <= 0.5f)
            fit = 0.0f;
        else
            fit += runHitRatio;
            */
        if (fit < 0 )
            fit = 0;

        return fit;
    }
    public void InitArray()
    {
        InputArr = box.InputSignalArray;

        for (int i = 0; i < 14; i++)
        {
            InputArr[i] = 0.0f;
        }
    }
    public void UpdateArray()
    {
        InputArr[(int)LastPState] = 0.0f;
        InputArr[(int)PState] = 1.0f;
        //Normalizing distance
        LS.Distance /= 15;
        if (LS.Distance > 1.0f)
            LS.Distance = 1.0f;
        InputArr[13] = LS.Distance;

    }
    public int FindMaxActivation()
    {
        double[,] max = new double[13, 2];
        int count = 0;
        max[0, 0] = -1000;
        max[0, 1] = -1000;
        //Action is stored in max[x,0]
        int Action;

        for (int i = 0; i < 13; i++)
        {
            //Ignore jump state
            if (i == 2)
                continue;
            if (OutputArr[i] > max[count, 1])
            {
                count = 0;
                max[count, 1] = OutputArr[i];//Highest value
                max[count, 0] = i;//Index of highest value
            }
            else if (OutputArr[i] == max[count, 1])
            {
                count++;
                max[count, 1] = OutputArr[i];//Highest value
                max[count, 0] = i;//Index of highest value

                //Now one more index has a second action with the same value
            }
        }
        if (count > 0)
        {
            Action = (int)max[Random.Range(0, count + 1), 0];
        }
        else
        {
            Action = (int)max[0, 0];
        }

        return Action;
    }
}
