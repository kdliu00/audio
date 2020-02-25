using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumAnalyzer : MonoBehaviour
{
    AudioSource audioSource;
    public bool coolbeans;
    public static float[] waveform = new float[1024];
    public static float[] samples = new float[512];
    // public static Buffer samples = new Buffer(512);

    private static int loudnessIndex = 0;
    private static float[] loudness = new float[86];

    // public FFTWindow fFTWindow;

    public static float energy = 0;
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
            int sampleCount = 2 << i;

            // Adding the remaining two samples into the last bin.
            if (i == 7) {
                sampleCount += 2;
            }

            // Go through the number of samples for each bin, add the data to the average
            for (int j = 0; j < sampleCount; j++) {
                average += samples[count];
                count++;
            }

            // Divide to create the average, and scale it appropriately.
            average /= count;
            freqBand[i] = (i+1) * 10 * average;
        }
    }

    void Start () {
        audioSource = GetComponent<AudioSource>();

        if (coolbeans) {
            GetComponent<BarSpectrum>().enabled = false;
        }
    }

    // public void Add(float left, float right) {
    //     // Debug.Log();
    //     float avg = (left + right) / 2;
    //     samples.Add(avg);
    //     Debug.Log(avg);
    //     int sampleLength = samples.Count;
    //     if (sampleLength == 512) {
    //         GetSpectrumAudioSource();
    //     }
    // }

    void GetSpectrumAudioSource() {
        
        if (coolbeans) {
            audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

            float volume = 0;
            for (int i = 0; i < 512; i++)
            {
                samples[i] = Mathf.Pow(256 + i, 1.1f) / 1492 * samples[i];
                volume += samples[i] * samples[i];
            }
            volume = Mathf.Sqrt(volume);

            loudness[loudnessIndex] = volume;
            loudnessIndex = (loudnessIndex + 1) % loudness.Length;

            volume = Mathf.Max(loudness);

            float t = Time.deltaTime / 10;
            for (int i = 0; i < 512; i++)
            {
                if (samples[i] > volume)
                {
                    samples[i] = 1;
                } else if (volume > 0)
                {
                    samples[i] = Mathf.Max(0, 3 + Mathf.Log10(samples[i] / volume)) / 3;
                }
            }
        }

        // audioSource.GetOutputData(waveform, 0);
        // energy = 0;
        // for (int i = 0; i < 1024; i++)
        // {
        //     energy += waveform[i] * waveform[i];
        // }
        // // audioSource.GetOutputData(waveform, 1);
        // for (int i = 0; i < 1024; i++)
        // {
        //     energy += waveform[i] * waveform[i];
        // }

        GaussianBlur(samples, 5);
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        // MakeFrequencyBands();
        // CreateAudioBands();
    }

    void GaussianBlur(float[] sample, int width = 3)
    {
        int sampleLength = sample.Length;
        float[] copy = new float[sampleLength + width - 1];
        float[] filter = new float[width];
        float total = 0;
        for (int i = 0; i < width; i++)
        {
            filter[i] = Mathf.Exp(-(i - width / 2) * (i - width / 2) / (float)(width * width));
            total += filter[i];
        }
        for (int i = 0; i < width; i++)
        {
            filter[i] /= total;
        }
        // Copy over
        for (int i = 0; i < width / 2; i++)
        {
            copy[i] = sample[0];
            copy[copy.Length - i - 1] = sample[sampleLength - 1];
        }
        for (int i = 0; i < sampleLength; i++)
        {
            copy[i + width / 2] = sample[i];
        }
        for (int i = 0; i < sampleLength; i++)
        {
            float result = 0;
            for (int j = 0; j < width; j++)
            {
                result += filter[j] * copy[i + j];
            }
            sample[i] = result;
        }
    }
}
