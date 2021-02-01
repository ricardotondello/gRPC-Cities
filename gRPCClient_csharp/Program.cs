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
            var chanel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Cities.CitiesClient(chanel);

            Console.WriteLine("List of cities");
            using (var call = client.GetCities(new CityRequest()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var ci = call.ResponseStream.Current;
                    Console.WriteLine($"{ci.Id} - {ci.Name}");
                }
            }
        }
    }
}