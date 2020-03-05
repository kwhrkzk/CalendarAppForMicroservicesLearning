using System;
using System.Collections.Generic;
using Microservices.Calendar.Domain;

namespace Microservices.Calendar.Infra
{
    public class その日リポジトリ : Iその日リポジトリ
    {
        public IEnumerable<その日> 一覧()
        {
            return new List<その日>
            {
                その日.Create(new DateTime(2020,2,14), new []
                {
                    要件.Create("要件1", "内容1"),
                }),
                その日.Create(new DateTime(2020,2,28), new []
                {
                    要件.Create("要件1", "内容1"),
                    要件.Create("要件2", "内容2"),
                    要件.Create("要件3", "内容3"),
                }),
                その日.Create(new DateTime(2020,2,29)),
            };
        }
    }
}
