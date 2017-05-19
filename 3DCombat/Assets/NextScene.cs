using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public GameObject[] Enemies;
    int RealtimeCount;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            int count = 0;
            foreach (GameObject en in Enemies)
            {
                if (en != null)
                {
                    count++;
                }
            }
            if (count == 0)
            {
                if (SceneManager.GetActiveScene().buildIndex == 77)
                {
                    if (PlayerPrefs.HasKey("Realtime"))
                    {
                        RealtimeCount = PlayerPrefs.GetInt("Realtime");
                        RealtimeCount++;
                        if (RealtimeCount >= 3)
                        {
                            PlayerPrefs.SetInt("Realtime", 0);
                            PlayerPrefs.SetFloat("Health", col.GetComponent<PlayerScript>().Health);
                            PlayerPrefs.Save();
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                        }
                        else
                        {
                            PlayerPrefs.SetInt("Realtime", RealtimeCount);
                            PlayerPrefs.SetFloat("Health", col.GetComponent<PlayerScript>().Health);
                            PlayerPrefs.Save();
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        }
                    }
                    else
                    {
                        PlayerPrefs.SetInt("Realtime", 1);
                        PlayerPrefs.SetFloat("Health", col.GetComponent<PlayerScript>().Health);
                        PlayerPrefs.Save();
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
                else
                {
                    PlayerPrefs.SetFloat("Health", col.GetComponent<PlayerScript>().Health);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
        }
    }
}
