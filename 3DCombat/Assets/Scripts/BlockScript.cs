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
       
        
    }
}
