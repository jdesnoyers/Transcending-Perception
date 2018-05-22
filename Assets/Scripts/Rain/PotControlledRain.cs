using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class PotControlledRain : MonoBehaviour
{

    public float rainMinSetting;
    public float rainMaxSetting;
    public ParticleSystem rainSystem;

    public int midiKnobNumber = 20;

    private float rainSetting;


    // Use this for initialization
    void Start()
    {
        MidiMaster.knobDelegate += RainPotHandler;  //subscribe to knob delegate
        
        if(rainSystem == null)
        {
            rainSystem = GetComponent<ParticleSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void RainPotHandler(MidiChannel channel, int knob, float value)
    {
            if (knob == midiKnobNumber) //if the incoming knob value matches this knob
            {

                rainSetting = (value * (rainMaxSetting - rainMinSetting)) + rainMinSetting;
                var emission = rainSystem.emission;
                emission.rateOverTime = rainSetting;
            

            }
        }
    }