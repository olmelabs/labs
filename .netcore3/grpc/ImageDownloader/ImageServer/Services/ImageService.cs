using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace ImageServer
{
    public class ImageService : ImageSvc.ImageSvcBase
    {
        private readonly ILogger<ImageService> _logger;

        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
        }

        public override async Task Download(IAsyncStreamReader<ImageRequest> requestStream, IServerStreamWriter<ImageResponse> responseStream, ServerCallContext context)
        {
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    _queue.Enqueue(message.Url);
                    _logger.LogInformation($"Download request {message.Url}");
                }
            });

            await readTask.ConfigureAwait(false);
            await ProcessQueue(responseStream, context).ConfigureAwait(false);

        }

        private async Task ProcessQueue(IServerStreamWriter<ImageResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out string url))
                {
                    await Task.Delay(100).ConfigureAwait(false);

                    var stream = await DownloadAsync(new Uri(url)).ConfigureAwait(false);

                    _logger.LogInformation($"Downloaded {url}. Content Length: {stream.Length}");

                    await responseStream.WriteAsync(
                        new ImageResponse()
                        {
                            Url = url,
                            Content = await ByteString.FromStreamAsync(stream).ConfigureAwait(false)
                        }).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
        }

        private static async Task<Stream> DownloadAsync(Uri requestUri)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request).ConfigureAwait(false);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            stream.Position = 0;
            return stream;
        }
    }
}
