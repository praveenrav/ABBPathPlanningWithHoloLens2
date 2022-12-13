using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class AddSlider : PinchSlider
{
    public float addThresh;
    public float sliderBase = 0.5f; // Base value of slider
    public float sliderRange = 0.35f; // Range of sliders

    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        addThresh = ((sliderInfo.NewValue - 0.5f) * (sliderRange/ 0.5f)) + sliderBase;
    }
}
