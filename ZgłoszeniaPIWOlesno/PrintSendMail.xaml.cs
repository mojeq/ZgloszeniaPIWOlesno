using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Logika interakcji dla klasy PrintSendMail.xaml
    /// </summary>
    public partial class PrintSendMail : Window       
    {
        MainWindow mainWindow;
        public PrintSendMail(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }
             
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.txtFarmNumber.Text = "1111111111111";
        }
    }
}
