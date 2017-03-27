using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ReinforcementLearning : MonoBehaviour
{
    PlayerScript.State PState;
    int actions, states;
    

    [SerializeField]

    private int QState, QAction, QLastState, QLastAction;

    EnemyScript.LearningState LastLS;
    bool CanHit = false;
    public int LoadFromFile = 0;
    public float LearningRate = 0.5f;
    public int FileNo;
    public float Exploration;
    public int Iterations, Decisions;
    private float InitialExploration;
    bool RewardReceived;
    [System.Serializable]
    public class TestData : ScriptableObject
    {
        public string curDataStorage;
        //QDataClass QData;
    }
    [System.Serializable]
    public class QDataClass
    {
        public TestData curData = null;
        [SerializeField]
        public float[] Q0;
        public float[] Q1;
        public float[] Q2;
        public float[] Q3;
        public float[] Q4;
        public float[] Q5;
        public float[] Q6;
        public float[] Q7;
        public float[] Q8;
        public float[] Q9;
        public float[] Q10;
        public float[] Q11;
        public float[] Q12;
        public float[] Q13;
        public float[] Q14;
        public float[] Q15;
        public float[] Q16;
        public float[] Q17;
        public float[] Q18;
        public float[] Q19;
        public float[] Q20;
        public float[] Q21;
        public float[] Q22;
        public float[] Q23;
        public float[] Q24;
        public float[] Q25;


        public QDataClass()
            {
            curData = null;
            Q0 = new float[13];
            Q1 = new float[13];
            Q2 = new float[13];
            Q3 = new float[13];
            Q4 = new float[13];
            Q5 = new float[13];
            Q6 = new float[13];
            Q7 = new float[13];
            Q8 = new float[13];
            Q9 = new float[13];
            Q10 = new float[13];
            Q11 = new float[13];
            Q12 = new float[13];
            Q13 = new float[13];
            Q14 = new float[13];
            Q15 = new float[13];
            Q16 = new float[13];
            Q17 = new float[13];
            Q18 = new float[13];
            Q19 = new float[13];
            Q20 = new float[13];
            Q21 = new float[13];
            Q22 = new float[13];
            Q23 = new float[13];
            Q24 = new float[13];
            Q25 = new float[13];
            

        }
    }
   
    
    QDataClass QData;
    public float[,] QValues;

    string Path;
    [SerializeField]
    public ArrayLayout InspectorQValues;
    // Use this for initialization
    void Start()
    {
        Iterations = 0;
        Decisions = 0;
        QLastAction = -1;
        QLastState = -1;
        RewardReceived = false;
        InitialExploration = Exploration;
        Path = "Assets/RL" + FileNo + ".json";

        actions = 13;
        states = 26;
        //14 is same as 1 but with can hit true
        
        QData = new QDataClass();
        //QData.InspectorQValues = new ArrayLayout();
        //QData.InspectorQValues.rows[0] = new ArrayLayout.rowData();
        QValues = new float[states, actions];
        if (LoadFromFile == 0)
        {
            for (int i = 0; i < 26; i++)
            {

                for (int j = 0; j < 13; j++)
                {
                    QValues[i, j] = 0;

                }

            }
        }
        else
        {
          
            LoadData();
            
        }
        

    }

    // Update is called once per frame
    void Update()
    {

    }
    public int RLStep(EnemyScript.LearningState LS)
    {
        //Update last QValue? If hasn't been updated yet, either give small negative feedback or none. Move accomplished nothing
        if(!RewardReceived && Decisions>0)
        {
            //Avoided an attack, small positive bonus
            if (LastLS.PState == PlayerScript.State.Attack1 || LastLS.PState == PlayerScript.State.Attack2 || LastLS.PState == PlayerScript.State.Attack3 || LastLS.PState == PlayerScript.State.Attack4)
            {
                
               
                if(QLastAction==1 || QLastAction == 12 || QLastAction == 11)
                    UpdateQValues(2.0f, LastLS);
                else
                    UpdateQValues(0.1f, LastLS);

                Iterations--;
            }
            else
            {
                UpdateQValues(-0.01f, LastLS);
                Iterations--;
            }
        }
        
        Exploration = InitialExploration - (Iterations/200.0f);
        QState = (int)LS.PState;
        //The states where CanHit is true start from 13
        if (LS.CanHit)
            QState += 13;

        //Pair - QAction, QValue
        float[,] max = new float[13,2];
        int count = 0;
        max[0,0] = -1000;
        max[0, 1] = -1000;

        float r = Random.Range(0.0f, 1.0f);
        if (r < Exploration)
        {
            QAction = Random.Range(0, 13);
        }
        else
        {
            for (int i = 0; i < 13; i++)
            {
                //2 is jump, not used anymore. The others are handled by RLMoveScript
                if (i == 0 || i == 1 || i == 2 || i == 11 || i == 12)
                    continue;
                if (QValues[QState, i] > max[count, 1])
                {
                    count = 0;
                    max[count, 1] = QValues[QState, i];//QValue
                    max[count, 0] = i;//QAction                               
                }
                else if (QValues[QState, i] == max[count, 1])
                {
                    count++;
                    max[count, 1] = QValues[QState, i];//QValue
                    max[count, 0] = i;//QAction

                    //Now one more index has a second action with the same QValue
                }
            }
            if (count > 0)
            {
                QAction = (int)max[Random.Range(0, count + 1), 0];
            }
            else
            {
                QAction = (int)max[0, 0];
            }
        }
        Decisions++;
        int temp = QState;
        if (temp > 13)
            temp -= 13;
        //Debug.Log("State to Action" + (EnemyScript.State)temp + ", " + QAction);
        LastLS = LS;
        RewardReceived = false;
        return QAction;

    }
    public void UpdateQValues(float Reward, EnemyScript.LearningState LS)
    {
        RewardReceived = true;
        //Update to State when this reward was received, for accurate mapping
        //Ensures that the reward is updated for whatever state the player was in, for the action last taken
        QState = (int)LS.PState;
        //The states where CanHit is true start from 13
        if (LS.CanHit)
            QState += 13;
        //Q(state, action) = R(state, action) + Gamma * Max[Q(next state, all actions)]
        //𝑄(𝑆𝑡𝑎𝑡𝑒,𝐴𝑐𝑡𝑖𝑜𝑛)= (1−𝐿𝑒𝑎𝑟𝑛𝑖𝑛𝑔 𝑅𝑎𝑡𝑒)∗(𝐶𝑢𝑟𝑟𝑒𝑛𝑡 𝑄)+(𝐿𝑒𝑎𝑟𝑛𝑖𝑛𝑔 𝑅𝑎𝑡𝑒∗𝑅𝑒𝑤𝑎𝑟𝑑 𝑉𝑎𝑙𝑢𝑒)
        //Debug.Log("Reward received" + Reward);
        //QValues[QState, QAction] = (1 - LearningRate) * QValues[QState, QAction] + (LearningRate * Reward);
        //Other learning method from DeWolf

        if (QLastState >= 0 && QLastAction >= 0)
            QValues[QLastState, QLastAction] += LearningRate * (QValues[QState, QAction]);
        QValues[QState, QAction] += LearningRate * (Reward - QValues[QState, QAction] );

        

        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                
                
                InspectorQValues.rows[i].row[j] = QValues[i, j];

            }
        }

        Iterations++;
        QLastState = QState;
        QLastAction = QAction;

        //SaveData();
        
    }
    
    void SaveData()
    {
        QData.Q0 = InspectorQValues.rows[0].row;
        QData.Q1 = InspectorQValues.rows[1].row;
        QData.Q2 = InspectorQValues.rows[2].row;
        QData.Q3 = InspectorQValues.rows[3].row;
        QData.Q4 = InspectorQValues.rows[4].row;
        QData.Q5 = InspectorQValues.rows[5].row;
        QData.Q6 = InspectorQValues.rows[6].row;
        QData.Q7 = InspectorQValues.rows[7].row;
        QData.Q8 = InspectorQValues.rows[8].row;
        QData.Q9 = InspectorQValues.rows[9].row;
        QData.Q10 = InspectorQValues.rows[10].row;
        QData.Q11 = InspectorQValues.rows[11].row;
        QData.Q12 = InspectorQValues.rows[12].row;
        QData.Q13 = InspectorQValues.rows[13].row;
        QData.Q14 = InspectorQValues.rows[14].row;
        QData.Q15 = InspectorQValues.rows[15].row;
        QData.Q16 = InspectorQValues.rows[16].row;
        QData.Q17 = InspectorQValues.rows[17].row;
        QData.Q18 = InspectorQValues.rows[18].row;
        QData.Q19 = InspectorQValues.rows[19].row;
        QData.Q20 = InspectorQValues.rows[20].row;
        QData.Q21 = InspectorQValues.rows[21].row;
        QData.Q22 = InspectorQValues.rows[22].row;
        QData.Q23 = InspectorQValues.rows[23].row;
        QData.Q24 = InspectorQValues.rows[24].row;
        QData.Q25 = InspectorQValues.rows[25].row;

        // FileStream FS;
        string FileData;
        
        FileData = JsonUtility.ToJson(QData);
        if (File.Exists("RL" + FileNo))
            File.Delete("RL" + FileNo);

        //FS = File.Open("RL" + FileNo,FileMode.OpenOrCreate,FileAccess.ReadWrite);
        File.WriteAllText(Path, FileData);
       

        /*
        if (File.Exists("RL" + FileNo))
            File.Delete("RL" + FileNo);
        
            //FS = File.Open("RL" + FileNo,FileMode.OpenOrCreate,FileAccess.ReadWrite);
        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 13; j++)
             File.AppendAllText("RL" + FileNo, Data.QValues[i,j].ToString()+" ");
        }
        */

    }
    void LoadData()
    {
        string FileData = File.ReadAllText(Path);
        if(FileData!=null)
        {
            JsonUtility.FromJsonOverwrite(FileData, QData);
            //QData = JsonUtility.FromJson<QDataClass>(Path);
            Debug.Log("Trying to load data");
            InspectorQValues.rows[0].row = QData.Q0;
            InspectorQValues.rows[1].row = QData.Q1;
            InspectorQValues.rows[2].row = QData.Q2;
            InspectorQValues.rows[3].row = QData.Q3;
            InspectorQValues.rows[4].row = QData.Q4;
            InspectorQValues.rows[5].row = QData.Q5;
            InspectorQValues.rows[6].row = QData.Q6;
            InspectorQValues.rows[7].row = QData.Q7;
            InspectorQValues.rows[8].row = QData.Q8;
            InspectorQValues.rows[9].row = QData.Q9;
            InspectorQValues.rows[10].row = QData.Q10;
            InspectorQValues.rows[11].row = QData.Q11;
            InspectorQValues.rows[12].row = QData.Q12;
            InspectorQValues.rows[13].row = QData.Q13;
            InspectorQValues.rows[14].row = QData.Q14;
            InspectorQValues.rows[15].row = QData.Q15;
            InspectorQValues.rows[16].row = QData.Q16;
            InspectorQValues.rows[17].row = QData.Q17;
            InspectorQValues.rows[18].row = QData.Q18;
            InspectorQValues.rows[19].row = QData.Q19;
            InspectorQValues.rows[20].row = QData.Q20;
            InspectorQValues.rows[21].row = QData.Q21;
            InspectorQValues.rows[22].row = QData.Q22;
            InspectorQValues.rows[23].row = QData.Q23;
            InspectorQValues.rows[24].row = QData.Q24;
            InspectorQValues.rows[25].row = QData.Q25;

            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    QValues[i, j] = InspectorQValues.rows[i].row[j];
                    //if (QValues[i, j] > 1.0f)
                        //QValues[i, j] = 1.0f;



                }
            }
            
        }
       
        else
        {
            Debug.Log("File data was null!");
        }
    }
    TestData CreateDatabase()
    {
        TestData FileData = (TestData)ScriptableObject.CreateInstance(typeof(TestData));
        if(FileData!=null)
        {
            AssetDatabase.CreateAsset(FileData, Path);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return FileData;

        }
        else
        {
            return null;
        }
    }
    void OnApplicationQuit()
    {
        SaveData();
    }
}
