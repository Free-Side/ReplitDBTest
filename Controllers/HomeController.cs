using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReplitDbClient;
using ReplitDbTest.Models;

namespace ReplitDbTest.Controllers {
    public class HomeController : Controller {
        private readonly IReplitDbClient database;
        private readonly ILogger<HomeController> logger;

        public HomeController(
            IReplitDbClient database,
            ILogger<HomeController> logger) {
            this.database = database;
            this.logger = logger;
        }

        public async Task<IActionResult> Index() {
            return this.View(new HomeViewModel {
                Data = (await this.database.GetAll()).ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value
                )
            });
        }

        [HttpPost]
        public async Task<IActionResult> Set([FromForm] String key, [FromForm] String value) {
            await this.database.Set(key, value);

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] String id) {
            await this.database.Delete(id);

            return this.RedirectToAction(nameof(this.Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return this.View(new ErrorViewModel {
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier
            });
        }
    }
}
