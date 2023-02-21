using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class AddSlider : PinchSlider
{
    public float addThresh; // Placeholder variable storing addition threshold
    public float sliderBase = 0.5f; // Base value of slider
    public float sliderRange = 0.35f; // Range of sliders

    // Method that runs when the pointer on the slider is dragged:
    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        addThresh = ((sliderInfo.NewValue - 0.5f) * (sliderRange/ 0.5f)) + sliderBase; // Updates addition threshold
    }
}
