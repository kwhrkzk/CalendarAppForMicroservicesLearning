using System.Collections.Generic;
using System.Linq;
using MagicOnion;
using MagicOnion.Server;
using Microservices.Todo.Domain;
using Microservices.Todo.Infra;

namespace onion
{
    public class やることリポジトリService : ServiceBase<IやることリポジトリIDL>, IやることリポジトリIDL
    {
        public Iやることリポジトリ やることリポジトリ { get; }

        public やることリポジトリService()
        {
            やることリポジトリ = new やることリポジトリ();
        }

        public async UnaryResult<IEnumerable<やることPOCO>> 一覧()
        => やることリポジトリ.一覧().一覧.Select(x => x.ToやることPOCO());
    }
}