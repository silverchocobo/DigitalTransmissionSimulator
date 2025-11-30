using System;

namespace TransmissionSimulator.Models
{
    public class AmiModulator
    {
        public double Amplitude { get; }
        public double CarrierFrequency { get; }
        public int BitRate { get; }
        public int SamplingRate { get; }

        public AmiModulator(double amplitude, double carrierFrequency, int bitRate, int samplingRate)
        {
            Amplitude = amplitude;
            CarrierFrequency = carrierFrequency;
            BitRate = bitRate;
            if (samplingRate < carrierFrequency * 4)
            {
                throw new ArgumentException("Sampling rate should be at least 4 times the carrier frequency for good resolution.");
            }
            SamplingRate = samplingRate;
        }

        /// <summary>
        /// Modulates a ternary AMI string ('0', '+', '-') onto a carrier wave.
        /// </summary>
        /// <param name="amiCode">A string containing '0', '+', and '-' characters.</param>
        /// <returns>An array of doubles representing the modulated signal's amplitude over time.</returns>
         public double[] Modulate(List<int> amiCode)
        {
            if (amiCode == null || amiCode.Count == 0)
            {
                return new double[0];
            }

            double symbolDuration = 1.0 / BitRate;
            int samplesPerSymbol = (int)(SamplingRate * symbolDuration);
            int totalSamples = samplesPerSymbol * amiCode.Count;
            double[] modulatedSignal = new double[totalSamples];
            int sampleIndex = 0;

            // Iterate over each INTEGER in the AMI code list
            foreach (int symbol in amiCode)
            {
                // The 'currentLevel' is now just the symbol itself (1, -1, or 0)
                double currentLevel = symbol;

                for (int i = 0; i < samplesPerSymbol; i++)
                {
                    if (currentLevel == 0.0)
                    {
                        modulatedSignal[sampleIndex] = 0.0;
                    }
                    else
                    {
                        double time = (double)sampleIndex / SamplingRate;
                        modulatedSignal[sampleIndex] = Amplitude * currentLevel * Math.Cos(2 * Math.PI * CarrierFrequency * time);
                    }
                    sampleIndex++;
                }
            }
            return modulatedSignal;
        }
    }
}