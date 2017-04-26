using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PotionScript : MonoBehaviour {

    [SerializeField]
    PlayerScript PScript;
    [SerializeField]
    Text PotionText;

    [SerializeField]
    int PotionCount;
	// Use this for initialization
	void Start () {
		if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            
            PlayerPrefs.SetInt("Potions", PotionCount);
            PlayerPrefs.Save();
        }
        else
        {
            PotionCount = PlayerPrefs.GetInt("Potions");
        }
        PotionText.text = "x" + PotionCount;
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.T))
        {
            if (PotionCount > 0)
            {
                PotionCount--;
                PlayerPrefs.SetInt("Potions", PotionCount);
                PlayerPrefs.Save();
                PScript.Health = 100;
                PotionText.text = "x" + PotionCount;

            }

        }
        if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            PotionCount++;
            PlayerPrefs.SetInt("Potions", PotionCount);
            PlayerPrefs.Save();
            PotionText.text = "x" + PotionCount;
        }
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }


    }
}
