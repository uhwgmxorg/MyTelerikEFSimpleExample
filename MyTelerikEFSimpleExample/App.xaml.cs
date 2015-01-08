using System.Windows;

namespace MyTelerikEFSimpleExample
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Vars
        /// <summary>
        public NLog.Logger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            string Message = "Start ";
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            _logger = NLog.LogManager.GetCurrentClassLogger();
#if DEBUG
            Message += " Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
#else
            Message += " Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
#endif
            _logger.Info("Start MyTelerikEFSimpleExample {0}",Message);
        }

        /// <summary>
        /// OnDispatcherUnhandledException
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error("Exception in OnDispatcherUnhandledException (MainThread)");
            string ErrorMessage = string.Format("{0}", e.Exception);
            _logger.Error(ErrorMessage);
            ErrorMessage = string.Format("StackTrace: {0}", e.Exception.StackTrace);
            _logger.Error(ErrorMessage);
            if (e.Exception.InnerException != null)
            {
                ErrorMessage = string.Format("InnerException: {0}", e.Exception.InnerException.Message);
                _logger.Error(ErrorMessage);
                ErrorMessage = string.Format("StackTrace: {0}", e.Exception.InnerException.StackTrace);
                _logger.Error(ErrorMessage);
            }
            e.Handled = true;
        }
    }
}
