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
             
        private void btnGenerateAttachment_Click(object sender, RoutedEventArgs e)
        {           
            string OfficialPositionWhoGetNewNotification;
            OfficialPosition(out OfficialPositionWhoGetNewNotification); 
            
            System.IO.FileStream fs = new FileStream("PDF/" + mainWindow.txtFarmNumber.Text + ".pdf", FileMode.Create);
            // Create an instance of the document class which represents the PDF document itself.  
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF   
            // Writer class using the document and the filestrem in the constructor.  

            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            
            // Add meta information to the document  
            document.AddAuthor("Micke Blomquist");
            document.AddCreator("Sample application using iTextSharp");
            document.AddKeywords("PDF tutorial education");
            document.AddSubject("Document subject - Describing the steps creating a PDF document");
            document.AddTitle("The document title - PDF creation using iTextSharp");

            // Open the document to enable you to write to the document  
            document.Open();
            // Add a simple and wellknown phrase to the document in a flow layout manner  
            FontFactory.RegisterDirectory("C:WINDOWSFonts"); //dodajemy polskie znaki
            var polskie_znaki = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.EMBEDDED);
            
            //document.Add(new Paragraph("Rejestr zgłoszeń padłego bydła - załącznik 7", polskie_znaki));
            PdfPTable table = new PdfPTable(2);
            PdfPCell cell = new PdfPCell(new Phrase("Rejestr zgłoszeń padłego bydła - załącznik 7\n ", polskie_znaki));

            cell.Colspan = 2;
            cell.Border = 0;

            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

            table.AddCell(cell);

            table.AddCell(new Phrase("Numer zgłoszenia", polskie_znaki));
            table.AddCell("1608//2019");
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
            table.AddCell(new Phrase(mainWindow.txtAddressPersonReporting.Text , polskie_znaki));
            table.AddCell(new Phrase("Telefon", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtPhonePersonReporting.Text , polskie_znaki));

            PdfPCell cell3 = new PdfPCell(new Phrase("\nOsoba przyjmująca zgłoszenie", polskie_znaki));
            cell3.Colspan = 2;
            cell3.Border = 0;
            cell3.HorizontalAlignment = 0;
            table.AddCell(cell3);
            table.AddCell(new Phrase("Imię i nazwisko", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_WhoGetGetNotification.Text, polskie_znaki));
            table.AddCell(new Phrase("Stanowisko służbowe", polskie_znaki));
            table.AddCell(new Phrase(OfficialPositionWhoGetNewNotification , polskie_znaki));

            PdfPCell cell4 = new PdfPCell(new Phrase("\nMiejsce padnięcia zwierzęcia – adres gospodarstwa", polskie_znaki));
            cell4.Colspan = 2;
            cell4.Border = 0;
            cell4.HorizontalAlignment = 0;
            table.AddCell(cell4);
            table.AddCell(new Phrase("Imię i nazwisko posiadacza zwierzęcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtName.Text+' '+mainWindow.txtSurname.Text , polskie_znaki));
            table.AddCell(new Phrase("Adres gospodarstwa/nr siedziby stada", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtStreet.Text+' '+mainWindow.txtHouseNumber.Text
                +' '+mainWindow.txtLocalNumber.Text+' '+mainWindow.txtPostCode.Text+' '+mainWindow.txtPost.Text
                +'/'+mainWindow.txtFarmNumber.Text, polskie_znaki));
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
            table.AddCell(new Phrase(mainWindow.comboBox_TypeOfFarm.Text , polskie_znaki));
            table.AddCell(new Phrase("Liczba sztuk", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtHowManyAnimalsInFarm.Text, polskie_znaki));

            PdfPCell cell6 = new PdfPCell(new Phrase("\nIdentyfikacja padłego zwierzęcia", polskie_znaki));
            cell6.Colspan = 2;
            cell6.Border = 0;
            cell6.HorizontalAlignment = 0;
            table.AddCell(cell6);
            table.AddCell(new Phrase("nr kolczyka zwierzęcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtFarmNumber.Text , polskie_znaki));
            table.AddCell(new Phrase("data urodzenia i wiek", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtDateBorn.Text , polskie_znaki));
            table.AddCell(new Phrase("płeć", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_GenderOfDeadAnimal.Text, polskie_znaki));
            table.AddCell(new Phrase("Data i godzina padnięcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtDateDead.Text+' '+mainWindow.txtHourOfDeadAnimal.Text, polskie_znaki));
            table.AddCell(new Phrase("Przyczyna padnięcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.comboBox_DeadDeterminedOrNot.Text, polskie_znaki));
            table.AddCell(new Phrase("Podać prawdopodobną przyczynę padnięcia", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtWhyDead.Text, polskie_znaki));
            table.AddCell(new Phrase("Dodatkowe uwagi", polskie_znaki));
            table.AddCell(new Phrase(mainWindow.txtComment.Text, polskie_znaki));

            PdfPCell cell7 = new PdfPCell(new Phrase("Osoba przyjmująca zgłoszenie: \n "+ mainWindow.comboBox_WhoGetGetNotification.Text, polskie_znaki));
            cell7.Colspan = 2;
            cell7.Border = 0;
            cell7.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell7);

            document.Add(table);
            // Close the document  
            document.Close();
            // Close the writer instance  
            writer.Close();
            // Always close open filehandles explicity  
            fs.Close();
            //Process.Start("C:test.pdf");
            int HowManyMonthsAnimalLive;
            CalculateHowOldIsDeadAnimal(out HowManyMonthsAnimalLive);
            MessageBox.Show(mainWindow.txtTypeOfDeadAnimal.Text);

            if (HowManyMonthsAnimalLive >= 48 && mainWindow.txtTypeOfDeadAnimal.Text == "bydlo")
            {
                MessageBox.Show("Generujemy również załącznik nr 6");
            }
            else
            {
                MessageBox.Show("Za młoda sztuka");
            }
            //{
            //    BSE = true;
            //}
            //else if(Age > 18 && mainWindow.txtTypeOfDeadAnimal.Text == "owca")
            //{
            //    BSE = true;
            //}
        }

        private void CalculateHowOldIsDeadAnimal(out int HowManyMonthsAnimalLive)
        {
            //ToDo: obliczanie wieku padłych zwierząt
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


            MessageBox.Show(tempHowManyMonthsAnimalLive);
        }

        private void OfficialPosition(out string OfficialPositionWhoGetNewNotification)
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

        private void btnSendMail_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("OUTLOOK");
            int collCount = processes.Length;

            if (collCount != 0)
            {
                Microsoft.Office.Interop.Outlook.Application oApp = Marshal.GetActiveObject("Outlook.Application") as Microsoft.Office.Interop.Outlook.Application;
                Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                mailItem.Subject = "This is the test message";
                mailItem.To = "contactus@authorcode.com";
                mailItem.CC = "supporttools@authorcode.com";
                mailItem.Body = "This is the test message";
                mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceNormal;
                mailItem.Display(false);
                mailItem = null;
                oApp = null;
            }
            else
            {
                Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                mailItem.Subject = "This is the test message";
                mailItem.To = "contactus@authorcode.com";
                mailItem.CC = "supporttools@authorcode.com";
                mailItem.Body = "This is the test message";
                mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceNormal;
                mailItem.Display(false);
                mailItem = null;
                oApp = null;
            }

            funckcja();
            void funckcja()
            {
                MessageBox.Show("sdsd");
            }

        }
    }
}
