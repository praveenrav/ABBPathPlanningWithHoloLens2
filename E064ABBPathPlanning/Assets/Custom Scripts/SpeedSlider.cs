using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;

public class SpeedSlider : PinchSlider
{

#pragma warning disable 649
    [SerializeField]
    [Tooltip("The number of snap points")]
    public TextMeshProUGUI sliderLabel;
    public string[] speed_vals = new string[] { "v50", "v100", "v150", "v200", "v300", "v400", "v500", "v600", "v800", "v1000"}; // String array representing all possible speed values
    public string speedVal; // String representing selected speed value
    public int sliderIndexValue; // Index of the selected value of the slider
#pragma warning restore 649

    public PointPicking DataContainer;

    // Method that runs when the pointer on the slider is dragged:
    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        sliderIndexValue = (int) Mathf.Ceil(sliderInfo.NewValue * 10);
        updateLabel();
        
    }

    public void updateLabel()
    {
        // Method to run while the slider is being modified to update the UI as well as store the index of the selected speed value

        // Determines if the slider is set to its maximum value:
        if(sliderIndexValue == 10)
        {
            sliderIndexValue = 9;
        }
        
        speedVal = speed_vals[sliderIndexValue]; // Obtains the speed value corresponding to the index of the selected slider value
        sliderLabel.text = "Speed: " + speedVal; // Updates UI
        DataContainer.speedInd = sliderIndexValue; // Stores the index of the selected speed value
    }

}
