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
    //private int numSnapPoints = 10;
    //float lastSnapPoint;
    //float snapPointIncr;
    //bool isSelected = false;
    //public int sliderVal_norm;
    public string[] speed_vals = new string[] { "v50", "v100", "v150", "v200", "v300", "v400", "v500", "v600", "v800", "v1000"};
    public string speedVal;
    public int sliderIndexValue;
#pragma warning restore 649

    public PointPicking DataContainer;


    public void Update()
    {
        
    }

    
    public void OnPointerDragged(SliderEventData sliderInfo)
    {
        sliderIndexValue = (int) Mathf.Ceil(sliderInfo.NewValue * 10);
        updateLabel();
    }
    

    public void updateLabel()
    {
        if(sliderIndexValue == 10)
        {
            sliderIndexValue = 9;
        }
        
        speedVal = speed_vals[sliderIndexValue];
        sliderLabel.text = "Speed: " + speedVal;
        DataContainer.speedInd = sliderIndexValue;
    }

}
