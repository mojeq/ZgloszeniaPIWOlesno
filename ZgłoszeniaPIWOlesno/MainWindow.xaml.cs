using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Threading;

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
                    Thread.Sleep(1000);
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
            else
            {
                //gdy numer stada wpisany ma niepoprawną długość
                MessageBox.Show("Numer stada musi mieć 13 znaków, sprawdź czy jest poprawny");
            }
        }//przycisk szukaj, wpisujemy numer stada i klikamy przycisk szukaj

        // pola w formularzu, data urodzenia i data padnięcia zwierzęcia
        DateTime data = new DateTime();
        public void DatePickerDateBornAnimal_SelectedDateChanged(object sender, SelectionChangedEventArgs e) // Data picker - data urodzenia zwierzęcia
        {            
            var picker = sender as DatePicker;             
            DateTime? date = picker.SelectedDate; 
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
            var picker = sender as DatePicker; 
            DateTime? date = picker.SelectedDate; 
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
            if (String.IsNullOrWhiteSpace(txtEarTagNumber.Text)) // sprawdzamy czy wszystkie pola są wypełnione
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

            string farmNumber, dateBorn, howManyAnimalsInFarm, earTagNumber, whyDead, dateDead, hourOfDeadAnimal, typeOfDeadAnimal, genderOfDeadAnimal,
                deadDeterminedOrNot, utilizationCompany, whoReportingNewNotification, addressPersonReporting, phonePersonReporting,
                whoGetNewNotification, dateAndTimeNewNotificationOfAnimalDead, typeOfFarm, comment;

            WhatTypeOfAnimalDead(); //odczyt jaki gatunek zwierzęcia padł

            DataOfNewNotification(out farmNumber, out dateBorn, out howManyAnimalsInFarm, out earTagNumber, out whyDead, out dateDead,
                out hourOfDeadAnimal, out whoReportingNewNotification, out addressPersonReporting, out phonePersonReporting,
                out whoGetNewNotification, out dateAndTimeNewNotificationOfAnimalDead, out typeOfFarm, out genderOfDeadAnimal,
                out deadDeterminedOrNot, out utilizationCompany, out typeOfDeadAnimal, out comment); //pobranie z formularza danych zgłoszenia
       
            try
            {
                //zapis do bazy za pomocą Entity Framework
                using (var db2 = new BAZA_ARIMREntities())
                {
                    ZGLOSZENIA_ newItem = new ZGLOSZENIA_();
                    newItem.ID = 1;
                    newItem.NR_STADA = farmNumber;
                    newItem.TYP_STADA = typeOfFarm;
                    newItem.LICZBA_SZTUK = Convert.ToDouble(howManyAnimalsInFarm);
                    newItem.NR_KOLCZYKA = earTagNumber;
                    newItem.GATUNEK = typeOfDeadAnimal;
                    newItem.PLEC = genderOfDeadAnimal;
                    newItem.DATA_URODZENIA = dateBorn;
                    newItem.DATA_PADNIECIA = dateDead;
                    newItem.GODZINA_PADNIECIA = hourOfDeadAnimal;
                    newItem.PRZYCZYNA = deadDeterminedOrNot;
                    newItem.OPIS_PRZYCZYNA = whyDead;
                    newItem.KTO_ODBIERA = utilizationCompany;
                    newItem.OSOBA_ZGL = whoReportingNewNotification;
                    newItem.ADRES_OSOBY_ZGL = addressPersonReporting;
                    newItem.TEL_OSOBY_ZGL = phonePersonReporting;
                    newItem.DATA_CZAS_ZGL = dateAndTimeNewNotificationOfAnimalDead;
                    newItem.KTO_PRZYJMUJE_ZGL = whoGetNewNotification;

                    db2.ZGLOSZENIA_.Add(newItem);
                    db2.SaveChanges();
                }
                PrintSendMail okno = new PrintSendMail(this);// otwieramy nowe okno
                okno.Owner = this;
                okno.ShowDialog();

                ClearAlls(); // czyszczenie wszystkoch boxów i pól
            }
            catch (SqlException odbcEx)
            {
                MessageBox.Show("Coś poszło nie tak z zapisem zgłoszenia, trzeba to sprawdzić.");// obsługa bardziej szczegółowych wyjątków                                                                                                 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd 2"); // obsługa wyjątku głównego                                           
            }
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
        private void ClearAlls() //czyszczenie pól
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
        // wyszukiwanie zgloszeń w historii
        private void btnSearchNotification_Click(object sender, RoutedEventArgs e)
        {
            string farmNumber = txtFarmNumberSearch.Text;
            if (string.IsNullOrEmpty(farmNumber) || farmNumber.Length != 13)
            {
                MessageBox.Show("Wpisz poprawny numer stada PL... (13 znaków).");
            }
            else
            {
                SearchNotification search = new SearchNotification(this);
                search.Owner=this;
                search.ShowDialog();                
            }
        }
    }
}
