using System;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Diagnostics;
using Microservices.Calendar.Domain;
using Cysharp.Text;
using System.Text;
using System.Threading.Tasks;
using MagicOnion.Client;
using Grpc.Core;
using Bff.Idl;

namespace desktop.Models
{
    public class CalendarModel
    {
        public ObservableCollection<DateModel> CalendarList { get; } = new ObservableCollection<DateModel>();
        public CalendarModel()
        {
        }

        public async Task Initialize()
        {
            var list = (await onionから取得()).Select(x => x.Toその日());
            // var list = consoleから取得();
            // var list = new List<その日>();
            var date = new DateTime(2020,2,1);
            CalendarList.AddRange(
                            Enumerable.Range(1, 日時.Create(date).日数)
                            .Select(day => ToDateModel(list, date.Year, date.Month, day))
                        );
        }

        private DateModel ToDateModel(IEnumerable<その日> list, int year, int month, int day)
        {
            var model = new DateModel(new DateTime(year, month, day));
            model.その日 = list.FirstOrDefault(model.その日と合ってる);
            return model;
        }

        private IEnumerable<その日> consoleから取得()
        {
           var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\", @"bff\consoleAPIGateway\bin\Debug\netcoreapp3.1\consoleAPIGateway.exe");
            System.Windows.MessageBox.Show(File.Exists(path).ToString());
            var t = ProcessX.StartAsync(path, "こみこみ一覧", null, null, Encoding.UTF8).ToTask();
            t.Wait();
            return JsonSerializer.Deserialize<IEnumerable<その日POCO>>(String.Join("", t.Result)).Select(item => item.Toその日());
        }

        private async Task<IEnumerable<その日POCO>> onionから取得()
        {
             // standard gRPC channel
            var channel = new Channel("localhost", 12347, ChannelCredentials.Insecure);

            // get MagicOnion dynamic client proxy
            var client = MagicOnionClient.Create<IOnionAPIGatewayIDL>(channel);

            // call method.
            return await client.こみこみ一覧();
        }
    }
}