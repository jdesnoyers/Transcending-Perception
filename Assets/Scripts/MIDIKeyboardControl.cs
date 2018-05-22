using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MidiJack;
using HTC.UnityPlugin.PoseTracker;

/* main class for capturing keypresses and keyboard inputs from MIDIjack
 * 
 */

public class MIDIKeyboardControl : MonoBehaviour
{

    public int octaveShift = 24;                                        //octave shift variable, set to 24 to match fixed keyboard prototype
    public GameObject[] keyboardKeys;                                   //keyboard key array
    public float[] keyboardKeysHue;                                     //keyboard key color array
    public Material headMaterial;
    [SerializeField] private string keyboardKeysName = "Keyboard-Key_"; //key naming convention

    [SerializeField] private int keyUpAngle = -1;           //angle of key when at rest
    [SerializeField] private int keyDownAngle = 6;          //angle of key when pressed
    [SerializeField] private int rotationPerFrame = 1;      //rate of pressing/releasing key
    
    private Color[] defaultKeyColor;    //retains the default color for referencing when key is pressed

    //variables for averaging the keypress colours to change the head colour
    private Dictionary<int,Color> colorAverager = new Dictionary<int, Color>();
    private Color headColorAverage;


    void Start()
    {
        //initialize arrays
        keyboardKeys = new GameObject[61];
        keyboardKeysHue = new float[61];
        defaultKeyColor = new Color[61];

        for (int i = 0; i < keyboardKeys.Length; i++)  //for all of the keys
        {
            string findKeyboardKey = keyboardKeysName + string.Format("{0:000}", i + 1);   //set name of this key

            keyboardKeys[i] = transform.Find(findKeyboardKey).gameObject;               //find key with this name

            keyboardKeysHue[i] = i / (60f / 0.8f);      //set hue for all keys (rainbow effect)
            defaultKeyColor[i] = keyboardKeys[i].GetComponent<Renderer>().material.color;
        }

        MidiMaster.noteOnDelegate += NoteOnHandler;
        MidiMaster.noteOffDelegate += NoteOffHandler;
        

    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < keyboardKeys.Length; i++)  //for all of the keys
        {

            float keyangle = keyboardKeys[i].transform.localEulerAngles.x;  //get angle of the key

            if (keyangle > 180)
                keyangle = keyangle - 360;  //translate from 0-360 to -180-180

            if (MidiMaster.GetKey(i + octaveShift) > 0)    //if the key is pressed
            {
                if (keyangle < keyDownAngle)
                {
                    keyboardKeys[i].transform.Rotate(rotationPerFrame, 0, 0);           //move key to pressed position
                }
            }
            else
            {
                if (keyangle > keyUpAngle)
                {
                    keyboardKeys[i].transform.Rotate(-rotationPerFrame, 0, 0);          //move key to rest postion
                    
                }
                else if(keyangle < keyUpAngle)
                {
                    keyboardKeys[i].transform.localEulerAngles = new Vector3(keyUpAngle, 0, 0); //reset key if it travels too far
                }
            }
            

        }

        //set the head colour based on the average keypress colour
        headMaterial.SetColor("_EmissionColor", Color.Lerp(headMaterial.GetColor("_EmissionColor"), headColorAverage,10* Time.deltaTime));

    }

    void NoteOnHandler(MidiChannel channel, int note, float velocity)
    {

        int shiftedNote = note - octaveShift;

        if (keyboardKeys[shiftedNote].GetComponent<Rigidbody>() != null)
            keyboardKeys[shiftedNote].GetComponent<Rigidbody>().isKinematic = true;  //if the key is being simulated as a rigidbody make it kinematic

        //set colour based on the velocity and note pressed
        keyboardKeys[shiftedNote].GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.HSVToRGB(keyboardKeysHue[shiftedNote], 1f, 0.5f + (velocity * 1.5f)));
        keyboardKeys[shiftedNote].GetComponent<Renderer>().material.SetColor("_Color", Color.HSVToRGB(keyboardKeysHue[shiftedNote], .5f, .5f));

        //if the key hasn't been pressed add it to the averager
        if(!colorAverager.ContainsKey(shiftedNote))
        {
            Color emissionColor = Color.HSVToRGB(keyboardKeysHue[shiftedNote], 1f, 0.5f + velocity);
            headColorAverage = ((headColorAverage * colorAverager.Count) + emissionColor) / (colorAverager.Count + 1);

            colorAverager.Add(shiftedNote, emissionColor);

        }
        
    }

    void NoteOffHandler(MidiChannel channel, int note)
    {

        int shiftedNote = note - octaveShift;

        if (keyboardKeys[shiftedNote].GetComponent<Rigidbody>() != null)
            keyboardKeys[shiftedNote].GetComponent<Rigidbody>().isKinematic = false;  //release kinematic restraint to allow simulation of partial presses

        //reset colours to the default state
        keyboardKeys[shiftedNote].GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        keyboardKeys[shiftedNote].GetComponent<Renderer>().material.SetColor("_Color", defaultKeyColor[shiftedNote]);

        //if the key is in the averager remove it
        if(colorAverager.ContainsKey(shiftedNote))
        {

            colorAverager.Remove(shiftedNote);
            if (colorAverager.Count < 2)
            {
                headColorAverage = Color.black;
            }
            else
            {
                foreach (Color color in colorAverager.Values)
                {
                    headColorAverage += color;
                }
                headColorAverage /= colorAverager.Count;
            }

        }
        

    }

}
