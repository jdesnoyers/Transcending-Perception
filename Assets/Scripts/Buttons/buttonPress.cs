using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Leap.Unity.Interaction;

public class buttonPress : MonoBehaviour {

    public bool buttonActivated;	                // Has the button been turned on or off?

    [Header("3D/2D Button Settings")]
    public GameObject button;
	public GameObject onText;
	public GameObject offText;
	public GameObject buttonBase;
    [ColorUsage(true,true,0,8,.125f,3)] public Color onColor;

    [Header("2D Button Settings")]
    public bool proximityDetector = false;          // Use the proximity detector feature or not
    
    public GameObject targetObject;                 // Object that triggers the proximity detectors if contacted

    [Space(10)]

    // Left Hand
    
    public GameObject LeftThumb;
    
    public GameObject LeftIndexFinger;
    
    public GameObject LeftMiddleFinger;
    
    public GameObject LeftRingFinger;
    
    public GameObject LeftPinkieFinger;

    [Space(10)]

    // Right Hand
    
    public GameObject RightThumb;
    
    public GameObject RightIndexFinger;
    
    public GameObject RightMiddleFinger;
    
    public GameObject RightRingFinger;
    
    public GameObject RightPinkieFinger;

    private GameObject[] HandArr = new GameObject[10];  // Store fingers in an array

    Leap.Unity.ProximityDetector proxDetect;        // Access Leap Motions proximity detector methods and variables
    

    // Button Base
    private Color baseOrigCol;

    // Button
	private float buttonPosOrig;	        // Button Z Origin
	private float buttonPosCurr;	        // Button Z Current Position
    private int proxDetectActive = -1;      // Which proximity detector is currently active

	// Use this for initialization
	void Start () {

		buttonActivated = false;
		baseOrigCol = buttonBase.GetComponent<Renderer> ().material.GetColor("_EmissionColor");

        if (proximityDetector == true)
        {

            initHand();

        }

    }

    // Update is called once per frame
    void Update () {

        if (proximityDetector == true)
        {

            // Loop through hand array
            for (int i = 0; i < 10; i++)
            {

                // Get proximity detector component on each finger
                proxDetect = HandArr[i].GetComponent<Leap.Unity.ProximityDetector>();

                // If proximity detector has an object within range
                if(proxDetect.CurrentObject != null)
                {

                    // Track the active object with the array value
                    proxDetectActive = i;

                }

                // If active proximity detector leaves the object
                if (proxDetect.CurrentObject == null && proxDetectActive == i && (onText.activeSelf || offText.activeSelf))
                {

                    // Call Button UnPresed and reset
                    ButtonUnPressed();
                    proxDetectActive = -1;

                }

            }

        }

        

	}	//END UPDATE


    public void ButtonPressed()
    {

        // Is the button currently activated or de-activated
        if(buttonActivated == false)
        {

            buttonActivated = true;
            buttonBase.GetComponent<Renderer>().material.SetColor("_EmissionColor", onColor);   // Change base to color active color
            onText.SetActive(true);
            if (offText.activeSelf)
                offText.SetActive(false);
        }
        else
        {

            buttonActivated = false;
            buttonBase.GetComponent<Renderer>().material.SetColor("_EmissionColor", baseOrigCol); // Revert to neutral color
            offText.SetActive(true);
            if (onText.activeSelf)
                onText.SetActive(false);
        }

        /*
        // If text isn't already active
        if (!onText.activeSelf && !offText.activeSelf)
        {

            if (buttonActivated)
            {

                // Display text and change color
                onText.SetActive(true);
                buttonBase.GetComponent<Renderer>().material.SetColor("_EmissionColor",onColor);   // Change base to color active color

            }
            else
            {

                // Display text and change color
                offText.SetActive(true);
                buttonBase.GetComponent<Renderer>().material.SetColor("_EmissionColor",baseOrigCol); // Revert to neutral color

            }

        }*/

    }


    public void ButtonUnPressed()
    {

        // Wait 1 second to run the method before continuing
		StartCoroutine(wait ());


    }

	IEnumerator wait(){

		yield return new WaitForSeconds(1f);
		onText.SetActive(false);											// Disable text
		offText.SetActive(false);											// Disable text
		//buttonBase.GetComponent<Renderer> ().material.color = baseOrigCol;	// Change base to original color


	}

    private void initHand()
    {

        // Initialize Hand into an Array for ease of access
        HandArr[0] = LeftThumb;
        HandArr[1] = LeftIndexFinger;
        HandArr[2] = LeftMiddleFinger;
        HandArr[3] = LeftRingFinger;
        HandArr[4] = LeftPinkieFinger;
        HandArr[5] = RightThumb;
        HandArr[6] = RightIndexFinger;
        HandArr[7] = RightMiddleFinger;
        HandArr[8] = RightRingFinger;
        HandArr[9] = RightPinkieFinger;

        // Loop through hand array
        for(int i = 0; i < 10; i++)
        {

            //Initialize OnProximity to current object and ButtonPressed Method
            proxDetect = HandArr[i].GetComponent<Leap.Unity.ProximityDetector>();
            proxDetect.OnProximity.AddListener(delegate { ButtonPressed(); });
            proxDetect.TargetObjects = new GameObject[1];
            proxDetect.TargetObjects.SetValue(targetObject, 0);

        }

    }

}
