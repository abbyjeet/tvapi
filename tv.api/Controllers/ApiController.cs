using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tv.api.Sources;

namespace tv.api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        //private readonly ISource df;
        private readonly SourceResolver source;

        public ApiController(SourceResolver source)
        {
            this.source = source;
            //this.df = source("df");
        }

        [HttpGet("{src}/{encurl}")]
        public JsonResult GetChannels(string src, string encurl)
        {
            return new JsonResult(source(src).GetChannels(HttpUtility.UrlDecode(encurl)));
        }

        [HttpGet("{src}/c/{encurl}")]
        public JsonResult GetSource(string src, string encurl)
        {
            return new JsonResult(source(src).GetSource(HttpUtility.UrlDecode(encurl)));
        }

        [HttpGet("{src}/s/{encurl}")]
        public JsonResult GetShows(string src, string encurl)
        {
            return new JsonResult(source(src).GetShows(HttpUtility.UrlDecode(encurl)));
        }

        [HttpGet("{src}/e/{encurl}")]
        public JsonResult GetEpisodes(string src, string encurl)
        {
            return new JsonResult(source(src).GetEpisodes(HttpUtility.UrlDecode(encurl)));
        }

        [HttpGet("{src}/p/{encurl}")]
        public JsonResult GetPlayData(string src, string encurl)
        {
            return new JsonResult(source(src).GetPlayData(HttpUtility.UrlDecode(encurl)));
        }
    }

}