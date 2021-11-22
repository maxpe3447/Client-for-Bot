using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ClientForBot
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    public partial class ChatingWindow : Window
    {
        Tg_Bot.Client client;
        public ChatingWindow()
        {
            InitializeComponent();
        }
        public ChatingWindow(Tg_Bot.Client client) : this()
        {
            this.client = client;

            //var items = client.GetUsers();
            GetUsers();
        }

        private void GetUsers()
        {
            var users = client.GetUsers();
            if(users == null)
            {
                MessageBox.Show("Error connection", "Error",MessageBoxButton.OKCancel ,MessageBoxImage.Warning);
                return;
            }
            lbUsers.ItemsSource = users;
            
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            tbMsgLogs.AppendText($"To -> {lbUsers.SelectedItem}: \n{tbMessage.Text}\n");
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetUsers();
            tbMessage.Text += "1";
        }
    }
}
