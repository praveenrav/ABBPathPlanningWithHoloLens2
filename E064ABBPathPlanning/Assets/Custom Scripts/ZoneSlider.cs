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
    //private int numSnapPoints = 15;
    //float lastSnapPoint;
    //float snapPointIncr;
    //bool isSelected = false;
    //public int sliderVal_norm;
    public string[] zone_vals = new string[] {"fine", "z0", "z1", "z5", "z10", "z15", "z20", "z30", "z40", "z50", "z60", "z80", "z100", "z150", "z200"};
    public string zoneVal;
    public int sliderIndexValue;
#pragma warning restore 649

    public PointPicking DataContainer;



    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        sliderIndexValue = (int)Mathf.Ceil(sliderInfo.NewValue * 10);
        updateLabel();
    }

    public void updateLabel()
    {
        if (sliderIndexValue == 10)
        {
            sliderIndexValue = 9;
        }

        zoneVal = zone_vals[sliderIndexValue];
        sliderLabel.text = "Zone: " + zoneVal;
        DataContainer.zoneInd = sliderIndexValue;

    }

}
