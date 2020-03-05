using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Bff.Idl;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Server;
using Microservices.Calendar.Domain;
using Microservices.Todo.Domain;

namespace onionAPIGateway
{
    public class OnionAPIGatewayService : ServiceBase<IOnionAPIGatewayIDL>, IOnionAPIGatewayIDL
    {
        public async UnaryResult<IEnumerable<その日POCO>> こみこみ一覧()
        {
            var 完了一覧 = Microservices.Todo.Domain.やること一覧.Create((await やること一覧()).Select(x => x.Toやること())).完了一覧.ToList();

            var 一覧 = (await その日一覧()).Select(x => {
                var item = x.Toその日();
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
            return 一覧.OrderBy(x => x.yyyymmdd).Select(x => x.Toその日POCO());
        }

        public async UnaryResult<IEnumerable<その日POCO>> その日一覧()
        {
             // standard gRPC channel
            var channel = new Channel("localhost", 12345, ChannelCredentials.Insecure);

            // get MagicOnion dynamic client proxy
            var client = MagicOnionClient.Create<Iその日リポジトリIDL>(channel);

            // call method.
            return await client.一覧();
        }

        public async UnaryResult<IEnumerable<やることPOCO>> やること一覧()
        {
             // standard gRPC channel
            var channel = new Channel("localhost", 12346, ChannelCredentials.Insecure);

            // get MagicOnion dynamic client proxy
            var client = MagicOnionClient.Create<IやることリポジトリIDL>(channel);

            // call method.
            return await client.一覧();
        }
    }
}