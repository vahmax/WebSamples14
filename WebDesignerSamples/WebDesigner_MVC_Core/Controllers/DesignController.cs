﻿using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace WebDesignerMvcCore.Controllers
{
	[Route("/")]
	public class DesignController : Controller
	{

		[HttpGet]
		public ActionResult Index([FromQuery(Name = "theme")] string theme = "blue")
		{
			return RedirectToAction("create", new { theme });
		}

		[HttpGet("create")]
		public ActionResult Create([FromQuery(Name = "theme")] string theme = "blue")
		{
			ViewBag.Theme = theme;
			return View("Index");
		}

		[HttpGet("edit/{id}")]
		public ActionResult Edit([FromRoute] string id, [FromQuery(Name = "theme")] string theme = "blue")
		{
			if (string.IsNullOrWhiteSpace(id)) return BadRequest();
			ViewBag.Id = id;
			ViewBag.Theme = theme;
			return View("Index");
		}

		[HttpGet("{file}")]
		public object Resource(string file)
		{
			var stream = GetType().Assembly.GetManifestResourceStream("JSViewer_MVC_Core.wwwroot." + file);
			if (stream == null)
				return new NotFoundResult();

			if (Path.GetExtension(file) == ".html")
				return new ContentResult() { Content = new StreamReader(stream).ReadToEnd(), ContentType = "text/html" };

			if (Path.GetExtension(file) == ".ico")
				using (var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					return new FileContentResult(memoryStream.ToArray(), "image/x-icon") { FileDownloadName = file };
				}

			using (var streamReader = new StreamReader(stream))
				return new FileContentResult(System.Text.Encoding.UTF8.GetBytes(streamReader.ReadToEnd()),
					GetMimeType(file))
				{ FileDownloadName = file };
		}

		/// <summary>
		/// Gets the MIME type from the file extension
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <returns>MIME type</returns>
		private static string GetMimeType(string fileName)
		{
			if (fileName.EndsWith(".css"))
				return "text/css";

			if (fileName.EndsWith(".js"))
				return "text/javascript";

			return "text/html";
		}
	}
}
