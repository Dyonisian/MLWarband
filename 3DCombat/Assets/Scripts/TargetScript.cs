using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour {
     public GameObject BulletPrefab;
    public float BulletTime = 0.0f;
    Vector3 Direction;
    public GameObject PlayerTarget;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        BulletTime += Time.deltaTime;
        if(BulletTime>=4.0f)
        {
            Direction = (PlayerTarget.transform.position + Vector3.up) - this.transform.position;
            Direction.Normalize();
            GameObject bullet = Instantiate(BulletPrefab, transform.position + Direction * 1.5f, transform.rotation) as GameObject;
            bullet.GetComponent<Rigidbody>().velocity = Direction * 2.0f;
            BulletTime = 0.0f;
            Destroy(bullet, 5.0f);
        }
	}
}
