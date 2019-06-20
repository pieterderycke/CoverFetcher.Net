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

        public async Task<IList<byte[]>> FindCovers(string artist, string album, string countryCode)
        {
            try
            {
                string url = string.Format("https://itunes.apple.com/search?entity=album&term={0}&country={1}",
                    Uri.EscapeDataString(artist + " " + album), countryCode);

                HttpResponseMessage response = await client.GetAsync(url);
                SearchResult result = await response.Content.ReadAsAsync<SearchResult>(formatters);

                IList<byte[]> covers = new List<byte[]>();

                foreach (SearchResultItem resultItem in result.Items)
                {
                    string imageUrl = resultItem.ArtworkUrl600;
                    covers.Add(await client.GetByteArrayAsync(imageUrl));
                }

                return covers;
            }
            catch(HttpRequestException ex)
            {
                throw new Exception("Unable to connect to the Apple Itunes REST service. Please unsure an internet connection is available.", ex);
            }
        }
    }
}
