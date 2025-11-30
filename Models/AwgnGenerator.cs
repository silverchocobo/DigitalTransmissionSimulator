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
            _random = new Random();
            _hasSpareSample = false;
        }

        // Solução Box-Muller
        public double GenerateGaussianSample(double mean = 0, double stdDev = 1.0)
        {     
            if (_hasSpareSample)
            {
                _hasSpareSample = false;
                return _spareSample * stdDev + mean;
            }

            double u1, u2;
            do
            {
                u1 = 2.0 * _random.NextDouble() - 1.0; 
                u2 = 2.0 * _random.NextDouble() - 1.0;
            }
            while (u1 * u1 + u2 * u2 >= 1.0); 

            double logTerm = Math.Sqrt(-2.0 * Math.Log(u1 * u1 + u2 * u2));
            
            double sample1 = u1 * logTerm;
            double sample2 = u2 * logTerm;

            _spareSample = sample2;
            _hasSpareSample = true;

            return sample1 * stdDev + mean;
        }

        /// <summary>
        /// Array the ruído Gaussiano.
        /// </summary>
        /// <param name="sampleCount"Número de samples de ruído.</param>
        /// <param name="standardDeviation">Desvio padrão (força) do ruído.</param>
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