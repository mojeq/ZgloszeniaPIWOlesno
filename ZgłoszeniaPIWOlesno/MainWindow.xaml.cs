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

        /// <summary>    
        public void BtnBydlo_Click(object sender, RoutedEventArgs e)
        {
            if (txtFarmNumber.Text.Length == 13)
            {
                string FarmNumber = txtFarmNumber.Text;
                MessageBox.Show(FarmNumber);

                NewNotification.Visibility = Visibility.Visible; //wyświetlenie na prawej stronie Uzupełnij dane zgłoszenia
                //laczymy = baza.Connect1();
                //łączenie z bazą
                // Singleton.Instance.Connect(numer_stada);
                Singleton cs = Singleton.Instance; //tworzymy instancję Singletona do połączenia z bazą banych
                cs.GetDBConnection();
                cs.GetDBConnection().Open();

                SqlCommand CommandSQL = cs.GetDBConnection().CreateCommand();

                CommandSQL.Parameters.Add("@FarmNumber", SqlDbType.VarChar).Value = FarmNumber; //przypisane numeru stada(FarmNumber) w zapytaniu SQL

                CommandSQL.CommandText = "SELECT * FROM BAZA_GOSPODARSTWA$ WHERE NR_STADA=@FarmNumber";
                SqlDataReader reader = CommandSQL.ExecuteReader(); // wykonanie zapytania do bazy

                if (!reader.HasRows)
                {
                    MessageBox.Show("Nie ma takiego numeru w bazie, sprawdź wprowadzony numer lub wprowadź dane gospodarstwa ręcznie.");
                } // sprawdzam czy znaleziono jakiś rekord w bazie

                while (reader.Read())
                {
                    txtSurname.Text = reader["NAZWISKO_LUB_NAZWA"].ToString();
                    txtName.Text = reader["IMIE_LUB_NAZWA_SKROCONA"].ToString();
                    txtFarmNumberSaveNofification.Text = reader["NR_STADA"].ToString();
                    txtHowManyAnimalsInFarm.Text = reader["LICZBA_SZTUK"].ToString();
                    txtCity.Text = reader["MIEJSCOWOSC"].ToString();
                    txtStreet.Text = reader["ULICA"].ToString();
                    txtHouseNumber.Text = reader["POSESJA"].ToString();
                    txtLocalNumber.Text = reader["LOKAL"].ToString();
                    txtPostCode.Text = reader["KOD_POCZTOWY"].ToString();
                    txtPost.Text = reader["POCZTA"].ToString();
                }//wyświetlenie danych gospodarstwa którego dotyczy zgłoszenie 
                reader.Close();
                cs.GetDBConnection().Close(); // zamykanie połączenia        
            }
            else
            {
                //gdy nymer stada wpisany ma niepoprawną długość
                MessageBox.Show("Numer stada błędny, sprawdź czy jest poprawny");

                //wyświetlanie innego okna window1
                //Window1 wnd = new Window1();
                //wnd.Show();
            }
        }//przycisk BYDŁO, wpisujemy numer stada i klikamy przycisk BYDŁO

        private void DatePickerDateBornAnimal_SelectedDateChanged(object sender, SelectionChangedEventArgs e) // Data picker - data urodzenia zwierzęcia
        {
            var picker = sender as DatePicker; // referencja Data picker

            DateTime? date = picker.SelectedDate; /// pozyskanie daty nullable z SelectedDate
            if (date == null)
            {
                this.Title = "Brak daty";// gdy nie ma daty
            }
            else
            {
                this.Title = date.Value.ToString("yyyy-MM-dd"); // konwersja daty na string
                txtDateBorn.Text = this.Title; // zapis daty w boxie DataBorn
            }
        }
        private void DatePickerDateDeadAnimal_SelectedDateChanged(object sender, SelectionChangedEventArgs e) // Data picker - data padnięcia zwierzęcia
        {
            var picker = sender as DatePicker; // referencja Data picker

            DateTime? date = picker.SelectedDate; /// pozyskanie daty nullable z SelectedDate
            if (date == null)
            {
                this.Title = "Brak daty";// gdy nie ma daty
            }
            else
            {
                this.Title = date.Value.ToString("yyyy-MM-dd"); // konwersja daty na string
                txtDateDead.Text = this.Title; // zapis daty w boxie DataDead(data padnięcia)
            }
        }

        private void btnSaveNewNotificationOfAnimalDead_Click(object sender, RoutedEventArgs e) //przycisk "Zapisz zgłoszenie"
        {         
            Singleton cs = Singleton.Instance; //tworzymy instancję Singletona do połączenia z bazą banych
            cs.GetDBConnection();
            cs.GetDBConnection().Open();
            SqlCommand CommandSQL = cs.GetDBConnection().CreateCommand(); // tworzenie komendy SQl do bazy danych

            string DateNewNotificationOfAnimalDead = DateTime.Now.ToString("yyyy-MM-dd"); // aktualna data - data zgłoszenia padnięcia  
            string TimeNewNotificationOfAnimalDead = DateTime.Now.ToString("hh:mm"); // aktulna godzina - godzina zgłoszenia
            string FarmNumber, DateBorn, HowManyAnimalsInFarm;
            DataOfNewNotification(out FarmNumber, out DateBorn, out HowManyAnimalsInFarm);

            ConvertDataToCommandSQL(CommandSQL, FarmNumber, DateBorn);

            CommandSQL.CommandText = "INSERT INTO ZGLOSZENIA$(ID, NR_STADA, LICZBA_SZTUK, NR_KOLCZYKA, GATUNEK, PLEC, DATA_URODZENIA, DATA_PADNIECIA, GODZINA_PADNIECIA, PRZYCZYNA, OPIS_PRZYCZYNA, KTO_ODBIERA, OSOBA_ZGL, DATA_CZAS_ZGL) VALUES ('4', @FarmNumber, '23', '232222222001', '123', '123', @DateBorn, '2019-12-09', '1223', '1', 2, 23, 123, 444)";

            SqlDataReader save = CommandSQL.ExecuteReader(); // wykonanie zapytania do bazy
            cs.GetDBConnection().Close(); // zamykanie połączenia  
                                 
        }

        private static void ConvertDataToCommandSQL(SqlCommand CommandSQL, string FarmNumber, string DateBorn) //konwersja zmiennych zawierających dane zgłoszenia do polecenia SQL
        {
            CommandSQL.Parameters.Add("@FarmNumber", SqlDbType.VarChar).Value = FarmNumber; 
            CommandSQL.Parameters.Add("@DateBorn", SqlDbType.VarChar).Value = DateBorn; 
        }

        private void DataOfNewNotification(out string FarmNumber, out string DateBorn, out string HowManyAnimalsInFarm) //pobranie z formularza danych zgłoszenia do zmienych
        {
            HowManyAnimalsInFarm = txtHowManyAnimalsInFarm.Text;
            FarmNumber = txtFarmNumber.Text;
            DateBorn = txtDateBorn.Text;

        }



        /// </summary>
        //koniec przycisku BYDŁO
    }
}
