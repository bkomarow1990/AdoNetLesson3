using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        string current_table = "Users";
        AccountsdDB acc_db = new AccountsdDB();
        ShopDb shopDb = new ShopDb();
        private void LoadShop()
        {

        }
        private void LoadDataGrid()
        {
            if (current_table == "Users")
            {
                try
                {
                    // query for select data
                    string sql = $"select * from {current_table}";
                    // create data adapter
                    shopDb.da = new SqlDataAdapter(sql, AccountsdDB.connection);
                    // create command builder for auto generate insert, update and delete queries
                    new SqlCommandBuilder(shopDb.da);

                    // create empty DataSet
                    shopDb.set = new DataSet();
                    // execute select query on server and put data to DataSet
                    shopDb.da.Fill(shopDb.set, current_table);

                    // bind table to DataGrid
                    itemsDataGrid.ItemsSource = shopDb.set.Tables[current_table].DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                // query for select data
                string sql = $"select * from {current_table}";
                // create data adapter
                acc_db.da = new SqlDataAdapter(sql, AccountsdDB.connection);
                // create command builder for auto generate insert, update and delete queries
                new SqlCommandBuilder(acc_db.da);

                // create empty DataSet
                acc_db.set = new DataSet();
                // execute select query on server and put data to DataSet
                acc_db.da.Fill(acc_db.set, current_table);

                // bind table to DataGrid
                itemsDataGrid.ItemsSource = acc_db.set.Tables[current_table].DefaultView;
            }
              
            
        }
        public MainWindow()
        {
            InitializeComponent();
            LoadDataGrid();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // update server data (sync DataSet with database)
                acc_db.da.Update(acc_db.set, current_table);
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

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
           

            if (cb.SelectedIndex==0)
            {
                current_table = "Sales";
            }
            else
            {
                current_table = "Users";
            }
        }
    }
}
