using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tv.api.Common.Constants
{
    /// <summary>
    /// get code from here
    /// https://b2bapi.zee5.com/front/countrylist.php?lang=en&ccode=AU
    /// manually created list of recent shows
    /// https://gwapi.zee5.com/content/collection/0-8-manualcol_1990967088?page=1&limit=5&item_limit=20&country=AU&translation=en&languages=mr&version=6
    /// https://catalogapi.zee5.com/v1/tvshow?sort_by_field=release_date&sort_order=DESC&page=1&page_size=24&genres=Action,Animation,Comedy,Cookery,Crime,Devotional,Docudrama,Drama,Entertainment,Fantasy,Horror,Infotainment,Kids,Lifestyle,Mystery,News,Reality,Romance,Sci-Fi%20%26%20Fantasy,Thriller,Wellness&languages=mr&country=AU&translation=en&asset_subtype=tvshow
    /// show details (get seasons from here)
    /// https://catalogapi.zee5.com/v1/tvshow/0-6-1924
    /// get recent 9 episodes from here (with urls)
    /// https://catalogapi.zee5.com/v1/season/0-2-1732?page=1&page_size=9&asset_subtype=episode
    /// user token
    /// https://useraction.zee5.com/token/platform_tokens.php?platform_name=web_app
    /// sample url
    /// https://zee5vodnd.akamaized.net/hls1/elemental/hls/ON_AIR/DOMESTIC/ZEE_MARATHI/Aug2019/22082019/Seamless/Agga_Bai_Sasubai_Ep28_Seamless_22082019_mr_6138dc5e2ab3b7b40899b9b32e3a4844/index.m3u8?hdnea=st=1566517097~exp=1566520097~acl=/*~hmac=2e7f0b4480a9a30348079006e6408102549e1c48aed031f97b7ba21a287fdcf8
    /// </summary>
    public static class Z5api
    {
        public const string REGIONAL_DETAILS = "https://b2bapi.zee5.com/front/countrylist.php?lang=en&ccode=AU";

        //public static string ApiManualCollection(string collectionId) => $"https://gwapi.zee5.com/content/collection/{collectionId}?page=1&limit=5&item_limit=20&country=AU&translation=en&languages=mr&version=6";

        //asset_subtype=tvshow&languages=mr&page=1&page_size=9
        public static string ApiListShows(string query) => $"https://catalogapi.zee5.com/v1/tvshow?sort_by_field=release_date&sort_order=DESC&genres=Action,Animation,Comedy,Cookery,Crime,Devotional,Docudrama,Drama,Entertainment,Fantasy,Horror,Infotainment,Kids,Lifestyle,Mystery,News,Reality,Romance,Sci-Fi%20%26%20Fantasy,Thriller,Wellness&country=AU&translation=en&{query}";
        public static string ApiShowDetails(string showId) => $"https://catalogapi.zee5.com/v1/tvshow/{showId}";

        //page=1&page_size=9&asset_subtype=episode
        public static string ApiEpisodesForSeason(string seasonId, string query) => $"https://catalogapi.zee5.com/v1/season/{seasonId}?{query}";

        public static string ApiEpisodeById(string episodeId) => $"https://catalogapi.zee5.com/v1/episode/{episodeId}";
    }
}
