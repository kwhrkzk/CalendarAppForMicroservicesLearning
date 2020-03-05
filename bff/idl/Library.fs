namespace Bff.Idl
open MagicOnion
open Microservices.Todo.Domain
open Microservices.Calendar.Domain

type IOnionAPIGatewayIDL =
        inherit IService<IOnionAPIGatewayIDL>
        abstract やること一覧 : unit -> UnaryResult<やることPOCO seq>
        abstract その日一覧 : unit -> UnaryResult<その日POCO seq>
        abstract こみこみ一覧 : unit -> UnaryResult<その日POCO seq>
