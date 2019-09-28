using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using ImageServer;

namespace Client
{
    class Program
    {
        private static readonly string[] urls = {
            "https://assets.amuniversal.com/214746b0b6d10137bcb6005056a9545d",
            "https://assets.amuniversal.com/1f0ce490b6d10137bcb6005056a9545d",
            "https://assets.amuniversal.com/1d0d2800b6d10137bcb6005056a9545d",
            "https://assets.amuniversal.com/1acc0e30b6d10137bcb6005056a9545d",
            "https://assets.amuniversal.com/18584530b6d10137bcb6005056a9545d",
            "https://assets.amuniversal.com/463335109a090137b296005056a9545d",
            "https://assets.amuniversal.com/5d0dcc40b1440137ba94005056a9545d",
        };

        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ImageSvc.ImageSvcClient(channel);

            Console.WriteLine("Press any key to start downloading images");
            Console.ReadKey();

            await ProcessMesages(client).ConfigureAwait(false);

            Console.WriteLine("Shutting down");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task ProcessMesages(ImageSvc.ImageSvcClient client)
        {
            //var cts = new CancellationTokenSource();
            //cts.CancelAfter(TimeSpan.FromSeconds(10));
            //using var call = client.Download(cancellationToken: cts.Token);

            using var call = client.Download();

            int counter = 0;
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"Received content for {message.Url}. Length: {message.Content.Length}");

                    Interlocked.Increment(ref counter);

                    Directory.CreateDirectory("Images");
                    File.WriteAllBytes(Path.Combine("Images", $"img{counter}.gif"), message.Content.ToByteArray());
                }
            });

            foreach (var url in urls)
            {
                await call.RequestStream.WriteAsync(new ImageRequest() { Url = url }).ConfigureAwait(false);
            }

            await call.RequestStream.CompleteAsync().ConfigureAwait(false);
            await readTask.ConfigureAwait(false);
        }
    }
}
