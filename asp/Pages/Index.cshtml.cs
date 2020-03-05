using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bff.Idl;
using Grpc.Core;
using MagicOnion.Client;
using Microservices.Calendar.Domain;
using Microservices.Todo.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace asp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public string こみこみ一覧取得()
        {
            var t = こみこみ一覧をonionから取得();
            t.Wait();
            return System.Text.Json.JsonSerializer.Serialize(t.Result.SelectMany(x => x.ToFullCalendarPOCO()));
        }

        public string やること一覧取得()
        {
            var t = やること一覧をonionから取得();
            t.Wait();
            return System.Text.Json.JsonSerializer.Serialize(t.Result.Select(x => x.ToListJsPOCO()));
        }

        private async Task<IEnumerable<その日POCO>> こみこみ一覧をonionから取得()
        {
             // standard gRPC channel
            var channel = new Channel("localhost", 12347, ChannelCredentials.Insecure);

            // get MagicOnion dynamic client proxy
            var client = MagicOnionClient.Create<IOnionAPIGatewayIDL>(channel);

            // call method.
            return await client.こみこみ一覧();
        }

        private async Task<IEnumerable<やることPOCO>> やること一覧をonionから取得()
        {
             // standard gRPC channel
            var channel = new Channel("localhost", 12347, ChannelCredentials.Insecure);

            // get MagicOnion dynamic client proxy
            var client = MagicOnionClient.Create<IOnionAPIGatewayIDL>(channel);

            // call method.
            return await client.やること一覧();
        }
    }

    public class FullCalendarPOCO
    {
        public string title { get; set; }
        public string start { get; set; }
    }

    public static class その日POCOExtension
    {
        public static IEnumerable<FullCalendarPOCO> ToFullCalendarPOCO(this その日POCO p)
        {
            if (p.list == null)
                yield break;

            foreach (var item in p.list)
            {
                yield return new FullCalendarPOCO {
                    title = item.title,
                    start = p.date.ToString("yyyy-MM-dd")
                };
            }
        }
    }

    public class ListJsPOCO
    {
        public string title { get; set; }
    }

    public static class やることPOCOExtension
    {
        public static ListJsPOCO ToListJsPOCO(this やることPOCO p)
        {
            return new ListJsPOCO
            {
                title = p.title
            };
        }
    }
}
