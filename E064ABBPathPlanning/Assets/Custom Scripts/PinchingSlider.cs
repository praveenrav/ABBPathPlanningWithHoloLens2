using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class PinchingSlider : PinchSlider
{
    public float pinchSens; // Placeholder variable storing value of pinch sensitivity
    public float sliderBase = 0.7f; // Base value of slider
    public float sliderRange = 0.25f; // Range of sliders

    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        pinchSens = ((sliderInfo.NewValue - 0.5f) * (sliderRange / 0.5f)) + sliderBase; // Updating value of pinch sensitivity
    }
}
