using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CoverFetcher
{
    public class ItunesRepository
    {
        private readonly HttpClient client;
        private readonly MediaTypeFormatterCollection formatters;

        public ItunesRepository()
        {
            client = new HttpClient();

            formatters = new MediaTypeFormatterCollection();
            formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
        }

        public async Task<BitmapImage> FindCover(string artist, string album)
        {
            string url = string.Format("https://itunes.apple.com/search?entity=album&term={0}",
                Uri.EscapeDataString(artist + " " + album));

            HttpResponseMessage response = await client.GetAsync(url);
            SearchResult result = await response.Content.ReadAsAsync<SearchResult>(formatters);

            if (result.ResultCount > 0)
            {
                string imageUrl = result.Items.First().ArtworkUrl600;
                return await LoadImage(imageUrl);
            }
            else
            {
                return null;
            }
        }

        private async Task<BitmapImage> LoadImage(string url)
        {
            WebRequest request = WebRequest.Create(new Uri(url, UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = await request.GetResponseAsync();
            Stream responseStream = response.GetResponseStream();
            BinaryReader reader = new BinaryReader(responseStream);
            MemoryStream memoryStream = new MemoryStream();

            byte[] bytebuffer = new byte[1024];
            int bytesRead = reader.Read(bytebuffer, 0, 1024);

            while (bytesRead > 0)
            {
                memoryStream.Write(bytebuffer, 0, bytesRead);
                bytesRead = reader.Read(bytebuffer, 0, 1024);
            }

            BitmapImage image = new BitmapImage();

            image.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);

            image.StreamSource = memoryStream;
            image.EndInit();

            return image;
        }
    }
}
