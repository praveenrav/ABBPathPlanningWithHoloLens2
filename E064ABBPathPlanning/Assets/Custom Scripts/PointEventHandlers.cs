using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointEventHandlers : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        counter++;
        Debug.Log(counter.ToString());
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        Debug.Log("end" + counter.ToString());
    }

    public void OnMouseDown()
    {
        counter++;
        Debug.Log("start " + counter.ToString());
    }

    public void OnMouseUp()
    {
        Debug.Log("end " + counter.ToString());
    }

}
