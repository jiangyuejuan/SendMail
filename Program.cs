using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceComma
{
    class Program
    {
        static void Main(string[] args)
        {
            var TargetGroupIDList = ReadCSV1("C:\\GroupChange\\GetOpenID.csv");
            SaveDataToCSVFile(TargetGroupIDList, "C:\\GroupChange\\GetOpenIDReplace.csv");
           // ImportMySQL();
           // OpenCSV("C:\\GroupChange\\0.csv");
        }
        /// <summary>
        /// 更新，添加，删除的储存过程调用
        /// </summary>
        /// <param name="storedProcName">储存过程名</param>
        /// <param name="parameteres">更新，添加，删除所需参数</param>
        /// <param name="strConn">connection</param>
        /// <returns></returns>
        public static int RunProcedureForNonQuery(string storedProcName, IDataParameter[] parameteres, string strConn)
        {
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                int rowsAffected = 0;
                try
                {
                    conn.Open();
                    MySqlCommand cm = new MySqlCommand();
                    cm.Connection = conn;
                    cm.CommandText = storedProcName;
                    cm.CommandType = CommandType.Text;
                    cm.CommandTimeout = 3000;
                    if (parameteres != null) cm.Parameters.AddRange(parameteres);
                    rowsAffected = cm.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                   // logger.Debug("有异常抛出，具体原因如下：" + ex.Message);
                }
                return rowsAffected;
            }
        }

        private static void ImportMySQL()
        {
            string conn = "Server=10.10.6.237;Port=3306;Database=CRM_WeChat_9;Uid=root;Pwd=1;convert zero datetime=True;Allow User Variables=True;Connection Timeout=1000 ";
           // MySqlHelper dbhelp = new MySqlHelper(conn);

            DataTable dt = null;
            MySql.Data.MySqlClient.MySqlParameter p = new MySql.Data.MySqlClient.MySqlParameter();
            //这个dt反正就是自己从数据库查出来，或者是自行组装的，就不多说  
           // dt = dbhelp.ExecuteDataTable("select  org_code,org_name from DIM_MANAGE_ORG_V", p); ;
            dt = OpenCSV("C:\\GroupChange\\0.csv");
            //Adding dummy entries  
            string sql = "Insert Into GAP0506 VALUES('";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sql = "Insert Into GAP0506 VALUES('";
                sql = sql + dt.Rows[i][0].ToString() + "')";
                RunProcedureForNonQuery(sql, null, conn);
                sql = "";
            }

            string connectMySQL = "Server=127.0.0.1;Database=apd;Uid=root;Pwd=123456;";
            string strFile = "~/TempFolder/MySQL" + DateTime.Now.Ticks.ToString() + ".csv";

            ////Create directory if not exist... Make sure directory has required rights..  
            //if (!Directory.Exists(Server.MapPath("~/TempFolder/")))
            //    Directory.CreateDirectory(Server.MapPath("~/TempFolder/"));

            ////If file does not exist then create it and right data into it..  
            //if (!File.Exists(Server.MapPath(strFile)))
            //{
            //    FileStream fs = new FileStream(Server.MapPath(strFile), FileMode.Create, FileAccess.Write);
            //    fs.Close();
            //    fs.Dispose();
            //}

            ////Generate csv file from where data read  
            //CreateCSVfile(dt, Server.MapPath(strFile));
            //using (MySqlConnection cn1 = new MySqlConnection(connectMySQL))
            //{
            //    cn1.Open();
            //    MySqlBulkLoader bcp1 = new MySqlBulkLoader(cn1);
            //    bcp1.TableName = "productorder"; //Create ProductOrder table into MYSQL database...  
            //    bcp1.FieldTerminator = ",";

            //    bcp1.LineTerminator = "\r\n";
            //    bcp1.FileName = Server.MapPath(strFile);
            //    bcp1.NumberOfLinesToSkip = 0;
            //    bcp1.Load();

            //    //Once data write into db then delete file..  
            //    try
            //    {
            //        File.Delete(Server.MapPath(strFile));
            //    }
            //    catch (Exception ex)
            //    {
            //        string str = ex.Message;
            //    }
            //}
        }  

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
            return dt;
        }

        /// <summary>
        /// Save the List data to CSV file
        /// </summary>
        /// <param name="studentList">data source</param>
        /// <param name="filePath">file path</param>
        /// <returns>success flag</returns>
        private static bool SaveDataToCSVFile(List<string> studentList, string filePath)
        {
            bool successFlag = true;

            //StringBuilder strColumn = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            StreamWriter sw = null;
            //PropertyInfo[] props = GetPropertyInfoArray();

            try
            {
                sw = new StreamWriter(filePath);
                //for (int i = 0; i < props.Length; i++)
                //{
                //    strColumn.Append(props[i].Name);
                //    strColumn.Append(",");
                //}
                //strColumn.Remove(strColumn.Length - 1, 1);
                //sw.WriteLine(strColumn);    //write the column name

                for (int i = 0; i < studentList.Count; i++)
                {
                   // strValue.Remove(0, strValue.Length); //clear the temp row value
                    //strValue.Append(studentList[i]);
                    //strValue.Append(",");
                    //strValue.Append(studentList[i].Name);
                    //strValue.Append(",");
                    //strValue.Append(studentList[i].Age);
                    sw.WriteLine(studentList[i]); //write the row value
                }
            }
            catch (Exception ex)
            {
                successFlag = false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                }
            }

            return successFlag;
        }


        public static List<string> ReadCSV1(string filePathName)
        {
            // List<string> list = new List<string>();
            string[] returnArray;
            List<string> ls = new List<string>();
            //ls.OpenID = new string[];
            StreamReader fileReader = new StreamReader(filePathName);
            string strLine = "";
            while (strLine != null)
            {
                strLine = fileReader.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    strLine = strLine.Replace("\"", "");
                    returnArray = strLine.Split(',');
                    for (int i = 0; i < returnArray.Length; i++)
                    {
                        ls.Add(returnArray[i]);
                    }


                    //  return returnArray.ToList();

                }
            }
            fileReader.Close();
            return ls;
        }
    }
}
