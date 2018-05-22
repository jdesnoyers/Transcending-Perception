using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidInterfaceCalcuations : MonoBehaviour {

    //calculation boundary distance for each basin
    [SerializeField] private float maxCalcDistance = 0.3f;
    private float maxCalcDistanceSqr;
    
    //the axes around which the rotational momentum is calculated
    [SerializeField] private Transform basinAxis1;
    [SerializeField] private Transform basinAxis2;
    [SerializeField] private Transform basinAxis3;

    //output min/max
    [SerializeField] private float frequencyMin = 20f;
    [SerializeField] private float frequencyMax = 1000f;
    [SerializeField] private float gainMin = 0;
    [SerializeField] private float gainMax = 0.1f;
    [SerializeField] private float oscMin = 1f;
    [SerializeField] private float oscMax = 1000f;
    [SerializeField] private float ringFreqMin = 1f;
    [SerializeField] private float ringFreqMax = 1000f;
    [SerializeField] private float ringMultMin = 0;
    [SerializeField] private float ringMultMax = 1f;
    
    //input maximums
    [SerializeField] private float energyMax = .01f;
    [SerializeField] private float momentumMax = 0.001f;
    [SerializeField] private float netVelocityMax = 1.0f;

    //variables for calculating and storing the energy, momentum, and velocity of particles in each system
    private float totalEnergy1 = 0;
    private float totalEnergy2 = 0;
    private float totalEnergy3 = 0;

    private float totalRotationalMomentum1 = 0;
    private float totalRotationalMomentum2 = 0;
    private float totalRotationalMomentum3 = 0;

    private Vector3 netVelocity1 = Vector3.zero;
    private Vector3 netVelocity2 = Vector3.zero;
    private Vector3 netVelocity3 = Vector3.zero;


    // Use this for initialization
    void Start () {
        maxCalcDistanceSqr = maxCalcDistance*maxCalcDistance;
	}
	
	// Update is called once per frame
	void Update () {


        //reset counting variables
        totalEnergy1 = 0;
        totalEnergy2 = 0;
        totalEnergy3 = 0;

        totalRotationalMomentum1 = 0;
        totalRotationalMomentum2 = 0;
        totalRotationalMomentum3 = 0;

        netVelocity1 = Vector3.zero;
        netVelocity2 = Vector3.zero;
        netVelocity3 = Vector3.zero;

        foreach (Transform child in transform)
        {
            if (Vector3.SqrMagnitude(child.position - basinAxis1.position) <= maxCalcDistanceSqr) //if it's in the first basin
            {
                Rigidbody body = child.GetComponent<Rigidbody>();   //get rigidbody component
                if(body != null)   //verify it's not null
                {
                    totalEnergy1 += Vector3.SqrMagnitude(body.velocity) * body.mass;    //calculate total particle energy where E = m * |v|^2
                    totalRotationalMomentum1 += Vector3.Dot(Vector3.Cross(child.position - basinAxis1.position, body.mass * body.velocity),Vector3.up); //calculate total angular momentum where L = R x mV
                    netVelocity1 += body.velocity; //calculate net velocity in x,y,z
                }

            }
            else if (Vector3.SqrMagnitude(child.position - basinAxis2.position) <= maxCalcDistanceSqr) //if it's in the second basin
            {
                Rigidbody body = child.GetComponent<Rigidbody>();   //get rigidbody component
                if (body != null)   //verify it's not null
                {
                    totalEnergy2 += Vector3.SqrMagnitude(body.velocity) * body.mass;    //calculate total particle energy where E = m * |v|^2
                    totalRotationalMomentum2 += Vector3.Dot(Vector3.Cross(child.position - basinAxis2.position, body.mass * body.velocity), Vector3.up); //calculate total angular momentum where L = R x mV
                    netVelocity2 += body.velocity; //calculate net velocity in x,y,z
                }
            }
            else if (Vector3.SqrMagnitude(child.position - basinAxis3.position) <= maxCalcDistanceSqr) //if it's in the third basin
            {
                Rigidbody body = child.GetComponent<Rigidbody>();   //get rigidbody component
                if (body != null)   //verify it's not null
                {
                    totalEnergy3 += Vector3.SqrMagnitude(body.velocity) * body.mass;    //calculate total particle energy where E = m * |v|^2
                    totalRotationalMomentum3 += Vector3.Dot(Vector3.Cross(child.position - basinAxis3.position, body.mass * body.velocity), Vector3.up); //calculate total angular momentum where L = R x mV
                    netVelocity3 += body.velocity; //calculate net velocity in x,y,z
                }
            }
        }

        //update parameters on synthesizers based on inputs
        Hv_simple_sin_synth_AudioLib sinSynth = basinAxis1.GetComponent<Hv_simple_sin_synth_AudioLib>();
        sinSynth.SetFloatParameter(Hv_simple_sin_synth_AudioLib.Parameter.Gain, Mathf.Lerp(gainMin,gainMax, totalEnergy1/energyMax));
        sinSynth.SetFloatParameter(Hv_simple_sin_synth_AudioLib.Parameter.Oscfreq, Mathf.Lerp(oscMin,oscMax, Mathf.Abs(totalRotationalMomentum1)/momentumMax));
        sinSynth.SetFloatParameter(Hv_simple_sin_synth_AudioLib.Parameter.Freqcutoff,Mathf.Lerp(frequencyMin,frequencyMax, Mathf.Abs(netVelocity1.x)/ netVelocityMax));
        sinSynth.SetFloatParameter(Hv_simple_sin_synth_AudioLib.Parameter.Ringmodfreq, Mathf.Lerp(ringFreqMin, ringFreqMax, Mathf.Abs(netVelocity1.y) / netVelocityMax));
        sinSynth.SetFloatParameter(Hv_simple_sin_synth_AudioLib.Parameter.Ringmodmultiplier, Mathf.Lerp(ringMultMin, ringMultMax, Mathf.Abs(netVelocity1.z) / netVelocityMax));

        Hv_simple_saw_synth_AudioLib sawSynth = basinAxis2.GetComponent<Hv_simple_saw_synth_AudioLib>();
        sawSynth.SetFloatParameter(Hv_simple_saw_synth_AudioLib.Parameter.Gain, Mathf.Lerp(gainMin, gainMax, totalEnergy2 / energyMax));
        sawSynth.SetFloatParameter(Hv_simple_saw_synth_AudioLib.Parameter.Oscfreq, Mathf.Lerp(oscMin, oscMax, Mathf.Abs(totalRotationalMomentum2) / momentumMax));
        sawSynth.SetFloatParameter(Hv_simple_saw_synth_AudioLib.Parameter.Freqcutoff, Mathf.Lerp(frequencyMin, frequencyMax, Mathf.Abs(netVelocity2.x) / netVelocityMax));
        sawSynth.SetFloatParameter(Hv_simple_saw_synth_AudioLib.Parameter.Ringmodfreq, Mathf.Lerp(ringFreqMin, ringFreqMax, Mathf.Abs(netVelocity2.y) / netVelocityMax));
        sawSynth.SetFloatParameter(Hv_simple_saw_synth_AudioLib.Parameter.Ringmodmultiplier, Mathf.Lerp(ringMultMin, ringMultMax, Mathf.Abs(netVelocity2.z) / netVelocityMax));

        Hv_simple_square_synth_AudioLib squareSynth = basinAxis3.GetComponent<Hv_simple_square_synth_AudioLib>();
        squareSynth.SetFloatParameter(Hv_simple_square_synth_AudioLib.Parameter.Gain, Mathf.Lerp(gainMin, gainMax, totalEnergy3 / energyMax));
        squareSynth.SetFloatParameter(Hv_simple_square_synth_AudioLib.Parameter.Oscfreq, Mathf.Lerp(oscMin, oscMax, Mathf.Abs(totalRotationalMomentum3) / momentumMax));
        squareSynth.SetFloatParameter(Hv_simple_square_synth_AudioLib.Parameter.Freqcutoff, Mathf.Lerp(frequencyMin, frequencyMax, Mathf.Abs(netVelocity3.x) / netVelocityMax));
        squareSynth.SetFloatParameter(Hv_simple_square_synth_AudioLib.Parameter.Ringmodfreq, Mathf.Lerp(ringFreqMin, ringFreqMax, Mathf.Abs(netVelocity3.y) / netVelocityMax));
        squareSynth.SetFloatParameter(Hv_simple_square_synth_AudioLib.Parameter.Ringmodmultiplier, Mathf.Lerp(ringMultMin, ringMultMax, Mathf.Abs(netVelocity3.z) / netVelocityMax));
    }
    

}
