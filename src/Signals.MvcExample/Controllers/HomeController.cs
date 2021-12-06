using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Signals.MvcExample.Data;
using Signals.MvcExample.Models;
using Signals.MvcExample.Signals;
using Signals.Processor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Signals.MvcExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataRepository dataRepository;
        private readonly ISignalProcessor signalProcessor;

        public HomeController(IDataRepository dataRepository, ISignalProcessor signalProcessor)
        {
            this.dataRepository = dataRepository;
            this.signalProcessor = signalProcessor;
        }

        public IActionResult Index()
        {
            var model = dataRepository.Books;
            return View(model);
        }

        public async Task<IActionResult> Price(int id)
        {
            var signal = await signalProcessor.Process(new BookPriceSignal(id));
            return View(signal);
        }

        public IActionResult WorkFlows()
        {
            var workFlow = signalProcessor.WorkFlows;
            return View(workFlow);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
