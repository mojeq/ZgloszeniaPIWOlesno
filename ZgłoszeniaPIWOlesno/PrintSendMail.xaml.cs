using System;
using System.Windows;
using System.Runtime.InteropServices;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using NodaTime;
using System.Data.SqlClient;
using System.Data;

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
        static string savingDateTime = DateTime.Now.ToString("yyyy-MM-dd-hh-mm");
        private void btnGenerateAttachment_Click(object sender, RoutedEventArgs e)
        {
            string savePath = "PDFy/";
            string attachmentPath = @"C:\Users\mojeq\source\repos\ZgłoszeniaPIWOlesno\ZgłoszeniaPIWOlesno\bin\Debug\PDFy\";
            int numberLastNotificationInt = CheckNumberLastNotification();
            string numberLastNotification = numberLastNotificationInt.ToString();

            string officialPositionWhoGetNewNotification;
            OfficialPosition(out officialPositionWhoGetNewNotification);
            
            int wiek;
             
            CalculateHowOldIsDeadAnimal(out int howManyMonthsAnimalLive);
            wiek = howManyMonthsAnimalLive;
            CreateAttachmentNr7(officialPositionWhoGetNewNotification, numberLastNotification, howManyMonthsAnimalLive, savePath);
            CreateAttachments(howManyMonthsAnimalLive, officialPositionWhoGetNewNotification, numberLastNotification, savePath, attachmentPath);
        }

        private void CreateAttachments(int howManyMonthsAnimalLive, string officialPositionWhoGetNewNotification, 
            string numberLastNotification, string savePath, string attachmentPath) // w zależności od wieku padłego zwierzęcia generowane są inne załączniki
            {
            if (howManyMonthsAnimalLive >= 48 && mainWindow.txtTypeOfDeadAnimal.Text == "bydlo")
            {
                CreateAttachmentNr6(officialPositionWhoGetNewNotification, howManyMonthsAnimalLive, numberLastNotification, savePath);
                CreateMailWithAttachmentNr6(officialPositionWhoGetNewNotification, numberLastNotification, attachmentPath);
            }
            else if (howManyMonthsAnimalLive >= 18 && mainWindow.txtTypeOfDeadAnimal.Text == "koza")
            {
                CreateAttachmentNr6(officialPositionWhoGetNewNotification, howManyMonthsAnimalLive, numberLastNotification, savePath);
                CreateMailWithAttachmentNr6(officialPositionWhoGetNewNotification, numberLastNotification, attachmentPath);
            }
            else if (howManyMonthsAnimalLive >= 18 && mainWindow.txtTypeOfDeadAnimal.Text == "owca")
            {
                CreateAttachmentNr6(officialPositionWhoGetNewNotification, howManyMonthsAnimalLive, numberLastNotification, savePath);
                CreateMailWithAttachmentNr6(officialPositionWhoGetNewNotification, numberLastNotification, attachmentPath);
            }
        }
        private void CreateMailWithAttachmentNr6(string officialPositionWhoGetNewNotification, string numberLastNotification, string attachmentPath)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("OUTLOOK");
            int collCount = processes.Length;

            if (collCount != 0)
            {
                    Microsoft.Office.Interop.Outlook.Application oApp = Marshal.GetActiveObject("Outlook.Application") as Microsoft.Office.Interop.Outlook.Application;
                    Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                    mailItem.Subject = "PIW Olesno - zgłoszenia padniecia numer " + "1608/" + numberLastNotification + "/2019" + ".";
                    if (mainWindow.comboBox_UtilizationCompany.Text == "Jasta")
                    {
                        mailItem.To = "jastaska@jasta.net.pl, marcelina.skawska@jasta.net.pl";
                        mailItem.CC = "radomsko.piw @wetgiw.gov.pl, w.wlodara@wiw.opole.pl, d.tomas@wiw.opole.pl";
                    }
                    else if (mainWindow.comboBox_UtilizationCompany.Text == "Farmutil")
                    {
                        mailItem.To = "wegry@farmutil.pl";
                        mailItem.CC = "d.tomas@wiw.opole.pl, piw.opole@wiw.opole.pl, w.wlodara@wiw.opole.pl";
                    }
                    else
                    {
                        mailItem.To = "";
                    }
                    mailItem.Body = "Zgłoszenie padnięcia nr " + "1608/" + numberLastNotification + "/2019" + ". \nPIW Olesno\n" + officialPositionWhoGetNewNotification + "\n" + mainWindow.comboBox_WhoGetGetNotification.Text;                    
                    Microsoft.Office.Interop.Outlook.Attachments mailAttachments = mailItem.Attachments;
                    Microsoft.Office.Interop.Outlook.Attachment newAttachment = mailAttachments.Add(
                    attachmentPath + numberLastNotification + "-" + mainWindow.txtFarmNumber.Text + "-zal6-" + savingDateTime + ".pdf",
                    Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, 1, "Załącznik nr 6");
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

            CommandSQL.Parameters.Add("@dateAndTimeNewNotificationOfAnimalDead", SqlDbType.VarChar).Value = mainWindow.txtDateAndTimeNewNotificationOfAnimalDead.Text;
            CommandSQL.Parameters.Add("@farmNumber", SqlDbType.VarChar).Value = mainWindow.txtFarmNumber.Text;
            CommandSQL.CommandText = "SELECT ID FROM ZGLOSZENIA$ WHERE NR_STADA=@farmNumber and DATA_CZAS_ZGL=@dateAndTimeNewNotificationOfAnimalDead";
            SqlDataReader reader = CommandSQL.ExecuteReader(); // wykonanie zapytania do bazy
            reader.Read();
            string id = reader["ID"].ToString();
            int ID = Convert.ToUInt16(id);
            reader.Close();
            cs.GetDBConnection().Close();
            return ID;
        }                
        private void CreateAttachmentNr7(string officialPositionWhoGetNewNotification, string numberLastNotification, int howManyMonthsAnimalLive, string savePath) //tworzymy załącznik numer 7
        {            
            System.IO.FileStream fs = new FileStream(savePath+numberLastNotification +"-"+ mainWindow.txtFarmNumber.Text+"-zal7-"+ savingDateTime + ".pdf", FileMode.Create);
            // tworzymy instancje klasy dokumentu pdf z wymiarem A4  
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);        
            // klasa writer używająca dokument i strumienia w konstruktorze
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
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
            table.AddCell(new Phrase("Numer dokumentu", polskie_znaki));
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
            table.AddCell(new Phrase(officialPositionWhoGetNewNotification, polskie_znaki));

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
            table.AddCell(new Phrase(mainWindow.txtDateBorn.Text + " wiek: " + howManyMonthsAnimalLive + "miesięcy", polskie_znaki));
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
        }
        //todo:potwierdzenie dotarcia i potwierdzenia pobrania dodać na spodzie załącznika     
        private void CreateAttachmentNr6(string officialPositionWhoGetNewNotification, int howManyMonthsAnimalLive, 
            string numberLastNotification, string savepath) // tworzymy załącznik nr 6 w pdfie
        {
            checkBseOrTseTest(out string testType);
            System.IO.FileStream fs = new FileStream(savepath + numberLastNotification + "-" + mainWindow.txtFarmNumber.Text +"-zal6-"+savingDateTime+".pdf", FileMode.Create);
            // tworzymy instancje klasy dokumentu pdf z wymiarem A4  
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // klasa writer używająca dokument i strumienia w konstruktorze
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            // meta informacje o dokumencie
            document.AddAuthor(officialPositionWhoGetNewNotification);
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

            table.AddCell(new Phrase("Numer dokumentu", polskie_znaki));
            table.AddCell("1608/"+numberLastNotification+"/2019");

            table.AddCell(new Phrase("Numer kolczyka", polskie_znaki));
            table.AddCell(mainWindow.txtEarTagNumber.Text);

            table.AddCell(new Phrase("data urodzenia i wiek", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtDateBorn.Text+" wiek: "+howManyMonthsAnimalLive+"miesięcy" , polskie_znaki));

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
                "w kierunku BSE).\n ", polskie_znaki));
            cell5.Colspan = 2;
            cell5.Border = 0;
            cell5.HorizontalAlignment = 0;
            table.AddCell(cell5);

            table.AddCell(new Phrase("Potwierdzenie dotarcia zwłok do ZU: ", polskie_znaki));
            table.AddCell(new Phrase("POTWIERDZAM\n\nNIE POTWIERDZAM\n\nData i godzina:", polskie_znaki));

            table.AddCell(new Phrase("Potwierdzenie pobrania próbki na BSE: ", polskie_znaki));
            table.AddCell(new Phrase("POBRANO           NIE POBRANO**", polskie_znaki));

            PdfPCell cell6 = new PdfPCell(new Phrase("\n**podać przyczyną nie pobrania próbki .........................................................\n" +
                "\n........................................................................................................................", polskie_znaki));
            cell6.Colspan = 2;
            cell6.Border = 0;
            cell6.HorizontalAlignment = 0;
            table.AddCell(cell6);

            PdfPCell cell7 = new PdfPCell(new Phrase("Osoba wysyłająca awizo: \n " + mainWindow.comboBox_WhoGetGetNotification.Text, polskie_znaki));
            cell7.Colspan = 2;
            cell7.Border = 0;
            cell7.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell7);

            PdfPCell cell8 = new PdfPCell(new Phrase("\n\n\n\n\n-właściwe podkreślić", polskie_znaki));
            cell8.Colspan = 2;
            cell8.Border = 0;
            cell8.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell8);

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
        private void CalculateHowOldIsDeadAnimal(out int howManyMonthsAnimalLive) // liczymy ile miesięcy miało padłe zwierzę
        {            
            DateTime dateDead = DateTime.Parse(mainWindow.DateDead.Text);
            DateTime dateBorn = DateTime.Parse(mainWindow.DateBorn.Text);

            string tempYearBorn = dateBorn.ToString("yyyy");
            string tempMonthBorn = dateBorn.ToString("MM");
            string tempDayBorn = dateBorn.ToString("dd");
            int yearBorn = System.Convert.ToInt16(tempYearBorn);
            int monthBorn = System.Convert.ToInt16(tempMonthBorn);
            int dayBorn = System.Convert.ToInt16(tempDayBorn);

            string tempYearDead = dateDead.ToString("yyyy");
            string tempMonthDead = dateDead.ToString("MM");
            string tempDayDead = dateDead.ToString("dd");
            int yearDead = System.Convert.ToInt16(tempYearDead);
            int monthDead = System.Convert.ToInt16(tempMonthDead);
            int dayDead = System.Convert.ToInt16(tempDayDead);

            LocalDate whenAnimalBorn = new LocalDate(yearBorn, monthBorn, dayBorn);
            LocalDate whenAnimalDead = new LocalDate(yearDead, monthDead, dayDead);

            Period tempMonths = Period.Between(whenAnimalBorn, whenAnimalDead, PeriodUnits.Months);
            howManyMonthsAnimalLive = tempMonths.Months;
            string tempHowManyMonthsAnimalLive = howManyMonthsAnimalLive.ToString();
        }

        private void OfficialPosition(out string officialPositionWhoGetNewNotification) // osoba przyjmująca zgłoszenie
        {
            officialPositionWhoGetNewNotification = null;
            string whoGetGetNotification = mainWindow.comboBox_WhoGetGetNotification.Text;
            switch (whoGetGetNotification)
            {
                case "Gabriela Gallus":
                    officialPositionWhoGetNewNotification = "Starszy referent ds. administracyjnych";            
                    break;            
            
                case "Joanna Frankiewicz":
                    officialPositionWhoGetNewNotification = "Inspektor ds. dobrostanu zwierząt";
                    break;

                case "Piotr Moj":
                    officialPositionWhoGetNewNotification = "Informatyk";
                    break;

                case "Izabela Glomb":
                    officialPositionWhoGetNewNotification = "Inspektor ds. pasz i utylizacji";
                    break;

                case "Krzysztof Chyra":
                    officialPositionWhoGetNewNotification = "Zastępca PLW";
                    break;

                case "Łukasz Kościelny":
                    officialPositionWhoGetNewNotification = "Inspektor ds. chorób zakaźnych";
                    break;

                case "Sebastian Konwant":
                    officialPositionWhoGetNewNotification = "Powiatowy Lekarz Weterynarii";
                    break;

                case "Katarzyna Lech":
                    officialPositionWhoGetNewNotification = "Inspektor ds. higieny zwierząt";
                    break;

                case "Urszula Tylak":
                    officialPositionWhoGetNewNotification = "Kontroler weterynaryjny";
                    break;

                case "Małgorzata Wychrystenko":
                    officialPositionWhoGetNewNotification = "Zastępca głównej księgowej";
                    break;

                case "Anna Kała":
                    officialPositionWhoGetNewNotification = "Główna księgowa";
                    break;
            }
        }
    }
}
