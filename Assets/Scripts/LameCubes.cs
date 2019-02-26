using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LameCubes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float colorMult = transform.localScale.y / 100f;
        Color col = new Color(0.4f * colorMult, 0.2f * colorMult, colorMult);
        transform.GetComponentInChildren<MeshRenderer>().material.color = col;
        transform.GetComponentInChildren<Light>().color = col;
    }
}
