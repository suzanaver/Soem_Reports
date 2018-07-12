using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Net;
using System.Net.FtpClient;
using Limilabs.FTP.Client;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Configuration;

namespace SoemReports
{
    class Program
    {
        static void Main(string[] args)
        {
            Dal dal = new Dal();
            Monitor monitoring = new Monitor();

            string ReportNamepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ReportNamepath += @"\SoemReports\SoemReports\bin\Debug\ReportName\ReportName.txt";
            //Get Relevant Report Name
         //   string[] ReportName = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\ReportName\ReportName.txt");
            string[] ReportName = File.ReadAllLines(ReportNamepath);
            List<string> directories = new List<string>();

            //// server data
            //string UserName = "ftpuser";
            //string Password = "ericsson";
            //string ftp = "file://10.8.117.25/PHIFiles/Rep_SOEM";

            // create directory do download files
            string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\PMdata\";
            Directory.CreateDirectory(targetPath);

            // buld report name with the date
            List<string> CurrentReportName = new List<string>();
            foreach (var item in ReportName)
                CurrentReportName.Add(item + "_" + DateTime.Today.AddDays(-1).ToString("yyyyMMdd"));

            //Get File Correct Link


            //try
            //{
            //    var request = (FtpWebRequest)WebRequest.Create(ftp);
            //    request.Method = WebRequestMethods.Ftp.ListDirectory;
            //    request.Credentials = new NetworkCredential(UserName, Password);
            //    request.Proxy = null;
            //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            //    StreamReader reader = new StreamReader(response.GetResponseStream());
            //    var line = reader.ReadLine();
            //    while (!string.IsNullOrEmpty(line))
            //    {
            //        line = reader.ReadLine();
            //        if (line!="" && line!= null)
            //        {
            //            foreach (var item in CurrentReportName)
            //                if (line.ToString().Replace("PMdata/", null).Contains(item) == true)
            //                    directories.Add(line.ToString().Replace("PMdata/", null));
            //        }    
            //    }
            //    reader.Close();
            //}
            //catch (Exception ex)
            //{
            //    monitoring.WriteToFile(ex.Message.ToString());
            //    System.Threading.Thread.Sleep(5000);
            //    Program pr = new Program();
            //}

            //Check if some file are missing
          //  if (directories.Count < CurrentReportName.Count)
             //   monitoring.MissingFile(directories, CurrentReportName);
       
            //Download files to PMdata folder
            //try
            //{
            //    using (WebClient ftpClient = new WebClient())
            //    {
            //        ftpClient.Credentials = new System.Net.NetworkCredential(UserName, Password);
            //        for (int i = 0; i <= directories.Count - 1; i++)
            //            ftpClient.DownloadFile("file://10.8.117.25/PHIFiles/Rep_SOEM/" + directories[i].ToString(), targetPath + directories[i].ToString().Replace("txt", "csv"));
               
            //        ftpClient.Dispose();
            //    }
            //}
            //catch (Exception ex )
            //{
            //    monitoring.WriteToFile(ex.Message.ToString());
            //}


            if (!Directory.Exists(@"file:\\10.8.117.25\PHIFiles\Rep_SOEM\"))
            {
                string[] fileEntries = Directory.GetFiles(@"\\10.8.117.25\PHIFiles\Rep_SOEM\");
                foreach (var file in fileEntries)
                {
                    foreach (var item in CurrentReportName)

                        if (file.Contains(item) == true)
                            directories.Add(file.Replace(@"\\10.8.117.25\PHIFiles\Rep_SOEM\", null));

                }

                foreach (var item in directories)
                {
                    File.Copy(@"\\10.8.117.25\PHIFiles\Rep_SOEM\" + item, targetPath + item.Replace(".txt", ".csv"), true);
                }
            }

            DirectoryInfo di = new DirectoryInfo(targetPath);
            FileInfo[] rgFiles = di.GetFiles("*.csv");

            if (rgFiles.Count()==0)
            {
                monitoring.WriteToFile("No Files to import " );
            }

            foreach (FileInfo fi in rgFiles)
            {
                var dt = new DataTable();

                // load csv file to datatable
                using (StreamReader sr = new StreamReader(targetPath + fi.Name))
                {
                    string[] headers = sr.ReadLine().Split(',');

                    foreach (string header in headers)
                        dt.Columns.Add(header.Replace("-", "_").Replace(".", null));
                   
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (i!=3)
                                dr[i] = rows[i].Replace("-", null);

                            if (i==3)
                                dr[i] = DateTime.Parse(rows[i]).ToString("dd/MM/yyyy hh:mm:ss");
                        }
                        dt.Rows.Add(dr);
                    }

                    if (dt.Rows.Count>0)
                    {
                              // Insert data to DB
                        string tablename = "INSERT INTO " + fi.ToString().Remove(fi.ToString().Length - 20).Replace("_", null).Replace("SRV", "_") + "(";
                    dal.Insert_Into_DB(dt, tablename); 
                    }

                    else
                    {
                        monitoring.WriteToFile(" ****The file is empty  " + fi.ToString().Remove(fi.ToString().Length - 20).Replace("_", null).Replace("SRV", "_"));
                    }
           
                }
            }
                   // Delete folder PMdata
                   Directory.Delete(targetPath, true);
                
                   //Send meil --> error
                   if (General.ErrorMessge.Count > 0)
                   {
                        Notification notify = new Notification();
                   }
        }
    }
}
