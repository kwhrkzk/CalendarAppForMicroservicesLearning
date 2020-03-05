using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Cysharp.Diagnostics;
using Microservices.Calendar.Domain;
using Microservices.Todo.Domain;
using Microsoft.Extensions.Hosting;

namespace consoleAPIGateway
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            // target T as ConsoleAppBase.
            await Host
            .CreateDefaultBuilder()
            .ConfigureServices(services => {
            })
            .RunConsoleAppFrameworkAsync<Program>(args);
        }

        [Command("その日一覧")]
        public void その日一覧()
        {
            var 一覧 = その日一覧取得();

            var bytes = JsonSerializer.SerializeToUtf8Bytes<List<その日POCO>>(一覧.Select(x => x.Toその日POCO()).ToList());
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
        }

        private IEnumerable<その日> その日一覧取得()
        {
           var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\", @"microservices\calendar\console\bin\Debug\netcoreapp3.1\console.exe");

            var t = ProcessX.StartAsync(path, null, null, Encoding.UTF8).ToTask();
            t.Wait();
            return JsonSerializer.Deserialize<IEnumerable<その日POCO>>(String.Join("", t.Result)).Select(item => item.Toその日());
        }

        [Command("やること一覧")]
        public void やること一覧取得()
        {
            var 一覧 = _やること一覧取得().一覧;

            var bytes = JsonSerializer.SerializeToUtf8Bytes<List<やることPOCO>>(一覧.Select(x => x.ToやることPOCO()).ToList());
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
        }

        private やること一覧 _やること一覧取得()
        {
           var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\", @"microservices\todo\console\bin\Debug\netcoreapp3.1\console.exe");

            var t = ProcessX.StartAsync(path, null, null, Encoding.UTF8).ToTask();
            t.Wait();
            return やること一覧.Create(JsonSerializer.Deserialize<IEnumerable<やることPOCO>>(String.Join("", t.Result)).Select(item => item.Toやること()));
        }

        [Command("こみこみ一覧")]
        public void その日の一覧にやることを追加した一覧()
        {
            var 完了一覧 = _やること一覧取得().完了一覧.ToList();

            var 一覧 = その日一覧取得().Select(item => {

                var やること = 完了一覧.FirstOrDefault(x => x.完了日yyyymmdd == item.yyyymmdd);
                if (やること == null)
                    return item;

                完了一覧.Remove(やること);
                return item.Add要件(要件.Create(やること.件名文字列, やること.内容文字列));
            }).ToList();

            foreach (var item in 完了一覧)
            {
                var ret = 一覧.FirstOrDefault(x => x.yyyymmdd == item.完了日yyyymmdd);
                if (ret == null)
                    一覧.Add(その日.Create(item.完了日DateTime.Value, new []{ 要件.Create(item.件名文字列, item.内容文字列) }));
                else
                    ret.Add要件(要件.Create(item.件名文字列, item.内容文字列));
            }
            var bytes = JsonSerializer.SerializeToUtf8Bytes<List<その日POCO>>(一覧.OrderBy(x => x.yyyymmdd).Select(x => x.Toその日POCO()).ToList());
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
        }
    }
}
