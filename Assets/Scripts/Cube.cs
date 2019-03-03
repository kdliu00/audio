using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{

    public int band;
    public float startScale;
    public float scaleMultiplier;

    private Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        scaleMultiplier = 10;
        startScale = 10;
        target = new Vector3(1, 1, 1);
    }

    void Update () {
        if (SpectrumAnalyzer.freqBand[band] * scaleMultiplier < startScale) {
            target = new Vector3(startScale, startScale, startScale);
        } else {
            target = new Vector3(startScale, 
                SpectrumAnalyzer.freqBand[band] * scaleMultiplier, startScale);
        }
        if (transform.localScale.y > 30) {
            target = new Vector3(startScale,
                SpectrumAnalyzer.freqBand[band] * scaleMultiplier / 3f, startScale);
        }
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime / 0.1f);
        // Color change
        float colorMult = transform.localScale.y / 20f;
        Color col = new Color(0, colorMult, Mathf.Sqrt(colorMult));
        transform.GetComponentInChildren<MeshRenderer>().material.color = col;
        transform.GetComponentInChildren<Light>().color = col;
        transform.GetComponentInChildren<Light>().intensity = 0;
	}
}
