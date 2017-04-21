using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutLog : MonoBehaviour {
    public GameObject[] Blocks;
    public string AttackDirection;
    public Rigidbody SecondLog;
    Vector3 Force;
	// Use this for initialization
	void Start () {
        foreach (GameObject b in Blocks)
        {
            if (b.name=="Block"+AttackDirection)
            {
                b.SetActive(false);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.tag);
        if(col.CompareTag("PlayerSword"))
        {
            SecondLog.isKinematic = false;
            SecondLog.useGravity = true;
            if (AttackDirection == "Right")
            {
                Force = col.transform.root.right * 1000;
                Force += this.transform.up * 1000;
                Force += col.transform.root.forward * 1000;
            }
                    else if (AttackDirection == "Left")
            {
                Force = col.transform.root.right * -1000;
                Force += this.transform.up * 1000;
                Force += col.transform.root.forward * 1000;
            }
            else if (AttackDirection == "Up")
            {
                Force = col.transform.root.right * 000;
                Force += this.transform.up * 1000;
                Force += col.transform.root.forward * 1000;
            }
              else
            {
                Force = col.transform.root.right * 1000;
                Force += this.transform.up * 1000;
                Force += col.transform.root.forward * 1000;
            }
                
            SecondLog.AddForce(Force);

        }
    }
}
