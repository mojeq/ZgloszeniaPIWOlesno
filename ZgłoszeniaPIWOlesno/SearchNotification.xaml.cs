using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZgłoszeniaPIWOlesno
{
    /// <summary>
    /// Logika interakcji dla klasy SearchNotification.xaml
    /// </summary>
    public partial class SearchNotification : Window
    {
        private MainWindow mainWindow = null;
        //MainWindow mainWindow;
        public SearchNotification(MainWindow mainWin)
        {
            //this.mainWindow = mainWindow;
            mainWindow = mainWin;
            InitializeComponent();
            PasteDataToGrid();
        }
        private void PasteDataToGrid()
        {
            //wyszukanie zgłoszeń padnięcia dla wpisanego numeru stada za pomocą Entity Framework
            string farmNumber = mainWindow.txtFarmNumberSearch.Text;
            BAZA_ARIMREntities db = new BAZA_ARIMREntities();
            try
            {
                var notification_list = db.ZGLOSZENIA_.Where(a => a.NR_STADA == farmNumber);
                dataGrid1.ItemsSource = notification_list.ToList();
            }
            catch (SqlException odbcEx)
            {
                MessageBox.Show("Coś poszło nie tak z wyszukaniem gospodarstwa(SQL).");// obsługa bardziej szczegółowych wyjątkóws.GetDBConnection().Close(); // zamykanie połączenia 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd, przerwano działanie."); // obsługa wyjątku głównego                  
            }
        }

        private void BtnGenerateAttachments_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("Wygenerować załączniki do wybranej pozycji?");
            DataGrid dg = (DataGrid)sender;
            ZGLOSZENIA_ row = dg.SelectedItem as ZGLOSZENIA_;
            string numer = row.DATA_PADNIECIA.ToString();
            MessageBox.Show(numer);
        }
    }

}