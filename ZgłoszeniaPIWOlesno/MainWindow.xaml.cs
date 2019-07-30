﻿using System;
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
using Syncfusion.Windows.Shared;

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
           
            if (txtFarmNumber.Text.Length == 13)
            {
                if (checkOwca.IsChecked == false&& checkKoza.IsChecked == false&& checkBydlo.IsChecked == false)
                {
                    MessageBox.Show("Nie wybrano gatunku bydło/owaca/koza");
                    return;
                }
                string FarmNumber = txtFarmNumber.Text;
                
                NewNotification.Visibility = Visibility.Visible; //wyświetlenie na prawej stronie Uzupełnij dane zgłoszenia

                // Entity Framework
                //BAZA_ARIMREntities db = new BAZA_ARIMREntities();
                //// int a = 0;
                //var farms = db.BAZA_GOSPODARSTWA_.Where(a => a.NR_STADA == "020847366-001");
                //foreach(var item in farms)
                //{
                //    MessageBox.Show($"Nazwisko:  { item.NAZWISKO_LUB_NAZWA}");
                //}

                //

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
                    txtWhoReportingNewNotification.Text = reader["IMIE_LUB_NAZWA_SKROCONA"].ToString()+' '+reader["NAZWISKO_LUB_NAZWA"].ToString();
                    txtAddressPersonReporting.Text= reader["MIEJSCOWOSC"].ToString()+' '+reader["ULICA"].ToString()+' '+reader["POSESJA"].ToString()
                        +' '+reader["LOKAL"].ToString()+' '+reader["KOD_POCZTOWY"].ToString()+' '+reader["POCZTA"].ToString();

                    string DateNewNotificationOfAnimalDead = DateTime.Now.ToString("yyyy-MM-dd"); // aktualna data - data zgłoszenia padnięcia  
                    string TimeNewNotificationOfAnimalDead = DateTime.Now.ToString("hh:mm"); // aktulna godzina - godzina zgłoszenia
                    string DateAndTimeNewNotificationOfAnimalDead = DateNewNotificationOfAnimalDead + " " + TimeNewNotificationOfAnimalDead;
                    txtDateAndTimeNewNotificationOfAnimalDead.Text = DateAndTimeNewNotificationOfAnimalDead;

                }//wyświetlenie danych gospodarstwa którego dotyczy zgłoszenie 
                reader.Close();
                cs.GetDBConnection().Close(); // zamykanie połączenia        
            }
            else
            {
                //gdy numer stada wpisany ma niepoprawną długość
                MessageBox.Show("Numer stada musi mieć 13 znaków, sprawdź czy jest poprawny");

                //wyświetlanie innego okna window1
                //Window1 wnd = new Window1();
                //wnd.Show();
            }
        }//przycisk szukaj, wpisujemy numer stada i klikamy przycisk szukaj


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
            if (String.IsNullOrWhiteSpace(txtEarTagNumber.Text)) // sprawdzamy cy wszystkie pola są wypełnione
            {
                MessageBox.Show("Uzupełnij numer kolczyka");
                return;
            }
            else if (String.IsNullOrWhiteSpace(txtDateBorn.Text))
            {
                MessageBox.Show("Uzupełnij datę urodzenia");
                return;
            }
            else if (String.IsNullOrWhiteSpace(txtDateDead.Text))
            {
                MessageBox.Show("Uzupełnij datę padnięcia");
                return;
            }
            else if (String.IsNullOrWhiteSpace(txtHourOfDeadAnimal.Text))
            {
                MessageBox.Show("Uzupełnij godzinę padnięcia");
                return;
            }
            else if (String.IsNullOrWhiteSpace(txtHowManyAnimalsInFarm.Text))
            {
                MessageBox.Show("Uzupełnij liczbę sztuk w stadzie");
                return;
            }
            else if (String.IsNullOrWhiteSpace(txtWhoReportingNewNotification.Text)||
                String.IsNullOrWhiteSpace(txtAddressPersonReporting.Text)||
                String.IsNullOrWhiteSpace(txtPhonePersonReporting.Text))
            {
                MessageBox.Show("Uzupełnij dane zgłaszającego");
                return;
            }
        
            Singleton cs = Singleton.Instance; //tworzymy instancję Singletona do połączenia z bazą banych
            cs.GetDBConnection();
            cs.GetDBConnection().Open();
            SqlCommand CommandSQL = cs.GetDBConnection().CreateCommand(); // tworzenie komendy SQl do bazy danych
                                   
            string FarmNumber, DateBorn, HowManyAnimalsInFarm, EarTagNumber, WhyDead, DateDead, HourOfDeadAnimal, TypeOfDeadAnimal, GenderOfDeadAnimal,
                DeadDeterminedOrNot, UtilizationCompany, WhoReportingNewNotification, AddressPersonReporting, PhonePersonReporting,
                WhoGetNewNotification, DateAndTimeNewNotificationOfAnimalDead, TypeOfFarm, Comment;

            WhatTypeOfAnimalDead(); //odczyt jaki gatunek zwierzęcia padł

            DataOfNewNotification(out FarmNumber, out DateBorn, out HowManyAnimalsInFarm, out EarTagNumber, out WhyDead, out DateDead, 
                out HourOfDeadAnimal, out WhoReportingNewNotification, out AddressPersonReporting, out PhonePersonReporting,
                out WhoGetNewNotification, out DateAndTimeNewNotificationOfAnimalDead, out TypeOfFarm, out GenderOfDeadAnimal, 
                out DeadDeterminedOrNot, out UtilizationCompany, out TypeOfDeadAnimal, out Comment); //pobranie z formularza danych zgłoszenia

            
            

            
            //WhatGenderOfDeadAnimal(); // odczyt płci padłego zwierzęcia
            //IsDeadDeterminedOrNot(); // odczyt czy padnięcie ustalone lub nieustalone
            //WhoTakeDeadAnimal(); // odczyt kto zabiera padłe zwierzę 
            
            ConvertDataToCommandSQL(CommandSQL, FarmNumber, DateBorn, HowManyAnimalsInFarm, EarTagNumber, WhyDead, DateDead, HourOfDeadAnimal, TypeOfDeadAnimal, GenderOfDeadAnimal, 
                DeadDeterminedOrNot, UtilizationCompany, DateAndTimeNewNotificationOfAnimalDead, WhoReportingNewNotification, AddressPersonReporting, PhonePersonReporting,
                WhoGetNewNotification, TypeOfFarm);
                                                      
            CommandSQL.CommandText = "INSERT INTO ZGLOSZENIA$(NR_STADA, TYP_STADA, LICZBA_SZTUK, NR_KOLCZYKA, GATUNEK, PLEC, DATA_URODZENIA, DATA_PADNIECIA, GODZINA_PADNIECIA, PRZYCZYNA, " +
                "OPIS_PRZYCZYNA, KTO_ODBIERA, OSOBA_ZGL, ADRES_OSOBY_ZGL, TEL_OSOBY_ZGL, DATA_CZAS_ZGL, KTO_PRZYJMUJE_ZGL) VALUES (@FarmNumber, @TypeOfFarm, @HowManyAnimalsInFarm, @EarTagNumber, @TypeOfDeadAnimal, " +
                "@GenderOfDeadAnimal, @DateBorn, @DateDead, @HourOfDeadAnimal, @DeadDeterminedOrNot, @WhyDead, @UtilizationCompany , @WhoReportingNewNotification, @AddressPersonReporting, " +
                "@PhonePersonReporting, @DateAndTimeNewNotificationOfAnimalDead, @WhoGetNewNotification)";

            try // wykonanie zapytania do bazy
                //wyświetlanie nowego okna z pdfem i wysłanie maili z załącznikiem
            {
                SqlDataReader save = CommandSQL.ExecuteReader();
                cs.GetDBConnection().Close(); // zamykanie połączenia 
                PrintSendMail okno = new PrintSendMail(this);
                okno.Owner = this;
                okno.ShowDialog();

                ClearAlls(); // czyszczenie wszystkoch boxów i pól
            }
            catch (SqlException odbcEx)
            {
                MessageBox.Show("Coś poszło nie tak z zapisem zgłoszenia, trzeba to sprawdzić.");// obsługa bardziej szczegółowych wyjątkóws.GetDBConnection().Close(); // zamykanie połączenia 
                cs.GetDBConnection().Close(); // zamykanie połączenia 
            }
            catch (Exception ex)
            {
                MessageBox.Show("błąd 2"); // obsługa wyjątku głównego 
                cs.GetDBConnection().Close(); // zamykanie połączenia 
            }
            
            string WhatTypeOfAnimalDead()
            {
                if (checkOwca.IsChecked == true)
                {
                    txtTypeOfDeadAnimal.Text = "owca";
                    return txtTypeOfDeadAnimal.Text;
                }
                else if (checkBydlo.IsChecked == true)
                {
                    //chBoxMleczne.Visibility = Visibility.Hidden;
                    txtTypeOfDeadAnimal.Text = "bydlo";
                    return txtTypeOfDeadAnimal.Text;
                }
                else if (checkKoza.IsChecked == true)
                {
                    txtTypeOfDeadAnimal.Text = "koza";
                    return txtTypeOfDeadAnimal.Text;
                }
                
                return txtTypeOfDeadAnimal.Text = "brak";
            }

            ////string WhatGenderOfDeadAnimal()
            //{
            //    if (chBoxFemale.IsChecked==true)
            //    {
            //        GenderOfDeadAnimal = "samica";
            //        return GenderOfDeadAnimal;
            //    }
            //    else if (chBoxMale.IsChecked == true)
            //    {
            //        GenderOfDeadAnimal = "samiec";
            //        return GenderOfDeadAnimal;
            //    }
            //    else
            //    {
            //        return GenderOfDeadAnimal = "brak";
            //    }                
            //}

            //string IsDeadDeterminedOrNot()
            //{
            //    if (chBoxUstalona.IsChecked == true)
            //    {
            //        DeadDeterminedOrNot = "ustalona";
            //        return DeadDeterminedOrNot;
            //    }
            //    else if (chBoxUstalona.IsChecked == true)
            //    {
            //        DeadDeterminedOrNot = "nieustalona";
            //        return DeadDeterminedOrNot;
            //    }
            //    else
            //    {
            //        return DeadDeterminedOrNot = "brak";
            //    }
            //}

            ////string WhoTakeDeadAnimal()
            //{
            //    if (chBoxFarmutil.IsChecked == true)
            //    {
            //        UtilizationCompany = "Farmutil";
            //        return UtilizationCompany;                    
            //    }
            //    else if (chBoxJasta.IsChecked == true)
            //    {
            //        UtilizationCompany = "Jasta";
            //        return UtilizationCompany;
            //    }
            //    else
            //    {
            //        return UtilizationCompany = "Inna";
            //    }
            //}
        }

        private void ClearAlls()
        {
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
            //chBoxUstalona.IsChecked = false;
           // chBoxNieustalona.IsChecked = false;
            //chBoxMleczne.IsChecked = false;
            //chBoxOpasowe.IsChecked = false;
            //chBoxJasta.IsChecked = false;
            //chBoxFarmutil.IsChecked = false;
            txtDateAndTimeNewNotificationOfAnimalDead.Clear();
            txtWhoReportingNewNotification.Clear();
            txtAddressPersonReporting.Clear();
            txtPhonePersonReporting.Clear();
            txtComment.Clear();
        }

        private static void ConvertDataToCommandSQL(SqlCommand CommandSQL, string FarmNumber, string DateBorn,
            string HowManyAnimalsInFarm, string EarTagNumber, string WhyDead, string DateDead, string HourOfDeadAnimal, 
            string TypeOfDeadAnimal, string GenderOfDeadAnimal, string DeadDeterminedOrNot, string UtilizationCompany, 
            string DateAndTimeNewNotificationOfAnimalDead, string WhoReportingNewNotification, string AddressPersonReporting, 
            string PhonePersonReporting, string WhoGetNewNotification, string TypeOfFarm) //konwersja zmiennych zawierających dane zgłoszenia do polecenia SQL
        {
            CommandSQL.Parameters.Add("@FarmNumber", SqlDbType.VarChar).Value = FarmNumber;
            CommandSQL.Parameters.Add("@DateBorn", SqlDbType.VarChar).Value = DateBorn;
            CommandSQL.Parameters.Add("@HowManyAnimalsInFarm", SqlDbType.VarChar).Value = HowManyAnimalsInFarm;
            CommandSQL.Parameters.Add("@EarTagNumber", SqlDbType.VarChar).Value = EarTagNumber;
            CommandSQL.Parameters.Add("@WhyDead", SqlDbType.VarChar).Value = WhyDead;
            CommandSQL.Parameters.Add("@DateDead", SqlDbType.VarChar).Value = DateDead;
            CommandSQL.Parameters.Add("@HourOfDeadAnimal", SqlDbType.VarChar).Value = HourOfDeadAnimal;
            CommandSQL.Parameters.Add("@TypeOfDeadAnimal", SqlDbType.VarChar).Value = TypeOfDeadAnimal;
            CommandSQL.Parameters.Add("@GenderOfDeadAnimal", SqlDbType.VarChar).Value = GenderOfDeadAnimal;
            CommandSQL.Parameters.Add("@DeadDeterminedOrNot", SqlDbType.VarChar).Value = DeadDeterminedOrNot;
            CommandSQL.Parameters.Add("@UtilizationCompany", SqlDbType.VarChar).Value = UtilizationCompany;
            CommandSQL.Parameters.Add("@DateAndTimeNewNotificationOfAnimalDead", SqlDbType.VarChar).Value = DateAndTimeNewNotificationOfAnimalDead;
            CommandSQL.Parameters.Add("@WhoReportingNewNotification", SqlDbType.VarChar).Value = WhoReportingNewNotification;
            CommandSQL.Parameters.Add("@AddressPersonReporting", SqlDbType.VarChar).Value = AddressPersonReporting;
            CommandSQL.Parameters.Add("@PhonePersonReporting", SqlDbType.VarChar).Value = PhonePersonReporting;
            CommandSQL.Parameters.Add("@WhoGetNewNotification", SqlDbType.VarChar).Value = WhoGetNewNotification;
            CommandSQL.Parameters.Add("@TypeOfFarm", SqlDbType.VarChar).Value = TypeOfFarm;
        }

        private void DataOfNewNotification(out string FarmNumber, out string DateBorn, out string HowManyAnimalsInFarm,
            out string EarTagNumber, out string WhyDead, out string DateDead, out string HourOfDeadAnimal,
            out string WhoReportingNewNotification, out string AddressPersonReporting, out string PhonePersonReporting,
            out string WhoGetNewNotification, out string DateAndTimeNewNotificationOfAnimalDead, out string TypeOfFarm,
            out string GenderOfDeadAnimal, out string DeadDeterminedOrNot, out string UtilizationCompany, 
            out string TypeOfDeadAnimal, out string Comment) //pobranie z formularza danych zgłoszenia do zmienych
        {

            EarTagNumber = txtEarTagNumber.Text;
            HowManyAnimalsInFarm = txtHowManyAnimalsInFarm.Text;
            FarmNumber = txtFarmNumber.Text;
            DateBorn = txtDateBorn.Text;
            DateDead = txtDateDead.Text;
            WhyDead = txtWhyDead.Text;
            HourOfDeadAnimal = txtHourOfDeadAnimal.Text;
            WhoReportingNewNotification = txtWhoReportingNewNotification.Text;
            AddressPersonReporting = txtAddressPersonReporting.Text;
            PhonePersonReporting = txtPhonePersonReporting.Text;
            WhoGetNewNotification = comboBox_WhoGetGetNotification.Text;
            DateAndTimeNewNotificationOfAnimalDead = txtDateAndTimeNewNotificationOfAnimalDead.Text;
            TypeOfFarm = comboBox_TypeOfFarm.Text;
            GenderOfDeadAnimal = comboBox_GenderOfDeadAnimal.Text;
            DeadDeterminedOrNot = comboBox_DeadDeterminedOrNot.Text;
            UtilizationCompany = comboBox_UtilizationCompany.Text;
            TypeOfDeadAnimal = txtTypeOfDeadAnimal.Text;
            Comment = txtComment.Text;
        }

             #region CheckBoxAlwaysOnlyOneChecked
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

        private void CheckOwca_Checked(object sender, RoutedEventArgs e)
        {
            checkBydlo.IsChecked = false;
            checkKoza.IsChecked = false;
        }

       //// private void ChBoxUstalona_Checked(object sender, RoutedEventArgs e)
       // {
       //     chBoxNieustalona.IsChecked = false;
       // }

       // //private void ChBoxNieustalona_Checked(object sender, RoutedEventArgs e)
       // {
       //     chBoxUstalona.IsChecked = false;
       // }

        //private void ChBoxMleczne_Checked(object sender, RoutedEventArgs e)
        //{
        //    chBoxOpasowe.IsChecked = false;
        //}

        //private void ChBoxOpasowe_Checked(object sender, RoutedEventArgs e)
        //{
        //    chBoxMleczne.IsChecked = false;
        //}

        //private void ChBoxJasta_Checked(object sender, RoutedEventArgs e)
        //{
        //    chBoxFarmutil.IsChecked = false;
        //}

        //private void ChBoxFarmutil_Checked_1(object sender, RoutedEventArgs e)
        //{
        //    chBoxJasta.IsChecked = false;
        //}
        //private void ChBoxMale_Checked(object sender, RoutedEventArgs e)
        //{
        //    chBoxFemale.IsChecked = false;
        //}

        //private void ChBoxFemale_Checked(object sender, RoutedEventArgs e)
        //{
        //    chBoxMale.IsChecked = false;
        //}

        #endregion /// aby zawsze mógł być zaznaczony tylko jeden checkbox 

      }
}
