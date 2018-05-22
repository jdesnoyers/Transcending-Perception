using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* sets sun position to match approximate position of sun based on time of day
 *
 */

public class TimeOfDay : MonoBehaviour {
    
	void Start () {
		
	}
	
	void Update () {

        float angle =(System.DateTime.Now.Hour - 6) * (180/14) ;
        if (angle > 180)
        {
            angle = angle - 360;
        }

        transform.eulerAngles = new Vector3(angle, 0, 0);
	}
}
