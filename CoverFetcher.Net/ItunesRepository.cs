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
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.Proxy = null;
            clientHandler.UseProxy = false;

            client = new HttpClient(clientHandler);
            //client.MaxResponseContentBufferSize = 1000000; // 1MB

            formatters = new MediaTypeFormatterCollection();
            formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
        }

        public async Task<byte[]> FindCover(string artist, string album)
        {
            try
            {
                string url = string.Format("https://itunes.apple.com/search?entity=album&term={0}",
                    Uri.EscapeDataString(artist + " " + album));

                HttpResponseMessage response = await client.GetAsync(url);
                SearchResult result = await response.Content.ReadAsAsync<SearchResult>(formatters);

                if (result.ResultCount > 0)
                {
                    string imageUrl = result.Items.First().ArtworkUrl600;
                    return await client.GetByteArrayAsync(imageUrl);
                }
                else
                {
                    return null;
                }
            }
            catch(HttpRequestException ex)
            {
                throw new Exception("Unable to connect to the Apple Itunes REST service. Please unsure an internet connection is available.", ex);
            }
        }
    }
}
