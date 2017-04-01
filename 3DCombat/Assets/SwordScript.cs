using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {
    public ParticleSystem Particles;
    [SerializeField]
    PlayerScript MyPScript;
    [SerializeField]

    EnemyScript MyEnemyScript;
    [SerializeField]
    public PlayerScript OpponentScript;

    [SerializeField]

    EnemyScriptNeat MyEnemyScriptNeat;

    public bool IsNeat;
    enum State { Idle, Walk, Jump, Attack1, Attack2, Attack3, Block, Hit };

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider col)
    {
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
                        if (MyEnemyScript.IsReinforcementLearning)
                        {
                            MyEnemyScript.RLGiveReward(0.3f, MyPScript.GetState(), true);
                        }
                    }
                    else
                    {
                        
                    }

                }
                else if (col.CompareTag("Enemy"))
                {
                    Particles.Play();
                    StopParticles();
                    if (OpponentScript.Invincibility <= 0.0f)
                    {
                        OpponentScript.Invincibility = 0.2f;
                        OpponentScript.Hit = true;
                        
                    }
                }
                else if (col.CompareTag("EnemySword"))
                {
                    //Destroy(col.gameObject);

                    Particles.Play();
                    StopParticles();
                }
            }
            else //AI Sword, not player's
            {
                
                if (col.CompareTag("PlayerBlock"))
                {

                    MyEnemyScript.Hit = true;
                    OpponentScript.Invincibility = 0.2f;

                    if (!IsNeat)
                    {
                        if (MyEnemyScript.IsReinforcementLearning)
                        {
                            MyEnemyScript.RLGiveReward(-0.3f, OpponentScript.GetState(), MyEnemyScript.CanHit);
                        }
                    }
                    else
                    {

                    }
                    
                }
                else if (col.CompareTag("Player"))
                {
                    Particles.Play();
                    StopParticles();
                   
                    if (OpponentScript.Invincibility <= 0.0f )
                    {
                        OpponentScript.Invincibility = 0.2f;
                        OpponentScript.Hit = true;
                        if (!IsNeat)
                        {
                            if (MyEnemyScript.IsReinforcementLearning)
                            {
                                MyEnemyScript.RLGiveReward(1.0f, OpponentScript.GetState(), MyEnemyScript.CanHit);
                            }
                        }
                        else
                        {
                            MyEnemyScriptNeat.ChangeMyHits(1);
                        }
                    }

                }
                else if (col.CompareTag("PlayerSword"))
                {
                    //Destroy(col.gameObject);

                    Particles.Play();
                    StopParticles();
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
