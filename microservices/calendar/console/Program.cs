using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microservices.Calendar.Domain;
using Microservices.Calendar.Infra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Unity.Microsoft.DependencyInjection;

namespace console
{
    public class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            // target T as ConsoleAppBase.
            await Host
            .CreateDefaultBuilder()
            .UseUnityServiceProvider()
            .ConfigureServices(services => {
                services.AddSingleton<Iその日リポジトリ, その日リポジトリ>();
            })
            .RunConsoleAppFrameworkAsync<Main>(args);
        }
    }

    public class Main : ConsoleAppBase
    {
        private Iその日リポジトリ その日リポジトリ { get; }

        public Main(Iその日リポジトリ _その日リポジトリ)
        {
            その日リポジトリ = _その日リポジトリ;
        }

        public void 一覧()
        {
            var 一覧 = その日リポジトリ.一覧();
            var bytes = JsonSerializer.SerializeToUtf8Bytes<List<その日POCO>>(一覧.Select(x => x.Toその日POCO()).ToList());
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
        }
    }
}
