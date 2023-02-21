using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
public class ZoneSlider : PinchSlider
{
#pragma warning disable 649
    [SerializeField]
    [Tooltip("The number of snap points")]
    public TextMeshProUGUI sliderLabel;
    public string[] zone_vals = new string[] {"fine", "z0", "z1", "z5", "z10", "z15", "z20", "z30", "z40", "z50", "z60", "z80", "z100", "z150", "z200"}; // String array representing all possible zone values
    public string zoneVal; // String representing selected zone value
    public int sliderIndexValue; // Index of the selected value of the slider
#pragma warning restore 649

    public PointPicking DataContainer;

    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        sliderIndexValue = (int)Mathf.Ceil(sliderInfo.NewValue * 15); 
        updateLabel();
    }

    public void updateLabel()
    {
        // Method to run while the slider is being modified to update the UI as well as store the index of the selected zone value

        // Determines if the slider is set to its maximum value:
        if (sliderIndexValue == 15)
        {
            sliderIndexValue = 14;
        }

        zoneVal = zone_vals[sliderIndexValue]; // Obtains the zone value corresponding to the index of the selected slider value
        sliderLabel.text = "Zone: " + zoneVal; // Updates UI
        DataContainer.zoneInd = sliderIndexValue; // Stores the index of the selected zone value

    }

}
