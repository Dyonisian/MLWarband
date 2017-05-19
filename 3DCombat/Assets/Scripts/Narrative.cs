using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Narrative : MonoBehaviour {
    public string[] Dialog;
    public Text DialogText;
    public GameObject InstructionText;
    int DialogNo;
    public EnemyScript EScript;
    public PlayerScript PScript;
    [SerializeField]
    Image BoxImage;
	// Use this for initialization
	void Start () {
        DialogNo = -1;
	}
	
	// Update is called once per frame
	void Update () {
        if(SceneManager.GetActiveScene().buildIndex==5)
        {
            if (DialogNo % 2 == 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //InstructionText.SetActive(false);
                    DialogNo++;
                    InstructionText.SetActive(false);

                    BoxImage.color = new Color(BoxImage.color.r, BoxImage.color.g, BoxImage.color.b, 0);
                    DialogText.text = "";
                    EScript.Resume();
                    PScript.Resume();
                }
            }
        }

		else if(DialogNo!=-1)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                DisplayText();
            }
        }
	}
    public void DisplayText()
    {
        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            BoxImage.color = new Color(BoxImage.color.r, BoxImage.color.g, BoxImage.color.b, 1);
        }

        if (DialogNo < Dialog.Length - 1)
        {
            DialogNo++;

            DialogText.text = Dialog[DialogNo];
            InstructionText.SetActive(true);
        }
        else
        {
            InstructionText.SetActive(false);
        }
    }
}
