using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCircleScript : MonoBehaviour
{
    private RectTransform rectComponent;
    private float rotatespeed = 200f;

    // Start is called before the first frame update
    void Start()
    {
        rectComponent = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectComponent.Rotate(0f, 0f, -rotatespeed * Time.deltaTime);
    }
}
