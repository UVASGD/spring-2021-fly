using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class PerlinNoise
{
    // Math from https://www.redblobgames.com/maps/terrain-from-noise/
    public static float Sample(float x, float y, float elevationFreq,float elevationAmplitude, int octaveDepth, float redistribution)
    {
        float z = 0f;
        for (int octave = 0; octave < octaveDepth; octave++)
        {
            float multiplier = Mathf.Pow(2f, octave);
            float divisor = 1 / multiplier;
            z += divisor * Mathf.PerlinNoise(multiplier * x * elevationFreq / 100f, multiplier * y * elevationFreq / 100f);
        }
        z = elevationAmplitude * Mathf.Pow(z, redistribution);
        return z;
    }
}
