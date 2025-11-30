// Add this in a new file, e.g., BpskModulator.cs
using System;
using System.Collections.Generic;

namespace TransmissionSimulator.Models
{
    public class BpskModulator
    {
        public double Amplitude { get; }
        public double CarrierFrequency { get; }
        public int BitRate { get; }
        public int SamplingRate { get; }

        public BpskModulator(double amplitude, double carrierFrequency, int bitRate, int samplingRate)
        {
            Amplitude = amplitude;
            CarrierFrequency = carrierFrequency;
            BitRate = bitRate;
            SamplingRate = samplingRate;
        }

        /// <summary>
        /// Modulates an NRZ-Polar signal (+1, -1) onto a carrier wave.
        /// </summary>
        /// <param name="nrzPolarCode">A List of integers containing only 1s and -1s.</param>
        public double[] Modulate(List<int> nrzPolarCode)
        {
            if (nrzPolarCode == null || nrzPolarCode.Count == 0)
            {
                return new double[0];
            }

            double bitDuration = 1.0 / BitRate;
            int samplesPerBit = (int)(SamplingRate * bitDuration);
            int totalSamples = samplesPerBit * nrzPolarCode.Count;
            var modulatedSignal = new double[totalSamples];
            int sampleIndex = 0;

            foreach (int level in nrzPolarCode) // level will be +1 or -1
            {
                for (int i = 0; i < samplesPerBit; i++)
                {
                    double time = (double)sampleIndex / SamplingRate;
                    
                    // The core BPSK formula: level * Amplitude * cos(...)
                    modulatedSignal[sampleIndex] = level * Amplitude * Math.Cos(2 * Math.PI * CarrierFrequency * time);
                    
                    sampleIndex++;
                }
            }
            return modulatedSignal;
        }
    }
}