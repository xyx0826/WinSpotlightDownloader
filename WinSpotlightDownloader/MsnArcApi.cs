using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WinSpotlightDownloader
{
    class MsnArcApi
    {
        /// <summary>
        /// Consumes MsnArc API to retrieve a JSON object of Windows Spotlight images.
        /// </summary>
        /// <returns>A JSON object containing another JSON object which contains image info.</returns>
        public async Task<string> GetBatchResponseAsync()
        {
            var batchQuery = HttpUtility.ParseQueryString(String.Empty);
            batchQuery["pid"] = "338387";
            batchQuery["fmt"] = "json";
            batchQuery["cfmt"] = "poly";
            batchQuery["sft"] = "jpeg";
            batchQuery["ctry"] = "us";
            batchQuery["pl"] = "en-US";
            batchQuery["cdm"] = "1";

            using (var client = new HttpClient())
            {
                string responseUrl = batchQuery.ToString();
                var responseMsg = await client.GetAsync("https://arc.msn.com/v3/Delivery/Placement?" + batchQuery.ToString());
                return await responseMsg.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Extracts image info from nested batch response JSON object.
        /// </summary>
        /// <param name="batchResponse">Batch response JSON object, ideally from GetbatchResponseAsync().</param>
        /// <returns>A ImageInfo containing info of landscape and portrait spotlight images.</returns>
        public ImageInfos GetImageInfo(string batchResponse)
        {
            var batchParsed = JObject.Parse(batchResponse);
            var singleItem = batchParsed.SelectToken("batchrsp.items[0].item").ToString();

            var itemParsed = JObject.Parse(singleItem);
            return itemParsed.SelectToken("ad").ToObject<ImageInfos>();
        }

        /// <summary>
        /// Downloads a image and write it to working directory and return its filename.
        /// </summary>
        /// <param name="imageUrl">Image URL to download.</param>
        /// <returns>The file name of the downloaded image.</returns>
        public async Task<string> WriteImage(string imageUrl)
        {
            string imageName = imageUrl.Split('/').Last().Split('?')[0];
            using (var client = new HttpClient())
            {
                var imageResponse = await client.GetAsync(imageUrl);
                System.IO.File.WriteAllBytes(
                    $"./{imageName}.jpg",
                    await imageResponse.Content.ReadAsByteArrayAsync());
            }
            return imageName;
        }

        #region ImageInfo object
        public class ImageInfos
        {
            private ImageInfo[] _infos = new ImageInfo[2];
            
            [JsonProperty("image_fullscreen_001_landscape")]
            public ImageInfo Landscape { get { return _infos[0]; } set { _infos[0] = value; } }
            [JsonProperty("image_fullscreen_001_portrait")]
            public ImageInfo Portrait { get { return _infos[1]; } set { _infos[1] = value; } }
        }

        public class ImageInfo
        {
            [JsonProperty("w")]
            public string Width { get; set; }
            [JsonProperty("h")]
            public string Height { get; set; }
            [JsonProperty("fileSize")]
            public string Size { get; set; }
            [JsonProperty("sha256")]
            public string Sha256 { get; set; }
            [JsonProperty("u")]
            public string Url { get; set; }
        }
        #endregion
    }
}
