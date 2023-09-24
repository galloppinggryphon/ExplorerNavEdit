using System;
using System.Windows;
using System.Windows.Threading;

namespace ExplorerNav
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            base.OnStartup(e);
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error occurred:\n\n" + e.Exception.ToString(), "Program Error", MessageBoxButton.OK, MessageBoxImage.Stop);

            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // cannot handle this (but rare)
            var isTerminating = e.IsTerminating;
        }
    }
}
