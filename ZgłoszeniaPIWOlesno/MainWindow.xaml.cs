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
        public void BtnSzukaj_Click(object sender, RoutedEventArgs e)
        {
            if (checkOwca.IsChecked == true)
            {
                //MessageBox.Show("OWCA");
                //checkBydlo.IsChecked = false;
                //checkKoza.IsChecked = false;
            }
            else if (checkBydlo.IsChecked == true)
            {
                chBoxMleczne.Visibility = Visibility.Hidden;
                //MessageBox.Show("BYDŁO");
                //checkOwca.IsChecked = false;
                //checkKoza.IsChecked = false;
            }
            else if (checkKoza.IsChecked == true)
            {
                //MessageBox.Show("KOZA");
                // checkOwca.IsChecked = false;
                // checkKoza.IsChecked = false;
            }



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
                MessageBox.Show("Numer stada musi mieć 13 znaków, sprawdź czy jest poprawny");

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

        //zapisujemy zgłoszenie
        private void btnSaveNewNotificationOfAnimalDead_Click(object sender, RoutedEventArgs e) //przycisk "Zapisz zgłoszenie"
        {
            Singleton cs = Singleton.Instance; //tworzymy instancję Singletona do połączenia z bazą banych
            cs.GetDBConnection();
            cs.GetDBConnection().Open();
            SqlCommand CommandSQL = cs.GetDBConnection().CreateCommand(); // tworzenie komendy SQl do bazy danych

            string DateNewNotificationOfAnimalDead = DateTime.Now.ToString("yyyy-MM-dd"); // aktualna data - data zgłoszenia padnięcia  
            string TimeNewNotificationOfAnimalDead = DateTime.Now.ToString("hh:mm"); // aktulna godzina - godzina zgłoszenia

            string FarmNumber, DateBorn, HowManyAnimalsInFarm, EarTagNumber, WhyDead;
            DataOfNewNotification(out FarmNumber, out DateBorn, out HowManyAnimalsInFarm, out EarTagNumber, out WhyDead);

            ConvertDataToCommandSQL(CommandSQL, FarmNumber, DateBorn, HowManyAnimalsInFarm, EarTagNumber, WhyDead);

            CommandSQL.CommandText = "INSERT INTO ZGLOSZENIA$(ID, NR_STADA, LICZBA_SZTUK, NR_KOLCZYKA, GATUNEK, PLEC, DATA_URODZENIA, DATA_PADNIECIA, GODZINA_PADNIECIA, PRZYCZYNA, OPIS_PRZYCZYNA, KTO_ODBIERA, OSOBA_ZGL, DATA_CZAS_ZGL) VALUES ('4', @FarmNumber, '23', '232222222001', '123', '123', @DateBorn, '2019-12-09', '1223', '1', 2, 23, 123, 444)";
                       
            try // wykonanie zapytania do bazy
               //wyświetlanie nowego okna z pdfem i wysłanie maili z załącznikiem
            {
                SqlDataReader save = CommandSQL.ExecuteReader();
                cs.GetDBConnection().Close(); // zamykanie połączenia 
                PrintSendMail okno = new PrintSendMail(this);
                okno.Owner = this;
                okno.ShowDialog();


                txtSurname.Clear();
                txtName.Clear();
                txtFarmNumberSaveNofification.Clear();
                txtHowManyAnimalsInFarm.Clear();
                txtCity.Clear();
                txtStreet.Clear();
                txtHouseNumber.Clear();
                txtLocalNumber.Clear();
                txtPostCode.Clear();
                txtPost.Clear();
                txtFarmNumber.Clear();
                txtDateBorn.Clear();
                txtDateDead.Clear();
                txtEarTagNumber.Clear();
                txtWhyDead.Clear();
                txtHourOfDeadAnimal.Clear();
                chBoxUstalona.IsChecked = false;
                chBoxNieustalona.IsChecked = false;
                chBoxMleczne.IsChecked = false;
                chBoxOpasowe.IsChecked = false;
                chBoxJasta.IsChecked = false;
                chBoxFarmutil.IsChecked = false;
            }
            catch (SqlException odbcEx)
            {
                MessageBox.Show("Coś poszło nie tak z zapisem zgłoszenia, trzeba to sprawdzić.");// obsługa bardziej szczegółowych wyjątków
            }
            catch (Exception ex)
            {
                MessageBox.Show("błąd 2"); // obsługa wyjątku głównego 
            }                                   
        }


        private static void ConvertDataToCommandSQL(SqlCommand CommandSQL, string FarmNumber, string DateBorn,
            string HowManyAnimalsInFarm, string EarTagNumber, string WhyDead) //konwersja zmiennych zawierających dane zgłoszenia do polecenia SQL
        {
            CommandSQL.Parameters.Add("@FarmNumber", SqlDbType.VarChar).Value = FarmNumber;
            CommandSQL.Parameters.Add("@DateBorn", SqlDbType.VarChar).Value = DateBorn;
            CommandSQL.Parameters.Add("@HowManyAnimalsInFarm", SqlDbType.VarChar).Value = HowManyAnimalsInFarm;
            CommandSQL.Parameters.Add("@EarTagNumber", SqlDbType.VarChar).Value = EarTagNumber;
            CommandSQL.Parameters.Add("@WhyDead", SqlDbType.VarChar).Value = WhyDead;
        }

        private void DataOfNewNotification(out string FarmNumber, out string DateBorn, out string HowManyAnimalsInFarm, 
            out string EarTagNumber, out string WhyDead) //pobranie z formularza danych zgłoszenia do zmienych
        {

            EarTagNumber = txtEarTagNumber.Text;
            HowManyAnimalsInFarm = txtHowManyAnimalsInFarm.Text;
            FarmNumber = txtFarmNumber.Text;
            DateBorn = txtDateBorn.Text;
            WhyDead = txtWhyDead.Text;
        }

      
        private void CheckOwca_Checked(object sender, RoutedEventArgs e)
        {
            checkBydlo.IsChecked = false;
            checkKoza.IsChecked = false;
        }

        private void CheckBydlo_Checked(object sender, RoutedEventArgs e)
        {
            checkOwca.IsChecked = false;
            checkKoza.IsChecked = false;
        }

        private void CheckKoza_Checked(object sender, RoutedEventArgs e)
        {
            checkOwca.IsChecked = false;
            checkBydlo.IsChecked = false;
        }

        private void ChBoxUstalona_Checked(object sender, RoutedEventArgs e)
        {
            chBoxNieustalona.IsChecked =false;
        }

        private void ChBoxNieustalona_Checked(object sender, RoutedEventArgs e)
        {
            chBoxUstalona.IsChecked = false;
        }

        private void ChBoxMleczne_Checked(object sender, RoutedEventArgs e)
        {
            chBoxOpasowe.IsChecked = false;
        }

        private void ChBoxOpasowe_Checked(object sender, RoutedEventArgs e)
        {
            chBoxMleczne.IsChecked = false;
        }

        private void ChBoxJasta_Checked(object sender, RoutedEventArgs e)
        {
            chBoxFarmutil.IsChecked = false;
        }

        private void ChBoxFarmutil_Checked_1(object sender, RoutedEventArgs e)
        {
            chBoxJasta.IsChecked = false;
        }

        /// </summary>
       
    }
}
