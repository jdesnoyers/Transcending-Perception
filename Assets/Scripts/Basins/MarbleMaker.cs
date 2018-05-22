using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleMaker : MonoBehaviour {

    [SerializeField] private GameObject marblePrefab;
    [SerializeField] private GameObject marbleParent;
    public int maxMarbleCount = 200;
    private float marbleDelay = 0.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		

        if(Input.GetAxis("RightVRTrigger")>0.0f)
        {
            MakeMarblesMan(Input.GetAxis("RightVRTrigger"));
        }
	}
    
    private void MakeMarblesMan(float rate)
    {
        if (marbleDelay >= (1 / (rate*rate)))
        {
            if(marbleParent.transform.childCount < maxMarbleCount)
            {
                Instantiate(marblePrefab, transform.position + (Random.insideUnitSphere * 0.05f), transform.rotation, marbleParent.transform);
            }
            marbleDelay = 0;
        }
        else
        {
            marbleDelay++;
        }
    }
}
