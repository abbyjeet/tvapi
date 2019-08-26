using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace tv.api.Common.Constants
{
    public static class VKapi
    {
        public const string SECRET = "bcb959661b3be4613c1d380b3c29981e8b1ed868762af37d8201f4f9ff73";
        public const string APPID = "100005a";
        public const string KEY = "MM_d*yP@`&1@]@!AVrXf_o-HVEnoTnm$O-ti4[G~$JDI/Dc-&piU&z&5.;:}95=Iad";
        public const string USERAGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36"; // Not used

        private static string ApiGetSection(string url, string query)
        {
            var getQuery = $"{URL.VK}/v4/{url}?app={APPID}&t={Misc.CurrentTimeStamp}&sig={SECRET}";

            if (!string.IsNullOrWhiteSpace(query))
            {
                getQuery += $"&{query}";
            }

            return getQuery;
        }

        public static string ApiListShows(string query) => ApiGetSection("series.json", query);

        //Not required
        //public static string ApiShowDetails(string showId) => $"";

        public static string ApiEpisodesForSeason(string seriesid, string query) => ApiGetSection($"series/{seriesid}/episodes.json", query);

        public static string ApiEpisodeById(string episodeId) {
            var timestamp = Misc.CurrentTimeStamp.ToString().Substring(0, 10);

            var rawtxt = $"/v5/videos/{episodeId}/streams.json?app={APPID}&t={timestamp}";

            var hashed = HashHmacSHA1(rawtxt, KEY);

            return $"{URL.VK}{rawtxt}&sig={hashed}";
        }

        public static string ApiEpisodeSubtitleById(string episodeId)
        {
            var timestamp = Misc.CurrentTimeStamp.ToString().Substring(0, 10);

            var rawtxt = $"/v4/videos/{episodeId}/subtitles/en.srt?app={APPID}&t={timestamp}";

            var hashed = HashHmacSHA1(rawtxt, KEY);

            return $"{URL.VK}{rawtxt}&sig={hashed}";
        }

        private static string HashHmacSHA1(string text, string key)
        {
            var myEncoder = new System.Text.UTF8Encoding();
            var bKey = myEncoder.GetBytes(key);
            var bText = myEncoder.GetBytes(text);
            
            using (HMACSHA1 hasher = new HMACSHA1(bKey))
            {
                var hashedByteText = hasher.ComputeHash(bText);
                var hashedText = BitConverter.ToString(hashedByteText).Replace("-", "");
                return hashedText.ToLower();
            }
        }
    }
}
