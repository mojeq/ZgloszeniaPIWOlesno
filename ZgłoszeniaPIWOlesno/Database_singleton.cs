using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZgłoszeniaPIWOlesno
{
    public sealed class Singleton
    {
        private static Singleton mInstance = null;

        private readonly SqlConnection connect = new SqlConnection(@"Data source=LAPTOP-6E2UURS8\INSERTGT;
                                                             database=BAZA_ARIMR;
                                                             User id=sa;
                                                             Password=alpinus1;");



        public static Singleton Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new Singleton();
                }
                return mInstance;
            }
        }

        // public void Connect()
        // {
        //   try
        //    {
        //       SqlConnection polaczenie = new SqlConnection(@"Data source=LAPTOP-6E2UURS8\INSERTGT;
        //                                                    database=BAZA_ARIMR;
        //                                                    User id=sa;
        //                                                    Password=alpinus1;");

        //      polaczenie.Open();

        //SqlCommand komendaSQL = polaczenie.CreateCommand();

        //powiązanie zmiennej numer_stada z numerem stada w zapytaniu SQL
        //komendaSQL.Parameters.Add("@stado", SqlDbType.VarChar).Value = numer_stada;
        //

        //komendaSQL.CommandText = "SELECT * FROM BAZA_GOSPODARSTWA$ WHERE NR_STADA=@stado";
        //SqlDataReader czytnik = komendaSQL.ExecuteReader();

        //wyświetlenie pobranych danych z bazy
        //while (czytnik.Read())
        // {
        //     MessageBox.Show(czytnik["NAZWISKO_LUB_NAZWA"].ToString());
        // }

        // czytnik.Close();
        // polaczenie.Close();


        //}

        //catch (SqlException f)
        // {
        //wyświetlenie błędów połączenia z bazą
        //     MessageBox.Show("Wystąpił nieoczekiwany błąd" + f);
        //  }

        public SqlConnection GetDBConnection()
        {
            
            return connect;

        }



    }
}

