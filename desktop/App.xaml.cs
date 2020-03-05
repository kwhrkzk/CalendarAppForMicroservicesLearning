using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Windows;
using Prism.Unity;
using desktop;
using Cysharp.Text;
using desktop.Views;
using Microservices.Calendar.Domain;
using Microservices.Todo.Domain;
using desktop.ViewModels;
using desktop.Models;

namespace desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<CalendarModel>();
            containerRegistry.Register<CalendarViewModel>();
            containerRegistry.RegisterForNavigation<CalendarView>();
            containerRegistry.RegisterForNavigation<TODOView>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RequestNavigate("CalendarRegion", nameof(CalendarView));
            Container.Resolve<IRegionManager>().RequestNavigate("TODORegion", nameof(TODOView));
        }
        public App()
        {
            // マネージコード内で例外がスローされると最初に必ず発生する（.NET 4.0より）
            //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            // バックグラウンドタスク内で処理されなかったら発生する（.NET 4.0より）
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // 例外が処理されなかったら発生する（.NET 1.0より）
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMember = e.Exception.TargetSite.Name;
            string errorMessage = e.Exception.Message;
            string message = ZString.Format(@"例外が{0}で発生。プログラムを継続しますか？エラーメッセージ：{1}", errorMember, errorMessage);

            MessageBoxResult result = MessageBox.Show(message, "DispatcherUnhandledException", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                e.Handled = true;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            string errorMember = e.Exception.InnerException.TargetSite.Name;
            string errorMessage = e.Exception.InnerException.Message;
            string message = ZString.Format(@"例外がバックグラウンドタスクの{0}で発生。プログラムを継続しますか？エラーメッセージ：{1}", errorMember, errorMessage);

            MessageBoxResult result = MessageBox.Show(message, "UnobservedTaskException", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception == null)
            {
                MessageBox.Show("System.Exceptionとして扱えない例外");
                return;
            }

            string errorMember = exception.TargetSite.Name;

            string errorMessage = exception.Message;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                errorMessage += exception.Message;
            }

            string message = ZString.Format(@"例外が{0}で発生。プログラムは終了します。エラーメッセージ：{1}", errorMember, errorMessage);

            MessageBox.Show(message, "UnhandledException", MessageBoxButton.OK, MessageBoxImage.Stop);

            Environment.Exit(0);
        }
    }
}
