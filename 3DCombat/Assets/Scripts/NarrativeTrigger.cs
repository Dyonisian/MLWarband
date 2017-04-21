using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeTrigger : MonoBehaviour {
    public Narrative NarrativeScript;
    bool DialogDisplayed;
	// Use this for initialization
	void Start () {
        DialogDisplayed = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col)
    {
        if(col.transform.root.CompareTag("Player") && !DialogDisplayed)
        {
            NarrativeScript.DisplayText();
            Debug.Log("triggered");

            DialogDisplayed = true;
        }
    }
}
