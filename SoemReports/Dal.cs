using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class Dal
    {
        Monitor monitoring = new Monitor();

        OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["SOEM_Prod"].ConnectionString);
    
        public void Insert_Into_DB(DataTable dt, string tblename)
        {
             connection.Open();
            // Insert data to DB table
             string final = null; 
            try
            {
                    // Connect to DB server
                    OracleCommand command = new OracleCommand(final, connection);
                    // Build query
                    string strquery = tblename;
                    string Values = "";
                    command.Parameters.Clear();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        strquery += dt.Columns[i].ColumnName + ",";
                        Values += ":" + dt.Columns[i].ColumnName + ",";
                        command.Parameters.Add(new OracleParameter(":" + dt.Columns[i].ColumnName, null));
                    }

                    Values = Values.Remove(Values.Length - 1);         
                    command.CommandText = strquery.Remove(strquery.Length - 1) + ")" + "values(" + Values + ")"; ;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                            command.Parameters[":" + dt.Columns[j].ColumnName].Value = dt.Rows[i][j];

                        command.ExecuteNonQuery();
                    }
              
                command.Dispose();
            }
            catch (Exception ex)
            {
                //On ORA-00942: table or view does not exist
                if (ex.Message.Contains("ORA-00942") == true)
                {
                    //write in log file --> new table was created
                    monitoring.WriteToFile(" *** A new DB Table was careated:  ***" + tblename.Replace("INSERT INTO ", null).Replace("(", null));
                    connection.Close();
                    //create new table in DB
                    Create_Table_DB(dt, tblename);

                    //insert data to new created table
                    Insert_Into_DB(dt, tblename);
                }
                else
                {
                    monitoring.WriteToLogFileOnDBException(ex, tblename);
                }   
            }
            finally
            {
                connection.Close();
            }
          
       
        }

        // on ORA-00942 exception create new table in DB
        public void Create_Table_DB(DataTable dt, string tblename)
        {   
            connection.Open();

            string colum = null;
            for (int i = 0; i < dt.Columns.Count; i++)
                colum += dt.Columns[i].ColumnName + "  varchar2(50),  ";

            string query = "CREATE TABLE  " + tblename.Replace("INSERT INTO ", null).Replace("(", null) + " ( " + colum.Replace("-", "_").Replace(".", null).Remove(colum.Length - 3) + ")";
            try
            {
                OracleCommand command = new OracleCommand(query, connection);
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                monitoring.WriteToLogFileOnDBException(ex, tblename);
            }
            finally
            {
                connection.Close();
            }
          

        }
    }
}
