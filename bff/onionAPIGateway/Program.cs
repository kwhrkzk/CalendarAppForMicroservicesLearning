using System;
using Grpc.Core;
using MagicOnion.Server;

namespace onionAPIGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            // setup MagicOnion and option.
            var service = MagicOnionEngine.BuildServerServiceDefinition(isReturnExceptionStackTraceInErrorDetail: true);

            var server = new global::Grpc.Core.Server
            {
                Services = { service },
                Ports = { new ServerPort("localhost", 12347, ServerCredentials.Insecure) }
            };
            
            // launch gRPC Server.
            server.Start();

            // and wait.
            Console.ReadLine();
        }
    }
}
