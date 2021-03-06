﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitInstrument : MonoBehaviour {
    public Transform follow;

    [SerializeField] private float frequencyMin = 1f;
    [SerializeField] private float frequencyMax = 1000f;
    [SerializeField] private float gainMin = 0;
    [SerializeField] private float gainMax = 0.1f;
    [SerializeField] private float noteMin = 20f;
    [SerializeField] private float noteMax = 90f;
    [SerializeField] private float velocityMin = 0f;
    [SerializeField] private float velocityMax = 10f;
    [SerializeField] private float velocityLimit = 15f; //sets limit for what velocities are considered valid
    [SerializeField] private float heightMax = 2f;
    [SerializeField] private float posMin = -1f;
    [SerializeField] private float posMax= 1f;
    [SerializeField] private bool modifyMultiplier = false;
    [SerializeField] private bool modifyNote = false;
    
    private bool hitToggle = false;

    [SerializeField] private float thresholdVelocity = -1f;
    [SerializeField] private float thresholdHysterisis = -1f;

    [SerializeField] private int historySize = 10;
    //[SerializeField] private int shortHistorySize = 5;

    private Hv_dropletMax_AudioLib synth;
    private Vector3 untrackedJointScale = new Vector3(0, 1, 1);
    private float speed;

    private float velocityAverage = 0.0f;
    private Vector3 positionAverage = Vector3.zero;
    //private float shortVelocityAverage = 0.0f;

    private List<Vector3> positionHistory = new List<Vector3>();
    private List<float> velocityHistory = new List<float>();
    //private List<float> shortVelocityHistory = new List<float>();


    // Use this for initialization
    void Start()
    {
        synth = GetComponent<Hv_dropletMax_AudioLib>();

    }

    // Update is called once per frame
    void Update()
    {


        if (positionHistory.Count >= historySize)
        {
            positionAverage = Vector3.LerpUnclamped(positionHistory[0], positionAverage, ((float)positionHistory.Count) / (positionHistory.Count - 1));
            positionHistory.RemoveAt(0);
        }
        if (velocityHistory.Count >= historySize)
        {
            velocityAverage = Mathf.LerpUnclamped(velocityHistory[0], velocityAverage, ((float)velocityHistory.Count) / (velocityHistory.Count - 1));
            velocityHistory.RemoveAt(0);
        }
        /*if (shortVelocityHistory.Count >= shortHistorySize)
        {
            shortVelocityAverage = Mathf.LerpUnclamped(shortVelocityHistory[0], shortVelocityAverage, ((float)shortVelocityHistory.Count) / (shortVelocityHistory.Count - 1));
            shortVelocityHistory.RemoveAt(0);
        }*/

        transform.position = follow.position;

        Vector3 position = transform.position;
        positionHistory.Add(position);

        positionAverage = Vector3.LerpUnclamped(positionHistory[positionHistory.Count - 1], positionAverage, ((float)positionHistory.Count - 1) / positionHistory.Count);

        if (follow.localScale == untrackedJointScale)
        {
            velocityHistory.Add(0);
            velocityAverage = Mathf.LerpUnclamped(velocityHistory[velocityHistory.Count - 1], velocityAverage, ((float)velocityHistory.Count - 1) / velocityHistory.Count);
            
            //shortVelocityHistory.Add(0);
            //shortVelocityAverage = Mathf.LerpUnclamped(shortVelocityHistory[shortVelocityHistory.Count - 1], shortVelocityAverage, ((float)shortVelocityHistory.Count - 1) / shortVelocityHistory.Count);
            
            hitToggle = false;

        }
        else
        {

            if (positionHistory.Count > 1)
            {

                float triggerVelocity = (position.y - positionHistory[positionHistory.Count - 2].y) / Time.deltaTime;
                float velocity = Vector3.Magnitude(position - positionHistory[positionHistory.Count - 2]) / Time.fixedDeltaTime;

                if (velocity < velocityLimit) //check if the velocity is a reasonable value
                {
                    velocityHistory.Add(velocity);
                    velocityAverage = Mathf.LerpUnclamped(velocityHistory[velocityHistory.Count - 1], velocityAverage, ((float)velocityHistory.Count - 1) / velocityHistory.Count);
                    

                    //shortVelocityHistory.Add(velocity);
                    //shortVelocityAverage = Mathf.LerpUnclamped(shortVelocityHistory[shortVelocityHistory.Count - 1], shortVelocityAverage, ((float)shortVelocityHistory.Count - 1) / shortVelocityHistory.Count);


                    if ((triggerVelocity < (thresholdVelocity + thresholdHysterisis)) && !hitToggle)
                    {
                        hitToggle = true;
                    }
                    else if ((triggerVelocity > thresholdVelocity) && hitToggle)
                    {
                        hitToggle = false;

                        synth.SetFloatParameter(Hv_dropletMax_AudioLib.Parameter.Gain, Mathf.Lerp(gainMin, gainMax, Mathf.Pow((velocityAverage-velocityMin) / (velocityMax - velocityMin),2)));
                        synth.SetFloatParameter(Hv_dropletMax_AudioLib.Parameter.Cutoff, Mathf.Lerp(frequencyMin, frequencyMax, (velocityAverage - velocityMin) / (velocityMax - velocityMin)));

                        if(modifyMultiplier)
                        {
                            synth.SetFloatParameter(Hv_dropletMax_AudioLib.Parameter.Sqrmult, (positionAverage.z -posMin) / (posMax - posMin));
                            synth.SetFloatParameter(Hv_dropletMax_AudioLib.Parameter.Bandpass,(positionAverage.z - posMin) / (4*(posMax - posMin)));

                        }
                        if (modifyNote)
                        {
                            synth.SetFloatParameter(Hv_dropletMax_AudioLib.Parameter.Oscnote, Mathf.Round(Mathf.Lerp(noteMin, noteMax, (positionAverage.x - posMin) / (posMax - posMin))));
                        }

                        synth.SendEvent(Hv_dropletMax_AudioLib.Event.Bang);
                    }

                }
            }


        }
        


    }

}