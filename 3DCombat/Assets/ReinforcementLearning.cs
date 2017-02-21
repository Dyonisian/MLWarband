using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcementLearning : MonoBehaviour
{
    PlayerScript.State PState;
    int actions, states;
    [SerializeField]
    private float[,] QValues;
    [SerializeField]

    private int QState, QAction;
    bool CanHit = false;
    public int LoadFromFile = 0;
    public float LearningRate = 0.5f;
    // Use this for initialization
    void Start()
    {
        actions = 13;
        states = 26;
        //14 is same as 1 but with can hit true
        QValues = new float[states, actions];
        if (LoadFromFile != 0)
        {
            for (int i = 0; i < 26; i++)
            {

                for (int j = 0; j < 13; j++)
                {
                    QValues[i, j] = 0;

                }

            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public int RLStep(EnemyScript.LearningState LS)
    {
        //Update last QValue? If hasn't been updated yet, either give small negative feedback or none. Move accomplished nothing

        QState = (int)LS.PState;
        //The states where CanHit is true start from 13
        if (LS.CanHit)
            QState += 13;

        //Pair - QAction, QValue
        float[,] max = new float[13,2];
        int count = 0;
        max[0,0] = 0;
        max[0, 1] = 0;
        for (int i=0; i<13; i++)
        {

            if (QValues[QState, i] > max[count, 1])
            {
                count = 0;
                max[count, 1] = QValues[QState, i];//QValue
                max[count, 0] = i;//QAction                               
            }
            else if(QValues[QState, i] == max[count, 1])
            {
                count++;
                max[count, 1] = QValues[QState, i];//QValue
                max[count, 0] = i;//QAction

                //Now one more index has a second action with the same QValue
            }
        }
        if(count>0)
        {
            QAction = (int)max[Random.Range(0, count+1), 0];
        }
        else
        {
            QAction = (int)max[0, 0];
        }

        return QAction;

    }
    public void UpdateQValues(float Reward)
    {
        //Q(state, action) = R(state, action) + Gamma * Max[Q(next state, all actions)]
        //𝑄(𝑆𝑡𝑎𝑡𝑒,𝐴𝑐𝑡𝑖𝑜𝑛)= (1−𝐿𝑒𝑎𝑟𝑛𝑖𝑛𝑔 𝑅𝑎𝑡𝑒)∗(𝐶𝑢𝑟𝑟𝑒𝑛𝑡 𝑄)+(𝐿𝑒𝑎𝑟𝑛𝑖𝑛𝑔 𝑅𝑎𝑡𝑒∗𝑅𝑒𝑤𝑎𝑟𝑑 𝑉𝑎𝑙𝑢𝑒)

        QValues[QState, QAction] = (1 - LearningRate) * QValues[QState, QAction] + (LearningRate * Reward);
    }
}
