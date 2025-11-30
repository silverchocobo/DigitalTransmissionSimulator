using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransmissionSimulator.Models
{
    public class Transmission
    {
         public string DecodedMessage { get; set; }
        // --- New Debug Properties ---
        public string Debug_RecoveredAmi { get; set; }
        public string Debug_RecoveredBinary { get; set; }
        public string? Message { get; set; }
        public string? Binary { get; private set; } 
        public string? AmiCode { get; private set; } 
        public string? AmiModulatedSignal { get; private set; }
        public List<int>? AmiCodeInt {get; private set; }
        public double[]? AmiModulatedSignalDouble { get; private set; }
        public double NoiseLevel { get; set; } = 0.0;
        // public string? ConversionResult
        // {
        //     get
        //     {
        //         if (Message != null)
        //         {
        //             ConvertMessageToBinary();
        //             return this.Binary;
        //         }
        //         else
        //         {
        //             return null;
        //         }
        //     }
        // }
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
            // First, ensure we have an AMI code to modulate.
            // if (this.AmiCode == null)
            // {
            //     EncodeBinaryToAmi();
            // }

            if (string.IsNullOrEmpty(this.AmiCode))
            {
                this.AmiModulatedSignal = null;
                return;
            }

            // Use the modulator to generate the signal and store it.
            double[] amiModulated = modulator.Modulate(this.AmiCodeInt);

            this.AmiModulatedSignalDouble = amiModulated;

            string resultString = string.Join(",", amiModulated);
            this.AmiModulatedSignal= resultString;
        }

        public void AddNoiseToSignal(AwgnGenerator noiseGenerator, double standardDeviation)
        {
            // Do nothing if there's no signal to add noise to, or if noise level is zero
            if (this.AmiModulatedSignal == null || standardDeviation <= 0)
            {
                return;
            }

            int sampleCount = this.AmiModulatedSignalDouble.Length;
            double[] noise = noiseGenerator.GenerateNoiseSignal(sampleCount, standardDeviation);

            // Add the noise to the signal, sample by sample
            for (int i = 0; i < sampleCount; i++)
            {
                this.AmiModulatedSignalDouble[i] += noise[i];
            }
        }
    }
}