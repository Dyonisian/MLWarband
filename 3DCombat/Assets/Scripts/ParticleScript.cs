using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //StartCoroutine("DisableParticles");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    IEnumerator DisableParticles()
    {
        yield return new WaitForSeconds(0.3f);
        //this.gameObject.SetActive(false);

    }
}
