using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
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
            //CryptoKeyAccessRule rule = new CryptoKeyAccessRule("everyone", CryptoKeyRights.FullControl, AccessControlType.Allow);
            EncryptConnSettings("connectionStrings");
            fact = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["AccountsDB"].ProviderName);
            string cs = ConfigurationManager.ConnectionStrings["AccountsDB"].ConnectionString;
            conn = fact.CreateConnection();
            //conn.ConnectionString = cs;
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
            //создаем объект нашего конфигурационного файла
            //AppConfigDecrypt.exe — это имя выполняемого
            //файла вашего приложения если у вас оно
            //другое, то учтите этот момент

            Configuration objConfig = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location); //GetAppPath() + "config_encryptions.exe");
            MessageBox.Show($"{System.Reflection.Assembly.GetEntryAssembly().Location}, {System.Reflection.Assembly.GetEntryAssembly()}");
            //получаем доступ к разделу ConnectionStrings
            //нашего конфигурационного файла
            ConnectionStringsSection conSringSection = (ConnectionStringsSection)objConfig.GetSection(section);
            //если раздел не зашифрован — шифруем его
            if (!conSringSection.SectionInformation.IsProtected)
            {
                conSringSection.SectionInformation.ProtectSection("MyProtectionProvider");
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
