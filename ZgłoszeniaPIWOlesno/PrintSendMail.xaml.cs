using sharpPDF;
using sharpPDF.Enumerators;
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
using Microsoft.Office.Interop;
using System.Runtime.InteropServices;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Paragraph = iTextSharp.text.Paragraph;
using Rectangle = iTextSharp.text.Rectangle;
using System.Diagnostics;
using System.ComponentModel;
using NodaTime;
using System.Globalization;
using NodaTime.Text;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;

namespace ZgłoszeniaPIWOlesno
{
    
    /// Logika interakcji dla klasy PrintSendMail.xaml
    public partial class PrintSendMail : Window       
    {
        private MainWindow mainWindow = null;
        //MainWindow mainWindow;
        public PrintSendMail(MainWindow mainWin)
        {
            //this.mainWindow = mainWindow;
            mainWindow = mainWin;
            InitializeComponent();
        }
        static string SavingDateTime = DateTime.Now.ToString("yyyy-MM-dd-hh-mm");
        private void btnGenerateAttachment_Click(object sender, RoutedEventArgs e)
        {
            
            int NumberLastNotification = CheckNumberLastNotification();
            string numberLastNotification = NumberLastNotification.ToString();
            //MessageBox.Show(numberLastNotification);


            string OfficialPositionWhoGetNewNotification;
            OfficialPosition(out OfficialPositionWhoGetNewNotification);

            
            int wiek;
             
            CalculateHowOldIsDeadAnimal(out int HowManyMonthsAnimalLive);
            wiek = HowManyMonthsAnimalLive;
            CreateAttachmentNr7(OfficialPositionWhoGetNewNotification, numberLastNotification, SavingDateTime, HowManyMonthsAnimalLive);
            CreateAttachments(HowManyMonthsAnimalLive, OfficialPositionWhoGetNewNotification, numberLastNotification);
        }

        private void CreateAttachments(int HowManyMonthsAnimalLive, string OfficialPositionWhoGetNewNotification, 
            string numberLastNotification) // w zależności od wieku padłego zwierzęcia generowane są inne załączniki
            {
            if (HowManyMonthsAnimalLive >= 48 && mainWindow.txtTypeOfDeadAnimal.Text == "bydlo")
            {
                CreateAttachmentNr6(OfficialPositionWhoGetNewNotification, HowManyMonthsAnimalLive, numberLastNotification);
                CreateMailWithAttachmentNr6(OfficialPositionWhoGetNewNotification, HowManyMonthsAnimalLive, numberLastNotification);
            }
            else if (HowManyMonthsAnimalLive >= 18 && mainWindow.txtTypeOfDeadAnimal.Text == "koza")
            {
                CreateAttachmentNr6(OfficialPositionWhoGetNewNotification, HowManyMonthsAnimalLive, numberLastNotification);
                CreateMailWithAttachmentNr6(OfficialPositionWhoGetNewNotification, HowManyMonthsAnimalLive, numberLastNotification);
            }
            else if (HowManyMonthsAnimalLive >= 18 && mainWindow.txtTypeOfDeadAnimal.Text == "owca")
            {
                CreateAttachmentNr6(OfficialPositionWhoGetNewNotification, HowManyMonthsAnimalLive, numberLastNotification);
                CreateMailWithAttachmentNr6(OfficialPositionWhoGetNewNotification, HowManyMonthsAnimalLive, numberLastNotification);
            }
        }

        private void CreateMailWithAttachmentNr6(string officialPositionWhoGetNewNotification, int howManyMonthsAnimalLive, string numberLastNotification)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("OUTLOOK");
            int collCount = processes.Length;

