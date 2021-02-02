using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace gRPCClient_csharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var chanel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Cities.CitiesClient(chanel);

            Console.WriteLine("List of cities");
            var dtIni = DateTime.Now;
            var count = 0;
            using (var call = client.GetCities(new CityRequest()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var ci = call.ResponseStream.Current;
                    Console.WriteLine($"{ci.Id} - {ci.Name}");
                    count ++;
                }
            }
            var dtEnd = DateTime.Now;
            Console.WriteLine($"Count: {count}");
            Console.WriteLine($"Ini time: {dtIni}");
            Console.WriteLine($"End time: {dtEnd}");
            Console.WriteLine($"Diff time: {dtEnd - dtIni}");
            
            
        }
    }
}