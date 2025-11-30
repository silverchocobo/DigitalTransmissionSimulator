using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransmissionSimulator.Models
{
    public class Transmission
    {
        public string DecodedMessage { get; set; }
        public string DecodedMessageBpsk { get; set; }
        public string Debug_RecoveredBinaryBpsk { get; set; }
        public string Debug_RecoveredAmi { get; set; }
        public string Debug_RecoveredBinary { get; set; }
        public string? Message { get; set; }
        public string? Binary { get; private set; } 
        public string? AmiCode { get; private set; }
        public List<int>? NRZCode {get; private set;} 
        public string? AmiModulatedSignal { get; private set; }
        public List<int>? AmiCodeInt {get; private set; }
        public double[]? AmiModulatedSignalDouble { get; set; }
        public double[]? BpskSignal {get; set; }
        public double NoiseLevel { get; set; } = 0.0;
        public double Amplitude { get; set; } = 1.0;
        public string EncodingType { get; set; }
    
        public void EncodeToAmiBipolar ()
        {
                if (string.IsNullOrEmpty(this.Binary))
                {
                    this.AmiCode = null;
                }

                List<int> amiCode = new List<int>();

                int lastPolarity = 1;

                foreach (char bit in this.Binary)
                {
                   if (bit == '0')
                    {
                        amiCode.Add(0); 
                    }
                    else if (bit == '1')
                    {
                        amiCode.Add(lastPolarity); 
                        lastPolarity *= -1; // 
                    }
                    
                }

                this.AmiCodeInt = amiCode;
                string resultString = string.Join(",", amiCode);
                this.AmiCode = resultString;
            
        }

        public void EncodeToNRZPolar()
        {
            if(String.IsNullOrEmpty(this.Binary))
            {
                this.NRZCode = null;
            }

            List<int> NRZCode = new List<int>();

            foreach (char bit in this.Binary)
            {
                if (bit == '0')
                {
                    NRZCode.Add(-1);
                }
                else{
                    NRZCode.Add(1);
                }
            } 

            this.NRZCode = NRZCode;
        }

        public void ConvertMessageToBinary()
        {
            if (string.IsNullOrEmpty(Message))
            {
                this.Binary = null;
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(Message);

            string binaryString = string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

            this.Binary = binaryString;
        }

         public void ModulateAmiToSignal(AmiModulator modulator)
        {
            if (string.IsNullOrEmpty(this.AmiCode))
            {
                this.AmiModulatedSignal = null;
                return;
            }

            // Modula código AMI
            double[] amiModulated = modulator.Modulate(this.AmiCodeInt);

            this.AmiModulatedSignalDouble = amiModulated;
        }

        public double[] AddNoiseToSignal(double[] cleanSignal, AwgnGenerator noiseGenerator, double standardDeviation)
            {
                if (cleanSignal == null || standardDeviation <= 0)
                {
                    return cleanSignal;
                }

                int sampleCount = cleanSignal.Length;
                double[] noise = noiseGenerator.GenerateNoiseSignal(sampleCount, standardDeviation);
                var noisySignal = new double[sampleCount];

                // Adiciona ruído no sinal
                for (int i = 0; i < sampleCount; i++)
                {
                    noisySignal[i] = cleanSignal[i] + noise[i];
                }

                return noisySignal;
            }

        public void AddNoiseToBpskSignal(AwgnGenerator noiseGenerator, double standardDeviation)
        {
            if (this.BpskSignal == null || standardDeviation <= 0)
            {
                return;
            }

            int sampleCount = this.BpskSignal.Length;
            double[] noise = noiseGenerator.GenerateNoiseSignal(sampleCount, standardDeviation);

            // Adiciona ruído no sinal
            for (int i = 0; i < sampleCount; i++)
            {
                this.BpskSignal[i] += noise[i];
            }
        }

        public void ModulateNrzToBpsk(BpskModulator modulator)
        {
            if (this.NRZCode == null)
            {
                this.BpskSignal = null;
                return;
            }
            this.BpskSignal = modulator.Modulate(this.NRZCode);
        }
    }
}