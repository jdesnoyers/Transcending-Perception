using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Leap.Unity.Interaction;
using UnityEngine.Audio;

public class SliderPotControls : MonoBehaviour {

    public ParticleSystem rain;
    public AudioMixer masterMixer;

    public float gravityMax = 10f;
    public float gravityMin = 0f;

    public float resMax = 6f;
    public float resMin = 0f;

    public float cutMin = 200f;
    public float cutMax = 2000f;

    public float decMin = .25f;
    public float decMax = .75f;

    public float delMin = 40f;
    public float delMax = 2000f;
    
    public TextMeshPro gravityText;
    public TextMeshPro resonanceText;
    public TextMeshPro cutoffText;
    public TextMeshPro decayText;
    public TextMeshPro delayText;

    public InteractionSlider gravitySlider;
    public InteractionSlider resonanceSlider;
    public InteractionSlider cutoffSlider;
    public InteractionSlider decaySlider;
    public InteractionSlider delaySlider;
    

    public void SetRainGravity()
    {
        float g = Mathf.Lerp(gravityMin, gravityMax, gravitySlider.HorizontalSliderValue);
        
        var m = rain.main;
        m.gravityModifier = g;
        gravityText.text = "Gravity: \n" + g.ToString("F2");
    }

    public void SetRainResonance()
    {
        float resonance = Mathf.Lerp(resMin, resMax, resonanceSlider.HorizontalSliderValue);
        masterMixer.SetFloat("DroneLowRes", resonance);
        resonanceText.text = "Resonance: \n" + resonance.ToString("F2");
    }

    public void SetRainCutoff()
    {
        float cutoff = Mathf.Lerp(cutMin, cutMax, Mathf.Pow(cutoffSlider.HorizontalSliderValue,2));
        masterMixer.SetFloat("DroneLowCut", cutoff);
        cutoffText.text = "Cutoff: \n" + Mathf.RoundToInt(cutoff);
    }
    
    public void SetRainDecay()
    {

        float decay = Mathf.Lerp(decMin, decMax, decaySlider.HorizontalSliderValue);
        masterMixer.SetFloat("DroneEchoDecay", decay);
        decayText.text = "Decay: \n" + Mathf.RoundToInt(decay*100) + "%";
    }
    public void SetRainDelay()
    {
        float delay = Mathf.Lerp(delMin, delMax, Mathf.Pow(delaySlider.HorizontalSliderValue, 2));
        masterMixer.SetFloat("DroneEchoDelay", delay);
        delayText.text = "Delay: \n" + Mathf.RoundToInt(delay);
    }

}
