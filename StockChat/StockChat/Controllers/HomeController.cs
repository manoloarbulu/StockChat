using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockChat.Configuration;
using StockChat.Models;

namespace StockChat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrWhiteSpace(ChatConfiguration.ChatUrl))
                ChatConfiguration.ChatUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return View();
        }

        [Authorize]
        public IActionResult Chat(){ 
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
