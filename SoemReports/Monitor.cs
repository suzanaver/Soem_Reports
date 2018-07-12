using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Configuration;
using System.IO;
using System.Data;

namespace SoemReports
{
    public class Monitor
    {
        //Catch DB Exception
        public void WriteToLogFileOnDBException (Exception ex, string Tablelename)
        {
            WriteToFile("  **** ex.Message " + ex.Message + "  ****   Table Name: " + Tablelename.Replace("INSERT INTO", null).Replace("(", null));
        }

        // Fined the missing file
        public void MissingFile(List<string> directories, List<string> FilesToDownload)
        {
                string concutallfilesdownloaded = null;
                string missingfile = null; 

                foreach (var downloaded in directories)
                    concutallfilesdownloaded += downloaded.Remove(downloaded.Length - 11); 

                foreach (var item in FilesToDownload)
                {
                    if (concutallfilesdownloaded.Contains(item) == false)
                    {
                        missingfile += "  **** Missing Soem Report, the file was missing in ftp server --> " + item + "  ****" ;
                        WriteToFile(missingfile);               
                    }
                }
        }
        //Write To Log File
        public void WriteToFile (string messegetowrite)
        {
            General.ErrorMessge.Add(messegetowrite);

            string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dir += @"\SoemReports\SoemReports\bin\Debug\Error\Error.txt";

           // string dir = Directory.GetCurrentDirectory() + @"\Error\Error.txt";
            StreamWriter swLog = new StreamWriter(dir, true);
            swLog.Write("Date ****  " + DateTime.Now);
            swLog.WriteLine(messegetowrite);
            swLog.Close();
        }
    }
}
