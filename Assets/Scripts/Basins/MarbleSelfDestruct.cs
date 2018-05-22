using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleSelfDestruct : MonoBehaviour {

    [SerializeField] private ParticleSystem marblePoof;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < 0.05f)
        {
            Instantiate(marblePoof, transform.position, transform.rotation, transform.parent);
            Destroy(gameObject);  
        }
	}
}
