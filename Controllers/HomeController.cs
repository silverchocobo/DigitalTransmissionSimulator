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

        // 1. Call the methods to process the data in the correct order
            model.ConvertMessageToBinary();
            model.EncodeToAmiBipolar();

            double signalAmplitude = 5.0;

            // 2. Create the modulator and generate the signal
            // This logic is better handled in the controller, not the view.
            var amiModulator = new AmiModulator(
                amplitude: signalAmplitude,
                carrierFrequency: 1000,
                bitRate: 100,
                samplingRate: 8000
            );
            model.ModulateAmiToSignal(amiModulator);

            model.AddNoiseToSignal(_noiseGenerator, model.NoiseLevel);

            var receiver = new Receiver(
            amplitude: signalAmplitude,
            carrierFrequency: 1000,
            bitRate: 100,
            samplingRate: 8000
            );

                // Call the updated method
            DecodingResult result = receiver.DemodulateAndDecode(model.AmiModulatedSignalDouble);
            
            // Populate the model with all the results
            model.DecodedMessage = result.DecodedMessage;
            model.Debug_RecoveredAmi = result.RecoveredAmi;
            model.Debug_RecoveredBinary = result.RecoveredBinary;
 
            return View(model);
        }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
