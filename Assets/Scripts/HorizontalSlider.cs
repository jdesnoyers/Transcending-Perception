/*
 * Script to handle horizontal potentiometer on Mixed Reality MIDI Keyboard
 * Takes MIDI as input and adjusts virtual displays accordingly
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MidiJack;
using TMPro;


public class HorizontalSlider : MonoBehaviour {

    public string controlName = "Rain Rate";    //name of control
    public bool displayAsMidiCode;              //when true displays raw MIDI integers (out of 127), otherwise displays as float (max 1.0)
    [SerializeField] private GameObject sliderKnob; //the knob gameobject
    [SerializeField] private GameObject uiCanvas;   //the canvas to display the setting on
    [SerializeField] private GameObject leftTextDisplay;    //left hand text display object
    [SerializeField] private GameObject rightTextDisplay;    //right hand text display object
    [SerializeField] private GameObject localTextDisplay;    //object to display values locally
    [SerializeField] private float sliderMax = 100;    //the maximum distance the slider travels
    public float maxBrightness = 2.0f;
    private Vector3 abstractionStart;
    private Color knobMinColour;
    private Color knobMaxColour;
    private Image uiImage;

    
    void Start () {

        abstractionStart = sliderKnob.transform.localPosition; //set start value to initial position

        uiImage = uiCanvas.transform.GetChild(0).GetComponent<Image>(); //find image in canvas

        //set starting colour and maximum colour
        float hue; float sat; float val;
        knobMinColour = sliderKnob.GetComponent<Renderer>().material.GetColor("_EmissionColor");
        Color.RGBToHSV(knobMinColour,out hue,out sat,out val);
        knobMaxColour = Color.HSVToRGB(hue, sat, maxBrightness);

        MidiMaster.knobDelegate += SliderKnobHandler;
        displayValue(controlName, 0, false);    //set value output to default

    }

    // Update is called once per frame
    void Update () {

    }

    void SliderKnobHandler(MidiChannel channel, int knob, float value)
    {
        switch (knob)
        {
            case 20:    //if the knob is the abstraction knob
                {
                    sliderKnob.transform.localPosition = new Vector3(abstractionStart.x + (value * sliderMax), abstractionStart.y, abstractionStart.z);   //set position

                    sliderKnob.GetComponent<Renderer>().material.SetColor("_EmissionColor",Color.Lerp(knobMinColour,knobMaxColour,value)); //set colour
                    uiImage.fillAmount = value; //fill the UI
                    displayValue(controlName, value);    //set value output
                }
                break;
        }
    }

    void displayValue(string input, float value, bool global = false)
    {
        //scale float to midi value out of 127
        if (displayAsMidiCode == true)
        {
            value = (int)(value * 127);
        }

        localTextDisplay.GetComponent<TextMeshPro>().text = string.Concat(input, ": ", value);  //display text locally

        //if global text is requested, activate and display on the closest hand
        if (global)
        {
            if (Vector3.SqrMagnitude(leftTextDisplay.transform.position - transform.position) > Vector3.SqrMagnitude(rightTextDisplay.transform.position - transform.position))
            {
                if(rightTextDisplay != null)
                {
                    rightTextDisplay.SetActive(true);
                    rightTextDisplay.GetComponent<TextMeshPro>().text = string.Concat(input, ": ", value);
                }

            }
            else
            {
                if (leftTextDisplay != null)
                {
                    leftTextDisplay.SetActive(true);
                    leftTextDisplay.GetComponent<TextMeshPro>().text = string.Concat(input, ": ", value);
                }
            }
        }
    }

}



