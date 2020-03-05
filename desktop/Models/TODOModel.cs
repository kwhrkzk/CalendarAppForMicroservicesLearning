using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bff.Idl;
using Cysharp.Diagnostics;
using Grpc.Core;
using MagicOnion.Client;
using Microservices.Calendar.Domain;
using Microservices.Todo.Domain;

namespace desktop.Models
{
    public class TODOModel
    {
        public ObservableCollection<やること> List { get; } = new ObservableCollection<やること>();

        public async Task Initialize()
        {
            // List.AddRange(consoleから取得());
            List.AddRange((await onionから取得()).Select(x => x.Toやること()));
        }

        private IEnumerable<やること> consoleから取得()
        {
           var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\", @"bff\consoleAPIGateway\bin\Debug\netcoreapp3.1\consoleAPIGateway.exe");
            System.Windows.MessageBox.Show(File.Exists(path).ToString());
            var t = ProcessX.StartAsync(path, "やること一覧", null, null, Encoding.UTF8).ToTask();
            t.Wait();
            return JsonSerializer.Deserialize<IEnumerable<やることPOCO>>(String.Join("", t.Result)).Select(item => item.Toやること());
        }

        private async Task<IEnumerable<やることPOCO>> onionから取得()
        {
             // standard gRPC channel
            var channel = new Channel("localhost", 12347, ChannelCredentials.Insecure);

            // get MagicOnion dynamic client proxy
            var client = MagicOnionClient.Create<IOnionAPIGatewayIDL>(channel);

            // call method.
            return await client.やること一覧();
        }
    }
}