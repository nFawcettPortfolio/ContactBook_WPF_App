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

namespace ContactsList
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string[] state_abbr = new string[] { "AL","AK","AR","AZ","CA","CO","CT","DC","DE","FL","GA","HI","IA","ID","IL","IN",
                "KS","KY","LA","MA","MD","ME","MI","MN","MO","MS","MT","NC","ND","NE","NH","NJ","NM","NV","NY","OH","OK","OR","PA","RI",
                "SC","SD","TN","TX","UT","VA","VT","WA","WI","WV","WY"};
            InitializeComponent();
            this.cb_state.ItemsSource = state_abbr;
            try
            {
                SqlConnection conn = new SqlConnection(@"Server=(local);Trusted_Connection=Yes;");
                conn.Open();

                string str = "USE master;\n" +
                    "BEGIN\n" +
                    "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'ContactsList')\n" +
                    "\tCREATE DATABASE ContactsList;\n" +
                    "\tUSE ContactsList;\n" +
                    "IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'People')\n" +
                    "\tCREATE TABLE People (\n" +
                    "\tid INT PRIMARY KEY IDENTITY (1,1),\n" +
                    "\tfirst NVARCHAR(30),\n" +
                    "\tlast NVARCHAR(30),\n" +
                    "\tphone NVARCHAR(15),\n" +
                    "\temail NVARCHAR(50),\n" +
                    "\tstreet NVARCHAR(50),\n" +
                    "\tcity NVARCHAR(30),\n" +
                    "\tstate NVARCHAR(2),\n" +
                    "\tzipcode NVARCHAR(10))\n" +
                    "END;";

                SqlCommand cmd = new SqlCommand(str, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: "+ex);
            }
            UpdateLookUp();
        }

        private void ClearAll()
        {
            tb_first.Text = "";
            tb_last.Text = "";
            tb_phone.Text = "";
            tb_email.Text = "";
            tb_street.Text = "";
            tb_city.Text = "";
            cb_state.Text = "";
            tb_zip.Text = "";
        }
        private void UpdateLookUp()
        {
            try
            {
                cb_lookup.Items.Clear();
                cb_lookup.Items.Add("Add new contact...");
                SqlConnection conn = new SqlConnection(@"Server=(local);Trusted_Connection=Yes;");
                {
                    SqlCommand sqlCmd = new SqlCommand("USE ContactsList;\nSELECT * FROM People", conn);
                    conn.Open();
                    SqlDataReader sqlReader = sqlCmd.ExecuteReader();

                    while (sqlReader.Read())
                    {
                        cb_lookup.Items.Add(sqlReader["last"].ToString() + ", " + sqlReader["first"].ToString());
                    }

                    sqlReader.Close();
                }
            }
            catch { }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_lookup.Text== "Add new contact...")
            {
                ClearAll();
            }
            else
            {
                try
                {
                    string[] fullname = cb_lookup.SelectedItem.ToString().Split(',');
                    SqlConnection conn = new SqlConnection(@"Server=(local);Trusted_Connection=Yes;");
                    {
                        string s = "USE ContactsList;\nSELECT * FROM People WHERE last ='" + String.Concat(fullname[0].Where(c => !Char.IsWhiteSpace(c))) + "' AND first = '" + String.Concat(fullname[1].Where(c => !Char.IsWhiteSpace(c))) + "'";

                        SqlCommand sqlCmd = new SqlCommand(s, conn);
                        conn.Open();
                        SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                        if (sqlReader.Read())
                        {
                            tb_first.Text = sqlReader["first"].ToString();
                            tb_last.Text = sqlReader["last"].ToString();
                            tb_phone.Text = sqlReader["phone"].ToString();
                            tb_email.Text = sqlReader["email"].ToString();
                            tb_street.Text = sqlReader["street"].ToString();
                            tb_city.Text = sqlReader["city"].ToString();
                            cb_state.Text = sqlReader["state"].ToString();
                            tb_zip.Text = sqlReader["zipcode"].ToString();
                        }

                        sqlReader.Close();
                    }
                }
                catch{}

            }
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (this.cb_lookup.Text=="Add new contact...")
            {
                try
                {
                String str = "USE ContactsList; \nINSERT INTO People (first, last, phone, email, street, city, state, zipcode) " +
                    "VALUES ('"+this.tb_first.Text+"', '"+this.tb_last.Text+"', '"+this.tb_phone.Text+"', '"+
                    this.tb_email.Text+"', '"+this.tb_street.Text+"', '"+this.tb_city.Text+"', '"+
                    this.cb_state.Text+"', '"+this.tb_zip.Text+"');";
                SqlConnection conn = new SqlConnection(@"Server=(local);Trusted_Connection=Yes;");
                SqlCommand cmd = new SqlCommand(str, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                UpdateLookUp();
                MessageBox.Show("New contact added.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
            }
            else
            {
                try
                {
                    string[] fullname = cb_lookup.SelectedItem.ToString().Split(',');
                    String str = "USE ContactsList; \nUPDATE People SET " +
                        "first =  '" + this.tb_first.Text + "', " +
                        "last = '" + this.tb_last.Text + "', " +
                        "phone = '" + this.tb_phone.Text + "', " +
                        "email = '" + this.tb_email.Text + "', " +
                        "street = '" + this.tb_street.Text + "', " +
                        "city = '" + this.tb_city.Text + "', " +
                        "state = '" + this.cb_state.Text + "', " +
                        "zipcode = '" + this.tb_zip.Text + "' " +
                        "WHERE last ='" + String.Concat(fullname[0].Where(c => !Char.IsWhiteSpace(c))) + "' " +
                        "AND first = '" + String.Concat(fullname[1].Where(c => !Char.IsWhiteSpace(c))) + "'";
                    SqlConnection conn = new SqlConnection(@"Server=(local);Trusted_Connection=Yes;");
                    SqlCommand cmd = new SqlCommand(str, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    UpdateLookUp();
                    MessageBox.Show("Contact updated.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
            }
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            if (cb_lookup.Text != null && cb_lookup.Text != "Add new contact...")
            {
                SqlConnection conn = new SqlConnection(@"Server=(local);Trusted_Connection=Yes;");
                conn.Open();
                string[] fullname = cb_lookup.SelectedItem.ToString().Split(',');
                string str = "USE ContactsList;\nDELETE FROM People WHERE last ='" + String.Concat(fullname[0].Where(c => !Char.IsWhiteSpace(c))) + "' AND first = '" + String.Concat(fullname[1].Where(c => !Char.IsWhiteSpace(c))) + "'";

                SqlCommand cmd = new SqlCommand(str, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    ClearAll();

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
                UpdateLookUp();

            }
        }
    }
}
