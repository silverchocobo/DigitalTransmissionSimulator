using System;

namespace TransmissionSimulator.Models
{
    public class AwgnGenerator
    {
        private readonly Random _random;
        private bool _hasSpareSample;
        private double _spareSample;

        public AwgnGenerator()
        {
            // Create one instance of Random and reuse it.
            _random = new Random();
            _hasSpareSample = false;
        }

        /// <summary>
        /// Generates a single random sample from a Gaussian distribution.
        /// </summary>
        /// <param name="mean">The mean (μ) of the distribution (usually 0 for noise).</param>
        /// <param name="stdDev">The standard deviation (σ) of the distribution. This controls the noise power.</param>
        /// <returns>A single double representing a noise sample.</returns>
        public double GenerateGaussianSample(double mean = 0, double stdDev = 1.0)
        {
            // The Box-Muller transform generates two independent standard normal samples.
            // We return one and save the other for the next call, which is more efficient.
            if (_hasSpareSample)
            {
                _hasSpareSample = false;
                return _spareSample * stdDev + mean;
            }

            double u1, u2;
            do
            {
                u1 = 2.0 * _random.NextDouble() - 1.0; // Uniform randoms in [-1, 1]
                u2 = 2.0 * _random.NextDouble() - 1.0;
            }
            while (u1 * u1 + u2 * u2 >= 1.0); // Ensure they are in the unit circle

            double logTerm = Math.Sqrt(-2.0 * Math.Log(u1 * u1 + u2 * u2));
            
            double sample1 = u1 * logTerm;
            double sample2 = u2 * logTerm;

            // Save the second sample for the next call
            _spareSample = sample2;
            _hasSpareSample = true;

            return sample1 * stdDev + mean;
        }

        /// <summary>
        /// Generates an array of Gaussian noise samples.
        /// </summary>
        /// <param name="sampleCount">The number of noise samples to generate.</param>
        /// <param name="standardDeviation">The standard deviation (power) of the noise.</param>
        /// <returns>An array of double values representing the noise signal.</returns>
        public double[] GenerateNoiseSignal(int sampleCount, double standardDeviation)
        {
            var noise = new double[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                noise[i] = GenerateGaussianSample(0, standardDeviation);
            }
            return noise;
        }
    }
}