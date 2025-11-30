using System;
using System.Collections.Generic;
using System.Text;

namespace TransmissionSimulator.Models
{
    public class BpskReceiver
    {
        public double CarrierFrequency { get; }
        public int BitRate { get; }
        public int SamplingRate { get; }

        public BpskReceiver(double carrierFrequency, int bitRate, int samplingRate)
        {
            CarrierFrequency = carrierFrequency;
            BitRate = bitRate;
            SamplingRate = samplingRate;
        }

        public DecodingResult DemodulateAndDecode(double[] noisyBpskSignal)
        {
            var result = new DecodingResult
            {
                DecodedMessage = "[Decodificação falhou",
                RecoveredAmi = null,
                RecoveredBinary = ""
            };

            if (noisyBpskSignal == null || noisyBpskSignal.Length == 0) return result;

            var recoveredBinaryBuilder = new StringBuilder();
            int samplesPerBit = (int)(SamplingRate / (double)BitRate);

            for (int i = 0; i < noisyBpskSignal.Length; i += samplesPerBit)
            {
                double integratedEnergy = 0;

                // Itera sobre um tempo de bit
                for (int j = 0; j < samplesPerBit && (i + j) < noisyBpskSignal.Length; j++)
                {
                    int sampleIndex = i + j;
                    double time = (double)sampleIndex / SamplingRate;
                    
                    // Multiplica pela carrier wave
                    double localCarrier = Math.Cos(2 * Math.PI * CarrierFrequency * time);
                    integratedEnergy += noisyBpskSignal[sampleIndex] * localCarrier;
                }

                if (integratedEnergy > 0)
                {
                    recoveredBinaryBuilder.Append('1');
                }
                else
                {
                    recoveredBinaryBuilder.Append('0');
                }
            }
            
            result.RecoveredBinary = recoveredBinaryBuilder.ToString();

            // Binário para texto
            try
            {
                var byteList = new List<byte>();
                for (int k = 0; k < result.RecoveredBinary.Length; k += 8)
                {
                    if (k + 8 <= result.RecoveredBinary.Length)
                    {
                        string byteString = result.RecoveredBinary.Substring(k, 8);
                        byteList.Add(Convert.ToByte(byteString, 2));
                    }
                }
                result.DecodedMessage = Encoding.UTF8.GetString(byteList.ToArray());
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
    }
}