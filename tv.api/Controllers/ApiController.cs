using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tv.api.Common.Constants;
using tv.api.Sources;

namespace tv.api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly SourceResolver source;

        public ApiController(SourceResolver source)
        {
            this.source = source;
        }


        [HttpGet]
        public JsonResult GetSources()
        {
            return new JsonResult(Misc.Sources);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="encurl">optional - nothing by default</param>
        /// <returns></returns>
        [HttpGet("{src}/{encurl?}")]
        public JsonResult GetChannels(string src, string encurl = "")
        {
            return new JsonResult(source(src).GetChannels(HttpUtility.UrlDecode(encurl)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="encurl"></param>
        /// <returns></returns>
        [HttpGet("{src}/c/{encurl?}")]
        public JsonResult GetSource(string src, string encurl)
        {
            return new JsonResult(source(src).GetSource(HttpUtility.UrlDecode(encurl)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="encurl">{link}&p={page}</param>
        /// <returns></returns>
        [HttpGet("{src}/s/{encurl}")]
        public JsonResult GetShows(string src, string encurl)
        {
            return new JsonResult(source(src).GetShows(HttpUtility.UrlDecode(encurl)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="encurl">{link}&p={page}</param>
        /// <returns></returns>
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