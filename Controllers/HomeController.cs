using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TransmissionSimulator.Models;

namespace TransmissionSimulator.Controllers;

public class HomeController : Controller
{

    private static readonly AwgnGenerator _noiseGenerator = new AwgnGenerator();

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new Transmission());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Index(Transmission model)
    {
        model.ConvertMessageToBinary();
        double signalAmplitude = model.Amplitude;

        if (model.EncodingType == "ami"){
            model.EncodeToAmiBipolar();

            var amiModulator = new AmiModulator(signalAmplitude, 1000, 100, 8000);
            model.ModulateAmiToSignal(amiModulator);

            double[] noisyAmiSignal = model.AddNoiseToSignal(model.AmiModulatedSignalDouble, _noiseGenerator, model.NoiseLevel);
            model.AmiModulatedSignalDouble = noisyAmiSignal; 

            var amiReceiver = new Receiver(1000, 100, 8000, signalAmplitude);
            var amiResult = amiReceiver.DemodulateAndDecode(model.AmiModulatedSignalDouble);
            
            model.DecodedMessage = amiResult.DecodedMessage;
            model.RecoveredAmi = amiResult.RecoveredAmi;
            model.RecoveredBinary = amiResult.RecoveredBinary;

            model.CalculateHammingDistance(model.RecoveredBinary);
        }
        
        else if (model.EncodingType == "bpsk"){
            model.EncodeToNRZPolar();
            var bpskModulator = new BpskModulator(signalAmplitude, 1000, 100, 8000);
            model.ModulateNrzToBpsk(bpskModulator);

            double[] noisyBpskSignal = model.AddNoiseToSignal(model.BpskSignal, _noiseGenerator, model.NoiseLevel);
            model.BpskSignal = noisyBpskSignal; 

            var bpskReceiver = new BpskReceiver(1000, 100, 8000);
            var bpskResult = bpskReceiver.DemodulateAndDecode(model.BpskSignal);
            
            model.DecodedMessageBpsk = bpskResult.DecodedMessage;
            model.RecoveredBinaryBpsk = bpskResult.RecoveredBinary;

            model.CalculateHammingDistance(model.RecoveredBinaryBpsk);
            }
            
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
