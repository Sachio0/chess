using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Newtonsoft.Json;
using System.Web;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Api.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Autor Sławomir Nowiński \n " +
                                  "Aplikacja wykorzystuje publiczne API chessboard.js działające na licencji MIT. \n";
            ViewData["Instrukcja"] = "Aplikacja składa się z 2 warstw po stronie klijęta, edycyjnej oraz grywalnej. \n" +
                                     "Warstwa edycyjna polega na możliwości edycji mapy wg. własnych potrzeb oraz " +
                                     "w celu późniejszego rozegrania partji możliwości zapisu. \n" +
                                     "Warstwa gry daje możliwość rozgrania gry w szachy zapisu tej gry do pliku oraz odczytania wcześniej zapisanej gry";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Game(Board b)
        {
            HttpContext.Session.Clear();
            //Choser.DeleteFile();
            ViewData["pos"] = HttpContext.Request.Cookies["pos"];
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetData(Board board)
        {
            Set("pos", board.Possiotion, 2);


            return View("Game");
        }
        public void Set(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddSeconds(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            Response.Cookies.Append(key, value, option);
        }

        [HttpPost]
        public IActionResult Ia([FromBody]Board board)
        {
            Choser choser;
            if (board.turn == 'w') choser = new Choser(Response.HttpContext, "Game2.xml", board.turn);
            else choser = new Choser(Response.HttpContext, "Game1.xml", board.turn);
            return Json(choser.makeRandomeMove(board));
        }
        public async Task<IActionResult> DownloadIA()
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream("game.xml", FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            return File(memory, Path.GetFileName(@"F:\chess\Api\Api\Game.xml"));
        }
        //public async Task<IActionResult> Download(string filename)
        //{
        //    if (filename == null)
        //        return Content("filename not present");

        //    var path = Path.Combine(
        //                   Directory.GetCurrentDirectory(),
        //                   "wwwroot", filename);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(path, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    return File(memory, GetContentType(path), Path.GetFileName(path));
        //}

        //public async Task<IActionResult> UploadFile()
        //{
        //    if (file == null || file.Length == 0)
        //        return Content("file not selected");

        //    var path = Path.Combine(
        //                Directory.GetCurrentDirectory(), "wwwroot",
        //                file.FileName);

        //    using (var stream = new FileStream(path, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    return RedirectToAction("Files");
        //}
    }
}
