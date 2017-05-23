using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour {
    public GameObject[] Enemies;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            int count = 0;
            foreach(GameObject en in Enemies)
            {
                if(en!=null)
                {
                    count++;
                }
            }
            if(count==0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
