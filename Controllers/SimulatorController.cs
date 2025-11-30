using TransmissionSimulator.Models;
using Microsoft.AspNetCore.Mvc;

namespace TransmissionSimulator.Controllers
{
    public class SimulatorController : Controller
    {
        public ActionResult Index()
        {
            return View(new Transmission());
        }

        [HttpPost]
        public ActionResult Index(Transmission model)
        {
            // if (model != null)
            // {
            //     model.Convert();
            // }
            return View(model);
        }
    }
}

