using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointObjects : MonoBehaviour
{
    public Transform[] pointChildren;

    // Start is called before the first frame update
    void Start()
    {
        pointChildren = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            pointChildren[i] = transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
