using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAudio : MonoBehaviour
{
    [SerializeField]
    AudioClip[] SlashSounds;
    [SerializeField]
    AudioClip[] GruntSounds;

    [SerializeField]
    AudioSource Source;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlaySlash()
    {
       
            int s = Random.Range(0, SlashSounds.Length);
            Source.volume = Random.Range(0.5f, 1.0f);
        Source.PlayDelayed(0.4f);
        Source.PlayOneShot(SlashSounds[s]);
        

        
    }
    public void PlayGrunt(float Chance = 1.0f)
    {
        if (Random.Range(0.0f, 1.0f) <= Chance)
        {
            int s = Random.Range(0, GruntSounds.Length);
            Source.volume = Random.Range(0.5f, 1.0f);

            Source.PlayOneShot(GruntSounds[s]);
        }
    }

}
