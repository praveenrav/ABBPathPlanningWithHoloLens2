using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class CoordinateSlider : PinchSlider
{
    public float coorVal;
    public float coorRange = 0.25f; // Range of sliders in meters

    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        coorVal = (float) ((sliderInfo.NewValue - 0.5) * (coorRange/0.5));
    }
}
