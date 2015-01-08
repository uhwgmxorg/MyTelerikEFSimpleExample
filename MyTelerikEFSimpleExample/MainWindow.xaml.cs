/******************************************************************************/
/*                                                                            */
/*   Program: MyTelerikEFSimpleExample                                        */
/*   Example for bining a simple SQLDB table to a telerik RadGridView         */
/*   with EF 6 and CRUD (Create, Read, Update and Delete) functionality       */
/*                                                                            */
/*   28.11.2014 1.0.0.0 uhwgmxorg Start                                       */
/*                                                                            */
/******************************************************************************/
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Telerik.Windows.Controls;

namespace MyTelerikEFSimpleExample
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private App _application;
        private NLog.Logger _logger;

        /// <summary>
        /// Copnstructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _application = ((App)Application.Current);
            _logger = _application._logger;

#if DEBUG
            Title += "    Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
#else
            Title += "    Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
#endif
            LoadData();
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// ButtonReload_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            _logger.Info("ButtonReload was clicked");
        }

        /// <summary>
        /// Button_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _logger.Info("ButtonClose was clicked");
        }

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// PeopleGrid_AddingNewDataItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeopleGrid_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            _logger.Info("Add a new item");
            try
            {
                e.NewObject = new Person();
			}
			catch (Exception ex)
			{
                _logger.Error(ex);
				MessageBox.Show(ex.Message);
			}
        }

        /// <summary>
        /// PeopleGrid_RowEditEnded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeopleGrid_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            _logger.Info("Edit an item");
            try
            {
                PeopleEntityFrameworkDataSource.SubmitChanges();
			}
			catch (Exception ex)
			{
                _logger.Error(ex);
                MessageBox.Show(ex.Message);
			}
        }

        /// <summary>
        /// PeopleGrid_Deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeopleGrid_Deleted(object sender, GridViewDeletedEventArgs e)
        {
            _logger.Info("Delete an item");
            try
            {
                PeopleEntityFrameworkDataSource.SubmitChanges();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Window_Closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            _logger.Info("Stop (Window_Closed) MyTelerikEFSimpleExample");
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// LoadData
        /// </summary>
        private void LoadData()
        {
            _logger.Info("Load data");
            try
            {
                MyConfiguration MyConfiguration = new MyConfiguration();
                PeopleEntityFrameworkDataSource.DbContext = new TestDBEntities();
                PeopleEntityFrameworkDataSource.DbContext.Database.Log = DBLog;
			}
			catch (Exception ex)
			{
                _logger.Error(ex);
				MessageBox.Show(ex.Message);
			}
        }

        /// <summary>
        /// DBLog
        /// SQL logging for EntityFramework
        /// </summary>
        /// <param name="sQLString"></param>
        public void DBLog(string sQLString)
        {
            // remove carriage return line feed
            sQLString = sQLString.Replace("\r\n", string.Empty);
            // remove remove multiple spaces
            string cleanedString = System.Text.RegularExpressions.Regex.Replace(sQLString, @"\s+", " ");
            _logger.Trace(cleanedString);
            Debug.WriteLine(cleanedString);
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="p"></param>
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }

    #region Help Classes for Entity Framework 6

    /// <summary>
    /// Class MyExecutionStrategy
    /// </summary>
    public class MyExecutionStrategy : System.Data.Entity.Infrastructure.DbExecutionStrategy
    {
        private App _application;
        private NLog.Logger _logger;

        public static int Counter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxRetryCount"></param>
        /// <param name="maxDelay"></param>
        public MyExecutionStrategy(int maxRetryCount, TimeSpan maxDelay) : base(maxRetryCount, maxDelay)
        {
            _application = ((App)Application.Current);
            _logger = _application._logger;
            Counter = 0;
        }

        /// <summary>
        /// ShouldRetryOn
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected override bool ShouldRetryOn(Exception exception)
        {
            _logger.Error("EF Exception {0} in ShouldRetryOn", ++Counter);
            string ErrorMessage = String.Format("{0}",exception);
            _logger.Error(ErrorMessage);
            if(exception.InnerException != null)
                _logger.Error(exception.InnerException);
            Debug.WriteLine(ErrorMessage);
            return true;
        }
    }

    /// <summary>
    /// Class MyConfiguration
    /// </summary>
    public class MyConfiguration : System.Data.Entity.DbConfiguration
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MyConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new MyExecutionStrategy(0, TimeSpan.FromSeconds(0)));
        }
    }

    #endregion
}
