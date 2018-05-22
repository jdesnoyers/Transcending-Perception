using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MidiJack;

//turns on and off tracking when button on keyboard is pressed

public class TrackingControl : MonoBehaviour {

    private bool pauseState = false;

    public UnityEvent trackerPauseOn;
    public UnityEvent trackerPauseOff;
    

    
    void Start () {
        MidiMaster.knobDelegate += TrackerKnobHandler;
    }
    

    void TrackerKnobHandler(MidiChannel channel, int knob, float value)
    {
        //Vive tracking mode
        switch (knob)
        {

            case 111:   //pause tracker button pressed
                {
                    if ((value == 0) && pauseState)
                    {
                        pauseState = false;
                        trackerPauseOff.Invoke();


                    }
                    else if ((value > 0) && !pauseState)
                    {

                        pauseState = true;

                        trackerPauseOn.Invoke();

                    }
                }
                break;

        }
        
    }

}
 