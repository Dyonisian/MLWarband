using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAudio : MonoBehaviour {
    
    [SerializeField]
    AudioClip[] HitSounds;

    [SerializeField]
    AudioSource Source;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void PlayHit()
    {
        int s = Random.Range(0, HitSounds.Length);
        Source.volume = Random.Range(0.5f, 1.0f);

        Source.PlayOneShot(HitSounds[s]);
    }
}
