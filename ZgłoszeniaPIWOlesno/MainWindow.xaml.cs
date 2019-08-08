using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace ZgłoszeniaPIWOlesno
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }  
        public void BtnSzukaj_Click(object sender, RoutedEventArgs e)
        {

            if (txtFarmNumber.Text.Length == 13)
            {
                if (checkOwca.IsChecked == false && checkKoza.IsChecked == false && checkBydlo.IsChecked == false)
                {
                    MessageBox.Show("Nie wybrano gatunku bydło/owaca/koza");
                    return;
                }
                string FarmNumber = txtFarmNumber.Text;

                NewNotification.Visibility = Visibility.Visible; //wyświetlenie na prawej stronie Uzupełnij dane zgłoszenia

                //wyszukanie gospodarstwa w bazie za pomocą Entity Framework
                BAZA_ARIMREntities db = new BAZA_ARIMREntities();
                try
                {
                    BAZA_GOSPODARSTWA_ farm = db.BAZA_GOSPODARSTWA_.First(a => a.NR_STADA == FarmNumber);
                    txtSurname.Text = farm.NAZWISKO_LUB_NAZWA;
                    txtName.Text = farm.IMIE_LUB_NAZWA_SKROCONA;
                    txtFarmNumberSaveNofification.Text = farm.NR_STADA;
                    txtHowManyAnimalsInFarm.Text = farm.LICZBA_SZTUK.ToString();
                    txtCity.Text = farm.MIEJSCOWOSC;
                    txtStreet.Text = farm.ULICA;
                    txtHouseNumber.Text = farm.POSESJA.ToString();
                    txtLocalNumber.Text = farm.LOKAL;
                    txtPostCode.Text = farm.KOD_POCZTOWY;
                    txtPost.Text = farm.POCZTA;
                    txtWhoReportingNewNotification.Text = farm.IMIE_LUB_NAZWA_SKROCONA + ' ' + farm.NAZWISKO_LUB_NAZWA;
                    txtAddressPersonReporting.Text = farm.MIEJSCOWOSC + ' ' + farm.ULICA + ' ' + farm.POSESJA.ToString()
                        + ' ' + farm.LOKAL + ' ' + farm.KOD_POCZTOWY + ' ' + farm.POCZTA;

                    string DateNewNotificationOfAnimalDead = DateTime.Now.ToString("yyyy-MM-dd"); // aktualna data - data zgłoszenia padnięcia  
                    string TimeNewNotificationOfAnimalDead = DateTime.Now.ToString("hh:mm"); // aktulna godzina - godzina zgłoszenia
                    string DateAndTimeNewNotificationOfAnimalDead = DateNewNotificationOfAnimalDead + " " + TimeNewNotificationOfAnimalDead;
                    txtDateAndTimeNewNotificationOfAnimalDead.Text = DateAndTimeNewNotificationOfAnimalDead;
                    //}
                }
                catch (SqlException odbcEx)
                {
                    MessageBox.Show("Coś poszło nie tak z wyszukaniem gospodarstwa(SQL).");// obsługa bardziej szczegółowych wyjątkóws.GetDBConnection().Close(); // zamykanie połączenia 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd, przerwano działanie."); // obsługa wyjątku głównego                  
                }
                //

                //wyszukanie gospodarstwa w bazie za pomocą zapytania SQL
                //Singleton cs = Singleton.Instance; //tworzymy instancję Singletona do połączenia z bazą banych
                //cs.GetDBConnection();
                //cs.GetDBConnection().Open();

                //SqlCommand CommandSQL = cs.GetDBConnection().CreateCommand();

                //CommandSQL.Parameters.Add("@FarmNumber", SqlDbType.VarChar).Value = FarmNumber; //przypisane numeru stada(FarmNumber) w zapytaniu SQL

                //CommandSQL.CommandText = "SELECT * FROM BAZA_GOSPODARSTWA$ WHERE NR_STADA=@FarmNumber";
                //SqlDataReader reader = CommandSQL.ExecuteReader(); // wykonanie zapytania do bazy

                //if (!reader.HasRows)
                //{
                //    MessageBox.Show("Nie ma takiego numeru w bazie, sprawdź wprowadzony numer lub wprowadź dane gospodarstwa ręcznie.");
                //} // sprawdzam czy znaleziono jakiś rekord w bazie

                //while (reader.Read())
                //{
                //    txtSurname.Text = reader["NAZWISKO_LUB_NAZWA"].ToString();
                //    txtName.Text = reader["IMIE_LUB_NAZWA_SKROCONA"].ToString();
                //    txtFarmNumberSaveNofification.Text = reader["NR_STADA"].ToString();
                //    txtHowManyAnimalsInFarm.Text = reader["LICZBA_SZTUK"].ToString();
                //    txtCity.Text = reader["MIEJSCOWOSC"].ToString();
                //    txtStreet.Text = reader["ULICA"].ToString();
                //    txtHouseNumber.Text = reader["POSESJA"].ToString();
                //    txtLocalNumber.Text = reader["LOKAL"].ToString();
                //    txtPostCode.Text = reader["KOD_POCZTOWY"].ToString();
                //    txtPost.Text = reader["POCZTA"].ToString();
                //    txtWhoReportingNewNotification.Text = reader["IMIE_LUB_NAZWA_SKROCONA"].ToString()+' '+reader["NAZWISKO_LUB_NAZWA"].ToString();
                //    txtAddressPersonReporting.Text= reader["MIEJSCOWOSC"].ToString()+' '+reader["ULICA"].ToString()+' '+reader["POSESJA"].ToString()
                //        +' '+reader["LOKAL"].ToString()+' '+reader["KOD_POCZTOWY"].ToString()+' '+reader["POCZTA"].ToString();

                //    string DateNewNotificationOfAnimalDead = DateTime.Now.ToString("yyyy-MM-dd"); // aktualna data - data zgłoszenia padnięcia  
                //    string TimeNewNotificationOfAnimalDead = DateTime.Now.ToString("hh:mm"); // aktulna godzina - godzina zgłoszenia
                //    string DateAndTimeNewNotificationOfAnimalDead = DateNewNotificationOfAnimalDead + " " + TimeNewNotificationOfAnimalDead;
                //    txtDateAndTimeNewNotificationOfAnimalDead.Text = DateAndTimeNewNotificationOfAnimalDead;

                //}//wyświetlenie danych gospodarstwa którego dotyczy zgłoszenie 
                //reader.Close();
                //cs.GetDBConnection().Close(); // zamykanie połączenia        
            }
            else
            {
                //gdy numer stada wpisany ma niepoprawną długość
                MessageBox.Show("Numer stada musi mieć 13 znaków, sprawdź czy jest poprawny");
            }
        }//przycisk szukaj, wpisujemy numer stada i klikamy przycisk szukaj

        DateTime data = new DateTime();
        public void DatePickerDateBornAnimal_SelectedDateChanged(object sender, SelectionChangedEventArgs e) // Data picker - data urodzenia zwierzęcia
        {            
            var picker = sender as DatePicker; // referencja Data picker
            
            DateTime? date = picker.SelectedDate; /// pozyskanie daty nullable z SelectedDate
            if (date == null)
            {
                this.Title = "Brak daty";// gdy nie ma daty
                data = date.Value;
            }
            else
            {
                this.Title = date.Value.ToString("yyyy-MM-dd"); // konwersja daty na string
                txtDateBorn.Text = this.Title; // zapis daty w boxie DataBorn
                data = date.Value;
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
            else if (String.IsNullOrWhiteSpace(txtWhoReportingNewNotification.Text) ||
                String.IsNullOrWhiteSpace(txtAddressPersonReporting.Text))               
            {
                MessageBox.Show("Uzupełnij dane zgłaszającego");
                return;
            }

            Singleton cs = Singleton.Instance; //tworzymy instancję Singletona do połączenia z bazą banych
            cs.GetDBConnection();
            cs.GetDBConnection().Open();
            SqlCommand CommandSQL = cs.GetDBConnection().CreateCommand(); // tworzenie komendy SQl do bazy danych

            string farmNumber, dateBorn, howManyAnimalsInFarm, earTagNumber, whyDead, dateDead, hourOfDeadAnimal, typeOfDeadAnimal, genderOfDeadAnimal,
                deadDeterminedOrNot, utilizationCompany, whoReportingNewNotification, addressPersonReporting, phonePersonReporting,
                whoGetNewNotification, dateAndTimeNewNotificationOfAnimalDead, typeOfFarm, comment;

            WhatTypeOfAnimalDead(); //odczyt jaki gatunek zwierzęcia padł

            DataOfNewNotification(out farmNumber, out dateBorn, out howManyAnimalsInFarm, out earTagNumber, out whyDead, out dateDead,
                out hourOfDeadAnimal, out whoReportingNewNotification, out addressPersonReporting, out phonePersonReporting,
                out whoGetNewNotification, out dateAndTimeNewNotificationOfAnimalDead, out typeOfFarm, out genderOfDeadAnimal,
                out deadDeterminedOrNot, out utilizationCompany, out typeOfDeadAnimal, out comment); //pobranie z formularza danych zgłoszenia

            ConvertDataToCommandSQL(CommandSQL, farmNumber, dateBorn, howManyAnimalsInFarm, earTagNumber, whyDead, dateDead, hourOfDeadAnimal, typeOfDeadAnimal, genderOfDeadAnimal,
                deadDeterminedOrNot, utilizationCompany, dateAndTimeNewNotificationOfAnimalDead, whoReportingNewNotification, addressPersonReporting, phonePersonReporting,
                whoGetNewNotification, typeOfFarm);

            CommandSQL.CommandText = "INSERT INTO ZGLOSZENIA$(NR_STADA, TYP_STADA, LICZBA_SZTUK, NR_KOLCZYKA, GATUNEK, PLEC, DATA_URODZENIA, DATA_PADNIECIA, GODZINA_PADNIECIA, PRZYCZYNA, " +
                "OPIS_PRZYCZYNA, KTO_ODBIERA, OSOBA_ZGL, ADRES_OSOBY_ZGL, TEL_OSOBY_ZGL, DATA_CZAS_ZGL, KTO_PRZYJMUJE_ZGL) VALUES (@farmNumber, @typeOfFarm, @howManyAnimalsInFarm, @earTagNumber, @typeOfDeadAnimal, " +
                "@genderOfDeadAnimal, @dateBorn, @dateDead, @hourOfDeadAnimal, @deadDeterminedOrNot, @whyDead, @utilizationCompany , @whoReportingNewNotification, @addressPersonReporting, " +
                "@phonePersonReporting, @dateAndTimeNewNotificationOfAnimalDead, @whoGetNewNotification)";

            try // wykonanie zapytania do bazy
                //wyświetlanie nowego okna z pdfem i wysyłanie maili z załącznikiem
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

            //try
            //{
            //    //zapis do bazy za pomocą Entity Framework
            //    BAZA_ARIMREntities db2 = new BAZA_ARIMREntities();
            //    ZGLOSZENIA_ newItem = new ZGLOSZENIA_();
            //    newItem.NR_STADA = FarmNumber;
            //    newItem.TYP_STADA = TypeOfFarm;
            //    newItem.LICZBA_SZTUK = Convert.ToDouble(HowManyAnimalsInFarm);
            //    newItem.NR_KOLCZYKA = EarTagNumber;
            //    newItem.GATUNEK = TypeOfDeadAnimal;
            //    newItem.PLEC = GenderOfDeadAnimal;
            //    DateTime convert_date = DateTime.ParseExact("2000-12-12", "yyyy-MM-dd", CultureInfo.InvariantCulture);

            //    newItem.DATA_URODZENIA = convert_date;
            //    newItem.DATA_PADNIECIA = convert_date;
            //    newItem.GODZINA_PADNIECIA = HourOfDeadAnimal;
            //    newItem.PRZYCZYNA = DeadDeterminedOrNot;
            //    newItem.OPIS_PRZYCZYNA = WhyDead;
            //    newItem.KTO_ODBIERA = UtilizationCompany;
            //    newItem.OSOBA_ZGL = WhoReportingNewNotification;
            //    newItem.ADRES_OSOBY_ZGL = AddressPersonReporting;
            //    newItem.TEL_OSOBY_ZGL = PhonePersonReporting;
            //    newItem.DATA_CZAS_ZGL = DateAndTimeNewNotificationOfAnimalDead;
            //    newItem.KTO_PRZYJMUJE_ZGL  = WhoGetNewNotification;

            //    db2.ZGLOSZENIA_.Add(newItem);
            //    db2.SaveChanges();

            //    PrintSendMail okno = new PrintSendMail(this);// otwieramy nowe okno
            //    okno.Owner = this;
            //    okno.ShowDialog();

            //    ClearAlls(); // czyszczenie wszystkoch boxów i pól
            //}
            //catch (SqlException odbcEx)
            //{
            //    MessageBox.Show("Coś poszło nie tak z zapisem zgłoszenia, trzeba to sprawdzić.");// obsługa bardziej szczegółowych wyjątkóws.GetDBConnection().Close(); // zamykanie połączenia 
            //   // cs.GetDBConnection().Close(); // zamykanie połączenia 
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("błąd 2"); // obsługa wyjątku głównego 
            //   // cs.GetDBConnection().Close(); // zamykanie połączenia 
            //}


            //koniec zapisu Entity Framework



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
            txtDateAndTimeNewNotificationOfAnimalDead.Clear();
            txtWhoReportingNewNotification.Clear();
            txtAddressPersonReporting.Clear();
            txtPhonePersonReporting.Clear();
            txtComment.Clear();
        }

        private static void ConvertDataToCommandSQL(SqlCommand CommandSQL, string farmNumber, string dateBorn,
            string howManyAnimalsInFarm, string earTagNumber, string whyDead, string dateDead, string hourOfDeadAnimal,
            string typeOfDeadAnimal, string genderOfDeadAnimal, string deadDeterminedOrNot, string utilizationCompany,
            string dateAndTimeNewNotificationOfAnimalDead, string whoReportingNewNotification, string addressPersonReporting,
            string phonePersonReporting, string whoGetNewNotification, string typeOfFarm) //konwersja zmiennych zawierających dane zgłoszenia do polecenia SQL
        {
            CommandSQL.Parameters.Add("@farmNumber", SqlDbType.VarChar).Value = farmNumber;
            CommandSQL.Parameters.Add("@dateBorn", SqlDbType.VarChar).Value = dateBorn;
            CommandSQL.Parameters.Add("@howManyAnimalsInFarm", SqlDbType.VarChar).Value = howManyAnimalsInFarm;
            CommandSQL.Parameters.Add("@earTagNumber", SqlDbType.VarChar).Value = earTagNumber;
            CommandSQL.Parameters.Add("@whyDead", SqlDbType.VarChar).Value = whyDead;
            CommandSQL.Parameters.Add("@dateDead", SqlDbType.VarChar).Value = dateDead;
            CommandSQL.Parameters.Add("@hourOfDeadAnimal", SqlDbType.VarChar).Value = hourOfDeadAnimal;
            CommandSQL.Parameters.Add("@typeOfDeadAnimal", SqlDbType.VarChar).Value = typeOfDeadAnimal;
            CommandSQL.Parameters.Add("@genderOfDeadAnimal", SqlDbType.VarChar).Value = genderOfDeadAnimal;
            CommandSQL.Parameters.Add("@deadDeterminedOrNot", SqlDbType.VarChar).Value = deadDeterminedOrNot;
            CommandSQL.Parameters.Add("@utilizationCompany", SqlDbType.VarChar).Value = utilizationCompany;
            CommandSQL.Parameters.Add("@dateAndTimeNewNotificationOfAnimalDead", SqlDbType.VarChar).Value = dateAndTimeNewNotificationOfAnimalDead;
            CommandSQL.Parameters.Add("@whoReportingNewNotification", SqlDbType.VarChar).Value = whoReportingNewNotification;
            CommandSQL.Parameters.Add("@addressPersonReporting", SqlDbType.VarChar).Value = addressPersonReporting;
            CommandSQL.Parameters.Add("@phonePersonReporting", SqlDbType.VarChar).Value = phonePersonReporting;
            CommandSQL.Parameters.Add("@whoGetNewNotification", SqlDbType.VarChar).Value = whoGetNewNotification;
            CommandSQL.Parameters.Add("@typeOfFarm", SqlDbType.VarChar).Value = typeOfFarm;
        }

        private void DataOfNewNotification(out string farmNumber, out string dateBorn, out string howManyAnimalsInFarm,
            out string earTagNumber, out string whyDead, out string dateDead, out string hourOfDeadAnimal,
            out string whoReportingNewNotification, out string addressPersonReporting, out string phonePersonReporting,
            out string whoGetNewNotification, out string dateAndTimeNewNotificationOfAnimalDead, out string typeOfFarm,
            out string genderOfDeadAnimal, out string deadDeterminedOrNot, out string utilizationCompany,
            out string typeOfDeadAnimal, out string comment) //pobranie z formularza danych zgłoszenia do zmienych
        {

            earTagNumber = txtEarTagNumber.Text;
            howManyAnimalsInFarm = txtHowManyAnimalsInFarm.Text;
            farmNumber = txtFarmNumber.Text;
            dateBorn = txtDateBorn.Text;
            dateDead = txtDateDead.Text;
            whyDead = txtWhyDead.Text;
            hourOfDeadAnimal = txtHourOfDeadAnimal.Text;
            whoReportingNewNotification = txtWhoReportingNewNotification.Text;
            addressPersonReporting = txtAddressPersonReporting.Text;
            phonePersonReporting = txtPhonePersonReporting.Text;
            whoGetNewNotification = comboBox_WhoGetGetNotification.Text;
            dateAndTimeNewNotificationOfAnimalDead = txtDateAndTimeNewNotificationOfAnimalDead.Text;
            typeOfFarm = comboBox_TypeOfFarm.Text;
            genderOfDeadAnimal = comboBox_GenderOfDeadAnimal.Text;
            deadDeterminedOrNot = comboBox_DeadDeterminedOrNot.Text;
            utilizationCompany = comboBox_UtilizationCompany.Text;
            typeOfDeadAnimal = txtTypeOfDeadAnimal.Text;
            comment = txtComment.Text;
        }
        
        // aby tylko jeden checkbox mógł być zaznaczony 
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
    }
}
