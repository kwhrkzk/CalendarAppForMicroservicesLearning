namespace Microservices.Todo.Domain
open System
open MagicOnion
open MessagePack

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
    member this.内容文字列 = this._body
    static member Create a = { _body = a }

type やることID =
    {
        _id : Guid
    }
    static member Create(_id) = { _id = Guid.Parse(_id) }
    static member NEW = { _id = Guid.NewGuid() }
    member this.ID文字列 = this._id.ToString()

type 期限 =
    {
        _date: DateTime
    }
    member this.期限DateTime = this._date
    static member Create(_date) = { _date = _date }

type 完了日 =
    {
        _date: DateTime
    }
    member this.完了日DateTime = this._date
    member this.yyyymmdd = this._date.ToString("yyyyMMdd")
    static member Create(_date) = { _date = _date }

 [<MessagePackObject>]
type やることPOCO () =
    [<IgnoreMember>][<DefaultValue>] val mutable _id : string
    [<IgnoreMember>][<DefaultValue>] val mutable _件名 : string
    [<IgnoreMember>][<DefaultValue>] val mutable _内容 : string
    [<IgnoreMember>][<DefaultValue>] val mutable _期限 : Nullable<DateTime>
    [<IgnoreMember>][<DefaultValue>] val mutable _完了日 : Nullable<DateTime>
    [<Key(0)>]
    member this.id
        with get() = this._id
        and set (_id) = this._id <- _id
    [<Key(1)>]
    member this.title
        with get() = this._件名
        and set (_件名) = this._件名 <- _件名
    [<Key(2)>]
    member this.body
        with get() = this._内容
        and set (_内容) = this._内容 <- _内容
    [<Key(3)>]
    member this.limit
        with get() = this._期限
        and set (_期限) = this._期限 <- _期限
    [<Key(4)>]
    member this.complete
        with get() = this._完了日
        and set (_完了日) = this._完了日 <- _完了日

type やること =
    {
        _id: やることID
        _件名: 件名 Option
        _内容: 内容 Option
        _期限: 期限 Option
        _完了日: 完了日 Option
    }
    member this.やることID文字列 = this._id.ID文字列
    member this.件名文字列 = match this._件名 with None -> "" | Some(x) -> x.件名文字列
    member this.内容文字列 = match this._内容 with None -> "" | Some(x) -> x.内容文字列
    member this.期限DateTime = match this._期限 with None -> Nullable<DateTime>() | Some(x) -> Nullable x.期限DateTime
    member this.完了日DateTime = match this._完了日 with None -> Nullable<DateTime>() | Some(x) -> Nullable x.完了日DateTime
    member this.完了日yyyymmdd = match this._完了日 with None -> "" | Some(x) -> x.yyyymmdd
    member this.完了(_完了日:完了日) = { this with _完了日 = Some _完了日 }
    member this.未完了() = { this with _完了日 = None }
    member this.with期限(_期限:Nullable<DateTime>) = { this with _期限 = match _期限.HasValue with true -> Some (期限.Create(_期限.Value)) | false -> None }
    static member Create(_件名:件名) =
        {
            _id = やることID.NEW
            _件名 = Some _件名
            _内容 = None
            _期限 = None
            _完了日 = None
        }
    static member Create(_内容:内容) =
        {
            _id = やることID.NEW
            _件名 = None
            _内容 = Some _内容
            _期限 = None
            _完了日 = None
        }
    static member Create(_件名, _内容) =
        {
            _id = やることID.NEW
            _件名 = Some (件名.Create(_件名))
            _内容 = Some (内容.Create(_内容))
            _期限 = None
            _完了日 = None
        }
    static member Create(_id, _件名, _内容) =
        {
            _id = やることID.Create(_id)
            _件名 = Some (件名.Create(_件名))
            _内容 = Some (内容.Create(_内容))
            _期限 = None
            _完了日 = None
        }
    member this.ToやることPOCO() = 
        let poco = new やることPOCO()
        poco.id <- this.やることID文字列
        poco.title <- this.件名文字列
        poco.body <- this.内容文字列
        poco.limit <- this.期限DateTime
        poco.complete <- this.完了日DateTime
        poco

type やることPOCO with
    member this.Toやること() =
        let item = やること.Create(this.id, this.title, this.body)
        let item = match this.complete.HasValue with
                    | true -> item.完了(完了日.Create(this.complete.Value))
                    | false -> item.未完了()
        let item = item.with期限(this.limit)
        item

type やること一覧 =
    {
        _list: やること list
    }
    static member Create(_やること:やること seq) =
        {
            _list = List.ofSeq _やること
        }
    member this.一覧 = this._list |> List.toSeq
    member this.完了一覧 = this._list |> List.filter (fun x -> x._完了日.IsSome) |> List.toSeq

type Iやることリポジトリ =
    abstract 一覧 : unit -> やること一覧

type IやることリポジトリIDL =
        inherit IService<IやることリポジトリIDL>
        abstract 一覧 : unit -> UnaryResult<やることPOCO seq>