using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour {
    public PlayerScript EScript;
    public EnemyScript MyEnemyScript;
    public PlayerScript PScript;
    public bool IsPlayer = true;
	// Use this for initialization
	void Start () {
        if (transform.root.CompareTag("Player"))
            IsPlayer = true;
        else
            IsPlayer = false;
        PScript = transform.root.GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col)
    {
        if((col.CompareTag("EnemySword")&&IsPlayer)    ||  (col.CompareTag("PlayerSword"))&&!IsPlayer)
        {
            if(EScript==null)
            EScript = col.transform.root.GetComponent<PlayerScript>();

            EScript.Hit = true;
            PScript.Invincibility = 0.2f;

            if(MyEnemyScript.IsReinforcementLearning)
            {
                MyEnemyScript.RLGiveReward(0.5f);
            }
        }
        
    }
}
