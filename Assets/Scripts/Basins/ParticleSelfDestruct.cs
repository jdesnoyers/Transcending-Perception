using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour {

    public float destroyDelay = 5.0f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, destroyDelay);
	}

}
