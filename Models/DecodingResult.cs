// Add this class in a new file, or inside Receiver.cs
namespace TransmissionSimulator.Models
{
    public class DecodingResult
    {
        public string DecodedMessage { get; set; }
        public string RecoveredAmi { get; set; }
        public string RecoveredBinary { get; set; }
    }
}