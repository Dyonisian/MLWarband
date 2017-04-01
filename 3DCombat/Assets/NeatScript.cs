using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpNeat.Phenomes;

public class NeatScript : UnitController
{

    bool IsRunning;
    IBlackBox box;
    public EnemyScriptNeat MyEnemyScript;
    PlayerScript.State PState, LastPState;
    public int MyHits { get; set; }
    public int OpponentHits { get; set; }
    ISignalArray InputArr;
    ISignalArray OutputArr;
    int[] DataArray;
    EnemyScriptNeat.LearningStateNeat LS;

    void FixedUpdate()
    {
        if (IsRunning)
        {
            //Check hit
            //Check cooldown
            //If not on cooldown, do action

            //PState = MyEnemyScript.GetState();
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
        float fit = MyHits / OpponentHits;

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
            if (InputArr[i] > max[count, 1])
            {
                count = 0;
                max[count, 1] = InputArr[i];//Highest value
                max[count, 0] = i;//Index of highest value
            }
            else if (InputArr[i] == max[count, 1])
            {
                count++;
                max[count, 1] = InputArr[i];//QValue
                max[count, 0] = i;//QAction

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
