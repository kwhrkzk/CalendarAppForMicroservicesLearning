using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using desktop.Models;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace desktop.ViewModels {
    public class CalendarViewModel : IConfirmNavigationRequest, IRegionMemberLifetime, IDisposable, INotifyPropertyChanged {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        private IRegionNavigationService RegionNavigationService { get; set; }

        public bool KeepAlive => false;

        private IDialogService DialogService { get; }

        private CompositeDisposable DisposeCollection = new CompositeDisposable ();
        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        [SuppressMessage ("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")]
        protected virtual void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    DisposeCollection.Dispose ();
                }
                disposedValue = true;
            }
        }

        public void Dispose () => Dispose (true);
        #endregion
        public CalendarModel Model { get; }
        public CalendarViewModel (CalendarModel _calendarModel)
        {
            Model = _calendarModel;
        }

        public bool IsNavigationTarget (NavigationContext navigationContext) => true;
        public void ConfirmNavigationRequest (NavigationContext navigationContext, Action<bool> continuationCallback) => continuationCallback (true);

        public async void OnNavigatedTo (NavigationContext navigationContext) {
            RegionNavigationService = navigationContext.NavigationService;

            await Model.Initialize ();
        }

        public void OnNavigatedFrom (NavigationContext navigationContext) => Dispose ();
    }
}