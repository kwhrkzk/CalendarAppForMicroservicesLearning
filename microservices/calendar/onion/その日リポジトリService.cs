using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using Microservices.Calendar.Domain;
using Microservices.Calendar.Infra;

namespace onion
{
    public class その日リポジトリService : ServiceBase<Iその日リポジトリIDL>, Iその日リポジトリIDL {
        private Iその日リポジトリ その日リポジトリ { get; }
        public その日リポジトリService()
        {
            その日リポジトリ = new その日リポジトリ();
        }
        // You can use async syntax directly.
        public async UnaryResult<IEnumerable<その日POCO>> 一覧 () 
        => その日リポジトリ.一覧().Select(x => x.Toその日POCO());
    }
}