            if (collCount != 0)
            {
                Microsoft.Office.Interop.Outlook.Application oApp = Marshal.GetActiveObject("Outlook.Application") as Microsoft.Office.Interop.Outlook.Application;
                Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                mailItem.Subject = "PIW Olesno - zgłoszenia padniecia numer " + numberLastNotification + ".";
                mailItem.To = "wlodara@wiw.opole.pl";
                mailItem.CC = "farmutil@farmutil.pl";
                mailItem.Body = "Zgłoszenie padnięcia. \nPIW Olesno\n" + officialPositionWhoGetNewNotification+"\n"+ mainWindow.comboBox_WhoGetGetNotification.Text;


                Microsoft.Office.Interop.Outlook.Attachments mailAttachments = mailItem.Attachments;
                Microsoft.Office.Interop.Outlook.Attachment newAttachment = mailAttachments.Add(
                @"C:\Users\mojeq\source\repos\ZgłoszeniaPIWOlesno\ZgłoszeniaPIWOlesno\bin\Debug\PDFy\" + mainWindow.txtFarmNumber.Text + "-zal6-" + SavingDateTime + ".pdf",
                Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, 1, "The test attachment");
                mailItem.Save();
                mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceNormal;
                mailItem.Display(false);
                mailItem = null;
                oApp = null;
                MessageBox.Show("Utworzono wiadomość z załącznikiem nr 6.");

            }
        }
        private int CheckNumberLastNotification() // uzyskujemy numer ostatniego padnięcia w powiecie
        {

            Singleton cs = Singleton.Instance;
            cs.GetDBConnection();
            cs.GetDBConnection().Open();
            SqlCommand CommandSQL = cs.GetDBConnection().CreateCommand(); // tworzenie komendy SQl do bazy danych

            CommandSQL.Parameters.Add("@DateAndTimeNewNotificationOfAnimalDead", SqlDbType.VarChar).Value = mainWindow.txtDateAndTimeNewNotificationOfAnimalDead.Text;
            CommandSQL.Parameters.Add("@FarmNumber", SqlDbType.VarChar).Value = mainWindow.txtFarmNumber.Text;
            CommandSQL.CommandText = "SELECT ID FROM ZGLOSZENIA$ WHERE NR_STADA=@FarmNumber and DATA_CZAS_ZGL=@DateAndTimeNewNotificationOfAnimalDead";
            SqlDataReader reader = CommandSQL.ExecuteReader(); // wykonanie zapytania do bazy
            reader.Read();
            string id = reader["ID"].ToString();
            int ID = Convert.ToUInt16(id);
            reader.Close();
            cs.GetDBConnection().Close();
            return ID;

        }

        
        private void CreateAttachmentNr7(string OfficialPositionWhoGetNewNotification, string numberLastNotification, string SavingDateTime, int HowManyMonthsAnimalLive) //tworzymy załącznik numer 7
        {
            
            System.IO.FileStream fs = new FileStream("PDFy/" + mainWindow.txtFarmNumber.Text+"-zal7-"+ SavingDateTime + ".pdf", FileMode.Create);
            // tworzymy instancje klasy dokumentu pdf z wymiarem A4  
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);        
            // klasa writer używająca dokument i strumienia w konstruktorze
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            // Add meta information to the document  
            document.AddAuthor("Micke Blomquist");
            document.AddCreator("Sample application using iTextSharp");
            document.AddKeywords("PDF tutorial education");
            document.AddSubject("Document subject - Describing the steps creating a PDF document");
            document.AddTitle("The document title - PDF creation using iTextSharp");

