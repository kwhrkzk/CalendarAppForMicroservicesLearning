namespace Microservices.Calendar.Domain
open System
open System.Collections.Generic
open MagicOnion
open MessagePack

type 日時 =
    {
        _date : DateTime
    }
    static member Create(a) = { _date = a }
    member this.その月の日数 (_date:DateTime) = DateTime.DaysInMonth(_date.Year, _date.Month)
    member this.日数 = this.その月の日数(this._date)
    member this.yyyymmdd = this._date.ToString("yyyyMMdd")

type 件名 =
    {
        _title : string
    }
    member this.件名文字列 = this._title
    static member Create a = { _title = a }

type 内容 =
    {
        _body : string
    }
    static member Create a = { _body = a }

 [<MessagePackObject>]
 type 要件POCO () =
    [<IgnoreMember>][<DefaultValue>] val mutable _件名 : string
    [<IgnoreMember>][<DefaultValue>] val mutable _内容 : string
    [<Key(0)>]
    member this.title
        with get() = this._件名
        and set (_件名) = this._件名 <- _件名
    [<Key(1)>]
    member this.body
        with get() = this._内容
        and set (_内容) = this._内容 <- _内容

type 要件 =
    {
        _件名: 件名
        _内容: 内容
    }
    member this.件名文字列 = this._件名.件名文字列
    static member Create(_件名, _内容) =
        {
            _件名 = 件名.Create(_件名)
            _内容 = 内容.Create(_内容)
        }
    member this.To要件POCO() =
        let mutable poco = 要件POCO()
        poco.title <- this._件名._title
        poco.body <- this._内容._body
        poco

type 要件POCO with
    member this.To要件() =
        {
            要件._件名 = 件名.Create(this.title)
            _内容 = 内容.Create(this.body)
        }

type 要件一覧 =
    {
        _list: 要件 list
    }
    member this.件名一覧 = this._list |> List.map (fun x -> x.件名文字列) |> List.toSeq
    member this.Add(_要件:要件) = { this with _list = _要件 :: this._list }
    static member Create(_要件一覧:要件 seq) =
        {
            _list = Seq.toList _要件一覧
        }

 [<MessagePackObject>]
type その日POCO () =
    [<IgnoreMember>][<DefaultValue>] val mutable _日時 : DateTime
    [<IgnoreMember>][<DefaultValue>] val mutable _要件一覧 : System.Collections.Generic.List<要件POCO>
    [<Key(0)>]
    member this.date
        with get() = this._日時
        and set (_日時) = this._日時 <- _日時
    [<Key(1)>]
    member this.list
        with get() = this._要件一覧
        and set (_要件一覧) = this._要件一覧 <- _要件一覧


type その日 =
    {
        _日時: 日時
        _要件一覧: 要件一覧 Option
    }
    member this.yyyymmdd = this._日時.yyyymmdd
    member this.要件件名一覧 = match this._要件一覧 with None -> [||] |> Array.toSeq | Some(x) -> x.件名一覧
    member this.Add要件 (_要件:要件) =
        let 要件一覧 = match this._要件一覧 with
                        | None -> 要件一覧.Create([| _要件 |] |> Array.ofSeq)
                        | Some(x) ->  x.Add(_要件)
        { this with _要件一覧 = Some 要件一覧 }
    static member Create(_日時) =
        {
            _日時 = 日時.Create(_日時)
            _要件一覧 = None
        }
    static member Create(_日時, _要件一覧:要件 seq) =
        {
            _日時 = 日時.Create(_日時)
            _要件一覧 = Some (要件一覧.Create(_要件一覧))
        }
    member this.Toその日POCO() =
        let mutable poco = その日POCO()
        poco.date <- this._日時._date
        poco.list <- match this._要件一覧 with None -> null | Some(x) -> System.Collections.Generic.List<要件POCO>(x._list |> List.map (fun y -> y.To要件POCO()) |> List.toSeq)
        poco

type その日POCO with
    member this.Toその日() =
        {
            その日._日時 = 日時.Create(this.date)
            _要件一覧 = match this.list with null -> None | _ -> Some (this.list :> 要件POCO seq |> Seq.map (fun x -> x.To要件()) |> 要件一覧.Create)
        }

type Iその日リポジトリ =
    abstract 一覧 : unit -> その日 seq

type Iその日リポジトリIDL =
        inherit IService<Iその日リポジトリIDL>
        abstract 一覧 : unit -> UnaryResult<その日POCO seq>

