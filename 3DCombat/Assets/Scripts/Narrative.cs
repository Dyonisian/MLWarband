using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Narrative : MonoBehaviour {
    public string[] Dialog;
    public Text DialogText;
    public GameObject InstructionText;
    int DialogNo;
	// Use this for initialization
	void Start () {
        DialogNo = -1;
	}
	
	// Update is called once per frame
	void Update () {
		if(DialogNo!=-1)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                DisplayText();
            }
        }
	}
    public void DisplayText()
    {
        DialogNo++;
        if(DialogNo<9)
        DialogText.text = Dialog[DialogNo];
        InstructionText.SetActive(true);
    }
}
