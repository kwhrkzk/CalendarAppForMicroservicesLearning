using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microservices.Todo.Domain;
using Microservices.Todo.Infra;
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
                services.AddSingleton<Iやることリポジトリ, やることリポジトリ>();
            })
            .RunConsoleAppFrameworkAsync<Main>(args);
        }
    }

    public class Main : ConsoleAppBase
    {
        private Iやることリポジトリ やることリポジトリ { get; }

        public Main(Iやることリポジトリ _やることリポジトリ)
        {
            やることリポジトリ = _やることリポジトリ;
        }

        public void 一覧()
        {
            var 一覧 = やることリポジトリ.一覧().一覧;
            var bytes = JsonSerializer.SerializeToUtf8Bytes<List<やることPOCO>>(一覧.Select(x => x.ToやることPOCO()).ToList());
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
        }
    }
}
