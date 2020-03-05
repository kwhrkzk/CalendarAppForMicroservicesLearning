using System;
using Microservices.Todo.Domain;

namespace Microservices.Todo.Infra
{
    public class やることリポジトリ : Iやることリポジトリ
    {
        public やること一覧 一覧()
        => やること一覧.Create(new []{
            やること.Create("TODO件名１", "TODO内容１"),
            やること.Create("TODO件名２", "TODO内容２").完了(完了日.Create(new DateTime(2020,2,5))),
            やること.Create("TODO件名３", "TODO内容３"),
        });
    }
}
