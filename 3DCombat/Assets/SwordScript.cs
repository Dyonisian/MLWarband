using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {
    public ParticleSystem Particles;
    [SerializeField]
    PlayerScript PScript;
    [SerializeField]

    EnemyScript MyEnemyScript;
    enum State { Idle, Walk, Jump, Attack1, Attack2, Attack3, Block, Hit };

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider col)
    {
        if (PScript.GetState() == PlayerScript.State.Attack1 || PScript.GetState() == PlayerScript.State.Attack2 || PScript.GetState() == PlayerScript.State.Attack3 || PScript.GetState() == PlayerScript.State.Attack4)
        {
            if (transform.root.CompareTag ("Player"))
            {
                if (col.CompareTag("Enemy"))
                {
                    Particles.Play();
                    StopParticles();
                }
                if (col.CompareTag("EnemySword"))
                {
                    //Destroy(col.gameObject);

                    Particles.Play();
                    StopParticles();
                }
            }
            else //AI Sword, not player
            {
                if (MyEnemyScript == null)
                    MyEnemyScript = transform.parent.GetComponent<EnemyScript>();
                if(col.CompareTag("PlayerBlock"))
                {
                    Particles.Play();
                    StopParticles();
                    if(MyEnemyScript.IsReinforcementLearning)
                    {
                        MyEnemyScript.RLGiveReward(-0.3f);
                    }
                    
                }
                else if (col.CompareTag("Player"))
                {
                    Particles.Play();
                    StopParticles();
                    if (MyEnemyScript.IsReinforcementLearning)
                    {
                        MyEnemyScript.RLGiveReward(1.0f);
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
