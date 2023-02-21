using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class DeleteSlider : PinchSlider
{
    public float deleteThresh; // Placeholder variable storing deletion threshold
    public float sliderBase = 0.75f; // Base value of slider
    public float sliderRange = 0.5f; // Range of slider

    // Method that runs when the pointer on the slider is dragged:
    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        deleteThresh = ((sliderInfo.NewValue - 0.5f) * (sliderRange/0.5f)) + sliderBase; // Updates delete threshold
    }
}
