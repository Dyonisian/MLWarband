using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour {
    public PlayerScript EScript;
    public PlayerScript PScript;
    public bool IsPlayer = true;
	// Use this for initialization
	void Start () {
        if (transform.parent.parent.CompareTag("Player"))
            IsPlayer = true;
        else
            IsPlayer = false;
        PScript = transform.parent.parent.GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("EnemySword")||col.CompareTag("PlayerSword"))
        {
            if(EScript==null)
            EScript = col.transform.parent.parent.GetComponent<PlayerScript>();

            EScript.Hit = true;
            PScript.Invincibility = 0.2f;
        }
        
    }
}
