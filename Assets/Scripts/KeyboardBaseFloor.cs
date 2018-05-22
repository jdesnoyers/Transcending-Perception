using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keep the base of the keyboard attached to the floor so that the height can be adjusted

public class KeyboardBaseFloor : MonoBehaviour {

    private Vector3 offset;
    
	void Start () {
        offset = transform.localPosition;
	}
	void LateUpdate () {
        transform.localPosition = offset;
        transform.position = new Vector3(transform.position.x, 0,transform.position.z);
	}
}
