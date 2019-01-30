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
        [HttpPost]
        public IActionResult Reset()
        {
            HttpContext.Session.Clear();
            return Json("sucesfull");
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
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                ViewData["Upload"] = "fail";
                return View("Game", "Fail");
            }
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Game1.xml");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            ViewData["Upload"] = "wysłane";
            return View("Game", "wysłane");
        }
        //public FileResult Download(string id)
        //{
        //    string filename = @"F:\chess\Api\Api\Game1.xml";
        //    string contentType = "application/xml";
        //    //Parameters to file are
        //    //1. The File Path on the File Server
        //    //2. The content type MIME type
        //    //3. The parameter for the file save by the browser
        //    return File(filename, contentType, "Game1.xml");
        //}
        [HttpGet]
        public FileStreamResult Download()
        {
            return new FileStreamResult(new FileStream(Path.Combine(Directory.GetCurrentDirectory(),"Game1.xml" ), FileMode.Open), "text/xml");
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
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
