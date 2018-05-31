using System;

namespace WinSpotlightDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            MsnArcApi _api = new MsnArcApi();
            var response = _api.GetBatchResponseAsync();
            var images = _api.GetImageInfo(response.Result);

            // Writes image files and print their names.
            // Output format: `Landscape ([fileName]) Sha256: [fileHash]`
            Console.WriteLine($"Landscape ({_api.WriteImage(images.Landscape.Url).Result}) Sha256: {images.Landscape.Sha256}");
            Console.WriteLine($"Portrait ({_api.WriteImage(images.Portrait.Url).Result}) Sha256: {images.Portrait.Sha256}");
            Console.Read();
        }
    }
}
