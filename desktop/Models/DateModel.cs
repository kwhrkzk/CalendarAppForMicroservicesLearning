using System;
using System.Collections.ObjectModel;
using Microservices.Calendar.Domain;

#nullable enable

namespace desktop.Models
{
    public class DateModel
    {
        private DateTime Date { get; }

        private その日? _その日;
        public その日? その日
        {
            get => _その日;
            set
            {
                _その日 = value;
                if (_その日 != null)
                    要件件名一覧.AddRange(_その日.要件件名一覧);
            }

        }

        public ObservableCollection<string> 要件件名一覧 { get; } = new ObservableCollection<string>();

        public int 日 => Date.Day;

        public string yyyymmdd => Date.ToString("yyyyMMdd");
        public bool その日と合ってる(その日 _その日) => yyyymmdd == _その日.yyyymmdd;

        public DateModel(DateTime _date)
        {
            Date = _date;
        }
    }
}