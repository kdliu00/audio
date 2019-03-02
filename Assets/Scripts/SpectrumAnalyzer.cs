using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumAnalyzer : MonoBehaviour
{

    AudioSource audioSource;
    public static float[] samples = new float[512];

    public FFTWindow fFTWindow;

    public static float[] freqBand = new float[8];

    public static float[] freqBandHighest = new float[8];
    public static float[] audioBands = new float[8];

    void CreateAudioBands() {
        for (int i = 0; i < 8; i++) {
            if (freqBandHighest[i].Equals(null) || freqBandHighest[i] < freqBand[i]) {
                freqBandHighest[i] = freqBand[i];
            }
            audioBands[i] = freqBand[i] / freqBandHighest[i];
        }
    }

    void MakeFrequencyBands() {
        int count = 0;
        
        // Iterate through the 8 bins.
        for (int i = 0; i < 8; i++)  {
            float average = 0;
            int sampleCount = (int)Mathf.Pow (2, i + 1);

            // Adding the remaining two samples into the last bin.
            if (i == 7) {
                sampleCount += 2;
            }

            // Go through the number of samples for each bin, add the data to the average
            for (int j = 0; j < sampleCount; j++) {
                average += samples [count];
                count++;
            }

            // Divide to create the average, and scale it appropriately.
            average /= count;
            freqBand[i] = (i+1) * 100 * average;
        }
    }

    void Start () {
        audioSource = GetComponent<AudioSource>();
    }

    void GetSpectrumAudioSource() {
        audioSource.GetSpectrumData(samples, 0, fFTWindow);
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        CreateAudioBands();
    }
}
