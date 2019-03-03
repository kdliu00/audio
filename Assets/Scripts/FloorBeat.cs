using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBeat : MonoBehaviour
{
    private const int RANGE = 43;

    public Color loColor = new Color(0.3f, 0.8f, 0.9f);
    public Color hiColor = new Color(0.5f, 0.9f, 0.7f);

    public float decayTime = 0.1f;

    private float colorMult = 0;
    private float loVolume = 0;
    private float hiVolume = 0;
    private float cooldown = 0;

    private int index;
    private double[,] energies;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        energies = new double[8, RANGE];
    }

    // Update is called once per frame
    void Update()
    {
        CheckBeat();
        UpdateBeat();
    }

    private void CheckBeat()
    {
        for (int i = 0; i < 8; i++)
        {
            if (i > 2 && i < 6)
            {
                continue;
            }
            float energy = SpectrumAnalyzer.audioBands[i];
            energies[i, index] = energy;

            double mean = 0;
            for (int j = 0; j < RANGE; j++)
            {
                mean += energies[i, j] / RANGE;
            }

            double variance = 0;
            for (int j = 0; j < RANGE; j++)
            {
                variance += (energies[i, j] - mean) * (energies[i, j] - mean) / RANGE;
            }

            double thresh = -0.25 * variance + 1.54 - 0.02 * i;
            if (energy > thresh * mean)
            {
                CreateBeat(i);
            }
        }
        index = (index + 1) % RANGE;
    }

    private void CreateBeat(int band)
    {
        if (band < 4)
        {
            loVolume = 1;
        }
        else
        {
            hiVolume = 1;
        }
        colorMult = 1f;
        cooldown = 60f / 200;
    }

    private void UpdateBeat()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        // Change color
        Color newColor = transform.GetComponent<MeshRenderer>().material.GetColor("_Color");
        newColor = Color.Lerp(newColor, (loColor * loVolume + hiColor * hiVolume) / 2, Time.deltaTime / 0.1f);
        // transform.GetComponent<MeshRenderer>().material.SetColor("_Color", newColor);
        // transform.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", newColor);
        
        float t = Time.deltaTime / decayTime;
        loVolume *= 1 - t;
        hiVolume *= 1 - t;
    }
}
