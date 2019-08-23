using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            return new JsonResult(source(src).GetChannels(WebUtility.UrlDecode(encurl)));
        }

        [HttpGet("{src}/s/{encurl}")]
        public JsonResult GetShows(string src, string encurl)
        {
            return new JsonResult(source(src).GetShows(WebUtility.UrlDecode(encurl)));
        }

        [HttpGet("{src}/e/{encurl}")]
        public JsonResult GetEpisodes(string src, string encurl)
        {
            return new JsonResult(source(src).GetEpisodes(WebUtility.UrlDecode(encurl)));
        }

        [HttpGet("{src}/p/{encurl}")]
        public JsonResult GetPlayData(string src, string encurl)
        {
            return new JsonResult(source(src).GetPlayData(WebUtility.UrlDecode(encurl)));
        }
    }

}