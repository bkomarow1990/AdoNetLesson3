using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdoNetLesson3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        string connStringNotEncrypted = "";
        //шифрованная строка подключения
        string connStringWithEncryption = "";
        ShopDb shopDb = new ShopDb();
        private DbProviderFactory fact = null;
        // connection class to database
        private DbConnection conn = null;
        // data adapter for disconnected mode
        private DbDataAdapter da = null;
        // DataSet
        private DataSet set = null;
        // DataSet
        private void LoadShop()
        {

        }
        private void LoadDataGrid()
        {
            // query for select data
            string sql = "SELECT * FROM Users;";
            // create data adapter
            da = fact.CreateDataAdapter();
            da.SelectCommand = conn.CreateCommand();
            da.SelectCommand.CommandText = sql;
            // create command builder for auto generate insert, update and delete queries
            var builder = fact.CreateCommandBuilder();
            builder.DataAdapter = da;

            // create empty DataSet
            set = new DataSet();
            // execute select query on server and put data to DataSet
            da.Fill(set);

            // bind table to DataGrid
            itemsDataGrid.ItemsSource = set.Tables[0].DefaultView;

        }
        public MainWindow()
        {
            InitializeComponent();
            connStringNotEncrypted = ConfigurationManager.ConnectionStrings["AccountsDB"].ConnectionString;
            //отображаем строку подключения до шифрования
            //выполняем шифрование
            EncryptConnSettings("connectionStrings");
            //читаем строку подключения после шифрования
            //connStringWithEncryption = ConfigurationManager.ConnectionStrings["AccountsDB"].ConnectionString;
            ////отображаем строку подключения после шифрования
            string cs = ConfigurationManager.ConnectionStrings["AccountsDB"].ConnectionString;
            //// create factory from provider name
            fact = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["AccountsDB"].ProviderName);

            //// create sql connection class
            conn = fact.CreateConnection();
            conn.ConnectionString = cs;
            LoadDataGrid();
        }
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void EncryptConnSettings(string section)
        {
            Configuration objConfig = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location); //GetAppPath() + "config_encryptions.exe");
            //получаем доступ к разделу ConnectionStrings
            //нашего конфигурационного файла
            ConnectionStringsSection conSringSection = (ConnectionStringsSection)objConfig.GetSection(section);
            //если раздел не зашифрован — шифруем его
            if (!conSringSection.SectionInformation.IsProtected)
            {
                conSringSection.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                conSringSection.SectionInformation.ForceSave = true;
                objConfig.Save(ConfigurationSaveMode.Modified);
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (DataRow item in set.Tables[0].GetChanges(DataRowState.Added).Rows)
                {
                    MessageBox.Show(item["Password"].ToString());
                }
                for (int i = 0; i < set.Tables[0].GetChanges(DataRowState.Added).Rows.Count; i++)
                {
                    set.Tables[0].GetChanges(DataRowState.Added).Rows[i]["Password"] = ComputeSha256Hash(set.Tables[0].GetChanges(DataRowState.Added).Rows[i]["Password"].ToString());
                }
                da.Update(set);
                MessageBox.Show("Added");
                //MessageBox.Show(item["Password"].ToString());
                // update server data (sync DataSet with database)
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void FillButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
