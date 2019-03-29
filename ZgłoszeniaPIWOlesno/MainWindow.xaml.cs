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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using ZgłoszeniaPIWOlesno;

namespace ZgłoszeniaPIWOlesno
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
       
        InitializeComponent();
        }


        //przycisk BYDŁO, wpisujemy numer stada i klikamy przycisk BYDŁO
        /// <summary>    
        
        public void BtnBydloPadlo_Click(object sender, RoutedEventArgs e)
        {


            if (txtNumer_stada.Text.Length == 13)
            {
                string numer_stada = txtNumer_stada.Text;
                MessageBox.Show(numer_stada);

                dane_zgloszenia.Visibility = Visibility.Visible; //wyświetlenie na prawej stronie Uzupełnij dane zgłoszenia

                //laczymy = baza.Connect1();

                //łączenie z bazą
                // Singleton.Instance.Connect(numer_stada);
                Singleton cs = Singleton.Instance; //tworzymy instancję Singletona do połączenia z bazą banych
                cs.GetDBConnection();
                cs.GetDBConnection().Open();

                SqlCommand komendaSQL = cs.GetDBConnection().CreateCommand();

                komendaSQL.Parameters.Add("@stado", SqlDbType.VarChar).Value = numer_stada; //przypisane numeru stada w zapytaniu SQL

                komendaSQL.CommandText = "SELECT * FROM BAZA_GOSPODARSTWA$ WHERE NR_STADA=@stado";
                SqlDataReader czytnik = komendaSQL.ExecuteReader(); // wykonanie zapytania do bazy


                while (czytnik.Read()) //wyświetlenie danych gospodarstwa na ekranie
                {
                    txtNazwisko_nazwa.Text = czytnik["NAZWISKO_LUB_NAZWA"].ToString();
                    txtImie_nazwa.Text = czytnik["IMIE_LUB_NAZWA_SKROCONA"].ToString();
                    txtNumer_stada1.Text = czytnik["NR_STADA"].ToString();
                    txtLiczba_sztuk.Text = czytnik["LICZBA_SZTUK"].ToString();
                    txtMiejscowosc.Text = czytnik["MIEJSCOWOSC"].ToString();
                    txtUlica.Text = czytnik["ULICA"].ToString();
                    txtNumer_posesji.Text = czytnik["POSESJA"].ToString();
                    txtNumer_lokalu.Text = czytnik["LOKAL"].ToString();
                    txtKod_pocztowy.Text = czytnik["KOD_POCZTOWY"].ToString();
                    txtPoczta.Text = czytnik["POCZTA"].ToString();
                                        
                }
                czytnik.Close();
                cs.GetDBConnection().Close();


            }

            else
            {
                //gdy nymer stada wpisany ma niepoprawną długość
                MessageBox.Show("Numer stada błędny, sprawdź czy jest poprawny");

                //wyświetlanie innego okna window1
                //Window1 wnd = new Window1();
                //wnd.Show();
            }
        }

        /// </summary>
        //koniec przycisku BYDŁO
    }
}
