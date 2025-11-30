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
                throw new ArgumentException("Sampling rate precisa ser 4x maior que a crrier frequency para bons resultados");
            }
            SamplingRate = samplingRate;
        }

        //Modula a o código AMI (com 0, + e -) para uma carrier wave, retornará um array de doubles representando a amplitude do sinal modulado.
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

            // Itera sobre as integrais da lsita AMI code
            foreach (int symbol in amiCode)
            {
                // 'currentLevel' é o símbolo (1, -1, ou 0)
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