            // otwórz dokument 
            document.Open();
            // dodajemy zawartość 
            FontFactory.RegisterDirectory("C:WINDOWSFonts"); //dodajemy polskie znaki
            var polskie_znaki = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.EMBEDDED);
 
            PdfPTable table = new PdfPTable(2);
            PdfPCell cell = new PdfPCell(new Phrase("Rejestr zgłoszeń padłego bydła - załącznik 7\n ", polskie_znaki));

            cell.Colspan = 2;
            cell.Border = 0;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell);
            table.AddCell(new Phrase("Numer zgłoszenia", polskie_znaki));
            table.AddCell("1608/"+ numberLastNotification+"/2019");
            table.AddCell(new Phrase("Data i godzina przyjęcia zgłoszenia", polskie_znaki));
            table.AddCell(mainWindow.txtDateAndTimeNewNotificationOfAnimalDead.Text);
            table.AddCell(new Phrase("Powiatowy Inspektorat Weterynarii w ", polskie_znaki));
            table.AddCell(new Phrase("Oleśnie", polskie_znaki));

            PdfPCell cell2 = new PdfPCell(new Phrase("\nOsoba zgłaszająca", polskie_znaki));
            cell2.Colspan = 2;
            cell2.Border = 0;
            cell2.HorizontalAlignment = 0;
            table.AddCell(cell2);
            table.AddCell(new Phrase("Imię i nazwisko", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtWhoReportingNewNotification.Text, polskie_znaki));
            table.AddCell(new Phrase("Adres zam.", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtAddressPersonReporting.Text, polskie_znaki));
            table.AddCell(new Phrase("Telefon", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtPhonePersonReporting.Text, polskie_znaki));

            PdfPCell cell3 = new PdfPCell(new Phrase("\nOsoba przyjmująca zgłoszenie", polskie_znaki));
            cell3.Colspan = 2;
            cell3.Border = 0;
            cell3.HorizontalAlignment = 0;
            table.AddCell(cell3);
            table.AddCell(new Phrase("Imię i nazwisko", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_WhoGetGetNotification.Text, polskie_znaki));
            table.AddCell(new Phrase("Stanowisko służbowe", polskie_znaki));
            table.AddCell(new Phrase(OfficialPositionWhoGetNewNotification, polskie_znaki));

            PdfPCell cell4 = new PdfPCell(new Phrase("\nMiejsce padnięcia zwierzęcia – adres gospodarstwa", polskie_znaki));
            cell4.Colspan = 2;
            cell4.Border = 0;
            cell4.HorizontalAlignment = 0;
            table.AddCell(cell4);
            table.AddCell(new Phrase("Imię i nazwisko posiadacza zwierzęcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtName.Text + ' ' + mainWindow.txtSurname.Text, polskie_znaki));
            table.AddCell(new Phrase("Adres gospodarstwa/nr siedziby stada", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtStreet.Text + ' ' + mainWindow.txtHouseNumber.Text
                + ' ' + mainWindow.txtLocalNumber.Text + ' ' + mainWindow.txtPostCode.Text + ' ' + mainWindow.txtPost.Text
                + '/' + mainWindow.txtFarmNumber.Text, polskie_znaki));
            table.AddCell(new Phrase("miejscowość", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtCity.Text, polskie_znaki));
            table.AddCell(new Phrase("powiat", polskie_znaki));
            table.AddCell(new Phrase("olesno", polskie_znaki));
            table.AddCell(new Phrase("województwo", polskie_znaki));
            table.AddCell(new Phrase("opolskie", polskie_znaki));

            PdfPCell cell5 = new PdfPCell(new Phrase("\nOpis gospodarstwa", polskie_znaki));
            cell5.Colspan = 2;
            cell5.Border = 0;
            cell5.HorizontalAlignment = 0;
            table.AddCell(cell5);
            table.AddCell(new Phrase("Rodzaj produkcji", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_TypeOfFarm.Text, polskie_znaki));
            table.AddCell(new Phrase("Liczba sztuk", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtHowManyAnimalsInFarm.Text, polskie_znaki));

            PdfPCell cell6 = new PdfPCell(new Phrase("\nIdentyfikacja padłego zwierzęcia", polskie_znaki));
            cell6.Colspan = 2;
            cell6.Border = 0;
            cell6.HorizontalAlignment = 0;
            table.AddCell(cell6);
            table.AddCell(new Phrase("nr kolczyka zwierzęcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtFarmNumber.Text, polskie_znaki));
            table.AddCell(new Phrase("data urodzenia i wiek", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtDateBorn.Text + " wiek: " + HowManyMonthsAnimalLive + "miesięcy", polskie_znaki));
            table.AddCell(new Phrase("płeć", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_GenderOfDeadAnimal.Text, polskie_znaki));
            table.AddCell(new Phrase("Data i godzina padnięcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtDateDead.Text + ' ' + mainWindow.txtHourOfDeadAnimal.Text, polskie_znaki));
            table.AddCell(new Phrase("Przyczyna padnięcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_DeadDeterminedOrNot.Text, polskie_znaki));
            table.AddCell(new Phrase("Podać prawdopodobną przyczynę padnięcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtWhyDead.Text, polskie_znaki));
            table.AddCell(new Phrase("Dodatkowe uwagi", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtComment.Text, polskie_znaki));

            PdfPCell cell7 = new PdfPCell(new Phrase("Osoba przyjmująca zgłoszenie: \n " + mainWindow.comboBox_WhoGetGetNotification.Text, polskie_znaki));
            cell7.Colspan = 2;
            cell7.Border = 0;
            cell7.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell7);

            document.Add(table);
            // zamknij dokument 
            document.Close();
            // zamknij writer 
            writer.Close();
            // zakmnij obsługiwane pliki
            fs.Close();
           
            funckcja();
            void funckcja()
            {
                MessageBox.Show("Załącznik/i zapisano, wyślij maile jeśli są wymagane.");
            }
        }
        //todo:potwierdzenie dotarcia i potwierdzenia pobrania dodać na spodzie załącznika     
        private void CreateAttachmentNr6(string OfficialPositionWhoGetNewNotification, int HowManyMonthsAnimalLive, 
            string numerLastNotification) // tworzymy załącznik nr 6 w pdfie
        {
            checkBseOrTseTest(out string testType);
            string SavingDateTime = DateTime.Now.ToString("yyyy-MM-dd-hh-mm");
            System.IO.FileStream fs = new FileStream("PDFy/" + mainWindow.txtFarmNumber.Text +"-zal6-"+SavingDateTime+".pdf", FileMode.Create);
            // tworzymy instancje klasy dokumentu pdf z wymiarem A4  
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // klasa writer używająca dokument i strumienia w konstruktorze
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            // meta informacje o dokumencie
            document.AddAuthor(OfficialPositionWhoGetNewNotification);
            document.AddTitle("Rejestr zgłoszeń padłego bydła - załącznik 6");

            // otwieramy dokument aby dodac do niego zawartość
            document.Open();
            // dodajemy zawartość 
            FontFactory.RegisterDirectory("C:WINDOWSFonts"); //dodajemy polskie znaki
            var polskie_znaki = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            PdfPCell cell = new PdfPCell(new Phrase("Rejestr zgłoszeń padłego bydła - załącznik 6     Olesno "+DateTime.Now.ToString("yyyy-MM-dd")+"\n ", polskie_znaki));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell);

            PdfPCell cell2 = new PdfPCell(new Phrase("\nSkierowanie sztuki padłej/zabitej do ZU/ZP", polskie_znaki));
            cell2.Colspan = 2;
            cell2.Border = 0;
            cell2.HorizontalAlignment = 0;
            table.AddCell(cell2);

            PdfPCell cell3 = new PdfPCell(new Phrase("\nAdresat: (właściwy dla miejsca lokalizacji zakładu utylizacyjnego/zakładu pośredniego)", polskie_znaki));
            cell3.Colspan = 2;
            cell3.Border = 0;
            cell3.HorizontalAlignment = 0;
            table.AddCell(cell3);

            table.AddCell(new Phrase("Powiatowy Lekarz Weterynarii", polskie_znaki));
            table.AddCell(new Phrase(comboBox_PLWUtilizationArea.Text, polskie_znaki));

            PdfPCell cell4 = new PdfPCell(new Phrase("\nNadawca: (właściwy dla miejsca padnięcia/zabicia zwierzęcia)", polskie_znaki));
            cell4.Colspan = 2;
            cell4.Border = 0;
            cell4.HorizontalAlignment = 0;
            table.AddCell(cell4);
            table.AddCell(new Phrase("Powiatowy Lekarz Weterynarii ", polskie_znaki));
            table.AddCell(new Phrase(comboBox_PLWDeadArea.Text, polskie_znaki));

            table.AddCell(new Phrase("Numer zgłoszenia", polskie_znaki));
            table.AddCell("1608/"+numerLastNotification+"/2019");

            table.AddCell(new Phrase("Numer kolczyka", polskie_znaki));
            table.AddCell(mainWindow.txtEarTagNumber.Text);

            table.AddCell(new Phrase("data urodzenia i wiek", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtDateBorn.Text+" wiek: "+HowManyMonthsAnimalLive+"miesięcy" , polskie_znaki));

            table.AddCell(new Phrase("płeć", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_GenderOfDeadAnimal.Text, polskie_znaki));

            table.AddCell(new Phrase("Data i godzina padnięcia/zabicia", polskie_znaki));
            table.AddCell(mainWindow.txtDateDead.Text+" "+mainWindow.txtHourOfDeadAnimal.Text);

            table.AddCell(new Phrase("Imię i nazwisko posiadacza zwierzęcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtName.Text + ' ' + mainWindow.txtSurname.Text, polskie_znaki));

            table.AddCell(new Phrase("Adres gospodarstwa/nr siedziby stada", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtStreet.Text + ' ' + mainWindow.txtHouseNumber.Text
                + ' ' + mainWindow.txtLocalNumber.Text + ' ' + mainWindow.txtPostCode.Text + ' ' + mainWindow.txtPost.Text
                + '/' + mainWindow.txtFarmNumber.Text, polskie_znaki));
            table.AddCell(new Phrase("miejscowość", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtCity.Text, polskie_znaki));
            table.AddCell(new Phrase("powiat", polskie_znaki));
            table.AddCell(new Phrase("olesno", polskie_znaki));
            table.AddCell(new Phrase("województwo", polskie_znaki));
            table.AddCell(new Phrase("opolskie", polskie_znaki));
 
            table.AddCell(new Phrase("Imię i nazwisko", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtWhoReportingNewNotification.Text, polskie_znaki));
            table.AddCell(new Phrase("Adres zam.", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtAddressPersonReporting.Text, polskie_znaki));
            table.AddCell(new Phrase("Telefon", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtPhonePersonReporting.Text, polskie_znaki));

            table.AddCell(new Phrase("Przyczyna padnięcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_DeadDeterminedOrNot.Text , polskie_znaki));

            table.AddCell(new Phrase("Prawdopodobna przyczyna padnięcia: ", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtWhyDead.Text , polskie_znaki));

            PdfPCell cell5 = new PdfPCell(new Phrase("\nProszę o przesłanie próbek do badań w kierunku "+testType+" z adnotacją o potrzebie przesłania wyniku " +
                "badania faksem na nr 34 358 26 18 oraz drogą elektroniczną piw.olesno@wiw.opole.pl. " +
                "Kosztami badań należy obciążyć budżet wojewódzkiego inspektoratu weterynarii "+comboBox_WIW.Text+" (WIW właściwy dla miejsca pobrania próbki do badania " +
                "w kierunku BSE).", polskie_znaki));
            cell5.Colspan = 2;
            cell5.Border = 0;
            cell5.HorizontalAlignment = 0;
            table.AddCell(cell5);


            PdfPCell cell7 = new PdfPCell(new Phrase("Osoba przyjmująca zgłoszenie: \n " + mainWindow.comboBox_WhoGetGetNotification.Text, polskie_znaki));
            cell7.Colspan = 2;
            cell7.Border = 0;
            cell7.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell7);

            document.Add(table);
            // zamknij dokument 
            document.Close();
            // zamknij writer 
            writer.Close();
            // zakmnij obsługiwane pliki
            fs.Close();
        }

        private void checkBseOrTseTest(out string testType) // selekcja badanie BSE/TSE w zależności od tego co padło
        {
            testType = null;
            if (mainWindow.txtTypeOfDeadAnimal.Text == "bydlo")
            {
                string test = "BSE";
                testType = test;
            }
            else if(mainWindow.txtTypeOfDeadAnimal.Text == "koza"|| mainWindow.txtTypeOfDeadAnimal.Text == "owca")
            {
                string test = "TSE";
                testType = test;
            }           
        }

        private void CalculateHowOldIsDeadAnimal(out int HowManyMonthsAnimalLive) // liczymy ile miesięcy miało padłe zwierzę
        {
            
            DateTime DateDead = DateTime.Parse(mainWindow.DateDead.Text);
            DateTime DateBorn = DateTime.Parse(mainWindow.DateBorn.Text);

            string tempYearBorn = DateBorn.ToString("yyyy");
            string tempMonthBorn = DateBorn.ToString("MM");
            string tempDayBorn = DateBorn.ToString("dd");
            int YearBorn = System.Convert.ToInt16(tempYearBorn);
            int MonthBorn = System.Convert.ToInt16(tempMonthBorn);
            int DayBorn = System.Convert.ToInt16(tempDayBorn);

            string tempYearDead = DateDead.ToString("yyyy");
            string tempMonthDead = DateDead.ToString("MM");
            string tempDayDead = DateDead.ToString("dd");
            int YearDead = System.Convert.ToInt16(tempYearDead);
            int MonthDead = System.Convert.ToInt16(tempMonthDead);
            int DayDead = System.Convert.ToInt16(tempDayDead);

            LocalDate WhenAnimalBorn = new LocalDate(YearBorn, MonthBorn, DayBorn);
            LocalDate WhenAnimalDead = new LocalDate(YearDead, MonthDead, DayDead);

            Period tempMonths = Period.Between(WhenAnimalBorn, WhenAnimalDead, PeriodUnits.Months);
            HowManyMonthsAnimalLive = tempMonths.Months;
            string tempHowManyMonthsAnimalLive = HowManyMonthsAnimalLive.ToString();


            //MessageBox.Show(tempHowManyMonthsAnimalLive);
        }

        private void OfficialPosition(out string OfficialPositionWhoGetNewNotification) // osoba przyjmująca zgłoszenie
        {
            OfficialPositionWhoGetNewNotification = null;
            string WhoGetGetNotification = mainWindow.comboBox_WhoGetGetNotification.Text;
            switch (WhoGetGetNotification)
            {
                case "Gabriela Gallus":
                    OfficialPositionWhoGetNewNotification = "Starszy referent ds. administracyjnych";            
                    break;            
            
                case "Joanna Frankiewicz":
                    OfficialPositionWhoGetNewNotification = "Inspektor ds. dobrostanu zwierząt";
                    break;

                case "Piotr Moj":
                    OfficialPositionWhoGetNewNotification = "Informatyk";
                    break;

                case "Izabela Glomb":
                    OfficialPositionWhoGetNewNotification = "Inspektor ds. pasz i utylizacji";
                    break;

                case "Krzysztof Chyra":
                    OfficialPositionWhoGetNewNotification = "Zastępca PLW";
                    break;

                case "Łukasz Kościelny":
                    OfficialPositionWhoGetNewNotification = "Inspektor ds. chorób zakaźnych";
                    break;

                case "Sebastian Konwant":
                    OfficialPositionWhoGetNewNotification = "Powiatowy Lekarz Weterynarii";
                    break;

                case "Katarzyna Lech":
                    OfficialPositionWhoGetNewNotification = "Inspektor ds. higieny zwierząt";
                    break;

                case "Urszula Tylak":
                    OfficialPositionWhoGetNewNotification = "Kontroler weterynaryjny";
                    break;

                case "Małgorzata Wychrystenko":
                    OfficialPositionWhoGetNewNotification = "Zastępca głównej księgowej";
                    break;

                case "Anna Kała":
                    OfficialPositionWhoGetNewNotification = "Główna księgowa";
                    break;
            }

        }

        //public void btnSendMail_Click( object sender, RoutedEventArgs e) // wysłanie maili z załącznikami
        //{
        //    CalculateHowOldIsDeadAnimal(out int HowManyMonthsAnimalLive);
        //    if (HowManyMonthsAnimalLive > 48)
        //    {
        //        System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("OUTLOOK");
        //        int collCount = processes.Length;

        //        if (collCount != 0)
        //        {
        //            Microsoft.Office.Interop.Outlook.Application oApp = Marshal.GetActiveObject("Outlook.Application") as Microsoft.Office.Interop.Outlook.Application;
        //            Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
        //            mailItem.Subject = "PIW Olesno - zgłoszenia padniecia.";
        //            mailItem.To = "wlodara@wiw.opole.pl";
        //            mailItem.CC = "farmutil@farmutil.pl";
        //            mailItem.Body = "Zgłoszenie padnięcia. \nPIW Olesno";

        //            try
        //            {

        //                Microsoft.Office.Interop.Outlook.Attachments mailAttachments = mailItem.Attachments;
        //                Microsoft.Office.Interop.Outlook.Attachment newAttachment = mailAttachments.Add(
        //                @"C:\Users\mojeq\source\repos\ZgłoszeniaPIWOlesno\ZgłoszeniaPIWOlesno\bin\Debug\PDFy\" + mainWindow.txtFarmNumber.Text + "-zal6-" + SavingDateTime + ".pdf",
        //                Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, 1, "The test attachment");
        //                mailItem.Save();
        //                mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceNormal;
        //                mailItem.Display(false);
        //                mailItem = null;
        //                oApp = null;
        //                MessageBox.Show("Utworzono wiadomość z załącznikiem nr 6.");
        //            }
        //            catch
        //            {
        //                MessageBox.Show("Nie ma potrzeby wysłania załącznika, padła sztuka za młoda.");
        //            }
        //            // don't forget to release underlying COM objects
        //            // if (newAttachment != null) Marshal.ReleaseComObject(newAttachment);
        //            // if (mailAttachments != null) Marshal.ReleaseComObject(mailAttachments);
        //            //  Marshal.ReleaseComObject(mailItem);

        //        }

        //    }
        //    else
        //    {
        //        MessageBox.Show("Nie ma potrzeby wysłania załącznika, padła sztuka za młoda.");
        //    }
        //    //funckcja();
        //    //void funckcja()
        //    //{
        //    //    MessageBox.Show("Wysłano załącznik.");
        //    //}

        //}

    }
}
