using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransmissionSimulator.Models
{
    public class Receiver
    {
        public double Amplitude { get; } 
        public double CarrierFrequency { get; }
        public int BitRate { get; }
        public int SamplingRate { get; }

        public Receiver(double carrierFrequency, int bitRate, int samplingRate, double amplitude)
            {
                Amplitude = amplitude;
                CarrierFrequency = carrierFrequency;
                BitRate = bitRate;
                SamplingRate = samplingRate;
            }

       public DecodingResult DemodulateAndDecode(double[] noisySignal)
        {
            var result = new DecodingResult
            {
                DecodedMessage = "[Decodificação falhou]",
                RecoveredAmi = "",
                RecoveredBinary = ""
            };

            if (noisySignal == null || noisySignal.Length == 0) return result;

            var recoveredAmiBuilder = new StringBuilder();
            int samplesPerSymbol = (int)(SamplingRate / (double)BitRate);
            double expectedEnergy = this.Amplitude * (samplesPerSymbol / 2.0);
            double threshold = expectedEnergy * 0.5;

            for (int i = 0; i < noisySignal.Length; i += samplesPerSymbol)
            {
                double integratedEnergy = 0;
                for (int j = 0; j < samplesPerSymbol && (i + j) < noisySignal.Length; j++)
                {
                    int sampleIndex = i + j;
                    double time = (double)sampleIndex / SamplingRate;
                    
                    double localCarrier = Math.Cos(2 * Math.PI * CarrierFrequency * time);
                    integratedEnergy += noisySignal[sampleIndex] * localCarrier;
                }

                if (integratedEnergy > threshold) recoveredAmiBuilder.Append('+');
                else if (integratedEnergy < -threshold) recoveredAmiBuilder.Append('-');
                else recoveredAmiBuilder.Append('0');
            }
            
            result.RecoveredAmi = recoveredAmiBuilder.ToString();

            var recoveredBinaryBuilder = new StringBuilder();
            foreach (char symbol in result.RecoveredAmi)
            {
                recoveredBinaryBuilder.Append((symbol == '0') ? '0' : '1');
            }
            result.RecoveredBinary = recoveredBinaryBuilder.ToString();

            try
            {
                var byteList = new List<byte>();
                for (int i = 0; i < result.RecoveredBinary.Length; i += 8)
                {
                    if (i + 8 <= result.RecoveredBinary.Length)
                    {
                        string byteString = result.RecoveredBinary.Substring(i, 8);
                        byteList.Add(Convert.ToByte(byteString, 2));
                    }
                }
                result.DecodedMessage = Encoding.UTF8.GetString(byteList.ToArray());
                return result;
            }
            catch { return result; }
        }
    }
}