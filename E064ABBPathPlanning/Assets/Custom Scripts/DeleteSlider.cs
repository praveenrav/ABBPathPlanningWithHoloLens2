using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class DeleteSlider : PinchSlider
{
    public float deleteThresh;
    public float sliderBase = 0.75f; // Base value of slider
    public float sliderRange = 0.5f; // Range of slider

    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        deleteThresh = ((sliderInfo.NewValue - 0.5f) * (sliderRange/0.5f)) + sliderBase;
    }
}
