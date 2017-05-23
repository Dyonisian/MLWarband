using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {
    public ParticleSystem Particles;
    public GameObject EnemyBlood;
    [SerializeField]
    PlayerScript MyPScript;
    [SerializeField]

    EnemyScript MyEnemyScript;
    [SerializeField]
    public PlayerScript OpponentScript;
    public EnemyScriptNeat OpponentNeatScript;
    [SerializeField]

    EnemyScriptNeat MyEnemyScriptNeat;
    
    public bool IsNeat;

    public int PlayerHits, MyAttacks, EnemyBlocks;
    public int OpponentHits, OpponentAttacks, PlayerBlocks;
    enum State { Idle, Walk, Jump, Attack1, Attack2, Attack3, Block, Hit };

    [SerializeField]
    SwordAudio AudioScript;

    [SerializeField]
    bool IsTrainingMode;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider col)
    {
        if (!IsTrainingMode)
        {
            AudioScript.PlayHit();
        }
        if (MyPScript.GetState() == PlayerScript.State.Attack1 || MyPScript.GetState() == PlayerScript.State.Attack2 || MyPScript.GetState() == PlayerScript.State.Attack3 || MyPScript.GetState() == PlayerScript.State.Attack4)
        {
            if (transform.root.CompareTag ("Player"))
            {
                if (col.CompareTag("EnemyBlock"))
                {

                    MyPScript.Hit = true;
                    OpponentScript.Invincibility = 0.2f;
                    if (!IsNeat)
                    {
                        if (MyEnemyScript != null)
                        {
                            if (MyEnemyScript.IsReinforcementLearning)
                            {
                                MyEnemyScript.RLGiveReward(0.3f, MyPScript.GetState(), true);
                            }
                        }
                    }
                    else
                    {
                        
                    }
                    //AI Blocked (Confusing variable name)

                    EnemyBlocks++;

                }
                else if (col.CompareTag("Enemy"))
                {
                    if (!IsTrainingMode)
                    {
                        Particles.Play();
                        StopParticles();
                    }
                    if (OpponentScript.Invincibility <= 0.0f)
                    {
                        OpponentScript.Invincibility = 0.2f;
                       
                        OpponentScript.Health -= 10;
                        OpponentScript.Hit = true;

                    }
                    if (!IsTrainingMode)
                    {
                        GameObject blood = Instantiate(EnemyBlood, col.transform.position + Vector3.up, Random.rotation);
                        Destroy(blood, 1.0f);
                    }
                    
                    PlayerHits++;
                }
                else if (col.CompareTag("EnemySword"))
                {
                    //Destroy(col.gameObject);
                    if (!IsTrainingMode)
                    {
                        Particles.Play();
                        StopParticles();
                    }
                }
            }
            else //AI Sword, not player's
            {
                
                if (col.CompareTag("PlayerBlock"))
                {
                    if (!IsNeat)
                        MyEnemyScript.Hit = true;
                    else
                        MyEnemyScriptNeat.Hit = true;
                    OpponentScript.Invincibility = 0.2f;

                    if (!IsNeat)
                    {
                        if (MyEnemyScript != null)
                        {
                            if (MyEnemyScript.IsReinforcementLearning)
                            {
                                MyEnemyScript.RLGiveReward(-0.3f, OpponentScript.GetState(), MyEnemyScript.CanHit);
                            }
                        }
                    }
                    else
                    {
                        PlayerBlocks++;
                    }
                    
                }
                else if (col.CompareTag("Player"))
                {
                    if (!IsTrainingMode)
                    {
                        Particles.Play();
                        StopParticles();
                    }
                    if (OpponentScript.Invincibility <= 0.0f )
                    {
                        OpponentScript.Invincibility = 0.2f;
                        OpponentScript.Hit = true;
                        OpponentScript.Health -= 10;
                        if (!IsNeat)
                        {
                            if (MyEnemyScript != null)
                            {
                                if (MyEnemyScript.IsReinforcementLearning)
                                {
                                    MyEnemyScript.RLGiveReward(1.0f, OpponentScript.GetState(), MyEnemyScript.CanHit);
                                }
                                MyEnemyScript.v = Random.Range(-0.2f, -1);
                            }
                            OpponentHits++;

                        }
                        else
                        {
                            PlayerHits++;
                            OpponentHits++;

                            //MyEnemyScriptNeat.v = Random.Range(-0.2f, -1);
                        }

                    }

                }
                else if (col.CompareTag("PlayerSword"))
                {
                    //Destroy(col.gameObject);
                    if (!IsTrainingMode)
                    {
                        Particles.Play();
                        StopParticles();
                    }
                }

            }
        }
    }
    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(0.3f);
        Particles.Stop();
        
    }
   
}
