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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientForBot
{
    public partial class MainWindow : Window
    {
        Tg_Bot.Client client;
        FileNameContextMenueMode fileNameContextMenueMode;
        public MainWindow()
        {
            InitializeComponent();
            client = new Tg_Bot.Client();
            if (client.ServerStatusOperation.Count != 0)
                ShowLogs();

            WebSetting.GetIpPort();
            tbIp.Text = client.Ip = WebSetting.Ip;
            tbPort.Text = client.Port = WebSetting.Port;
            client.Connect();

            Closing += (a, s) => { WebSetting.SetIpPort(tbIp.Text, tbPort.Text); };
            tbLogs.MouseDoubleClick += (s, e) =>{ tbLogs.Text = "Logs:"; client.ServerStatusOperation.Clear(); };
            
        }

        private void bCreateEvent_Click(object sender, RoutedEventArgs e)
        {

            string temp = dpDate.ToString();
            if (string.IsNullOrEmpty(temp))
            {
                MessageBox.Show("No data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string date = Convert.ToDateTime(temp).ToUniversalTime().ToString();
            tbLogs.AppendText($"\n {date}");
            

            client.CreateEvent(date, tbInfo.Text);
            foreach (var log in client.ServerStatusOperation)
            {
                tbLogs.AppendText("\n" + log);
            }
        }
        private void GetFilesFromServer(string dir = null)
        {
            lvFileNames.Items.Clear();

            string[] files = client.GetFilesName(dir);

            foreach (var log in client.ServerStatusOperation)
            {
                tbLogs.AppendText("\n"+log);
            }
            if (files != null)
                foreach (var file in files)
                {
                    if (file.IndexOf(Tg_Bot.Client.MainDir) != -1)
                    {
                        lvFileNames.Items.Add(file.Remove(startIndex: 0, Tg_Bot.Client.MainDir.Length));
                    }
                    else if (file == string.Empty)
                    {
                        continue;
                    }
                    else
                    {
                        lvFileNames.Items.Add(file);
                    }
                }
        }
        //private void bGetFileNames_Click(object sender, RoutedEventArgs e)
        //{
        //    GetFilesFromServer();
        //}

        private string lastSelectFileName;
        private void lvFileNames_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                string lastSelectFileName_ = lvFileNames.SelectedItems[0].ToString();
                if (lastSelectFileName_ == Tg_Bot.Client.Separator)
                {
                    return;
                }
                else if(lastSelectFileName_ == "...")
                {
                    GetFilesFromServer();
                }
                else if (!lastSelectFileName_.Contains("..."))
                {
                    tbInfo.Text = client.GetTextFromFile(lastSelectFileName_);
                }
                else if (lastSelectFileName_.Contains(".db"))
                {
                    lvFileNames.Items.Clear();
                    var tables = client.GetTableNames();
                    foreach (var table in tables)
                    {
                        lvFileNames.Items.Add(table);
                    }
                }
                else
                {
                    GetFilesFromServer(lastSelectFileName_);
                }
                lastSelectFileName = lastSelectFileName_;
                ShowLogs();
            }
        }

        private void bSendInfo_Click(object sender, RoutedEventArgs e)
        {
            client.SetTextFromFile(lastSelectFileName, tbInfo.Text.ToString());
            ShowLogs();
        }

        private void ShowLogs()
        {
            foreach (var log in client.ServerStatusOperation)
            {
                tbLogs.AppendText('\n' + log);
            }
        }

        private void bCreateFileEvent_Click(object sender, RoutedEventArgs e)
        {
            tbForSetName.Visibility = Visibility.Visible;
            fileNameContextMenueMode = FileNameContextMenueMode.Create;
        }

        private void bEditFile_Click(object sender, RoutedEventArgs e)
        {
            tbForSetName.Visibility = Visibility.Visible;
            fileNameContextMenueMode = FileNameContextMenueMode.Edit;

        }

        private void bReconnect_Click(object sender, RoutedEventArgs e)
        {
            client.Connect(tbIp.Text, tbPort.Text);
            ShowLogs();
        }

        private void tbInfo_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                lastSelectFileName = string.Empty;
                tbInfo.Text = string.Empty;
            }
        }

        private void tbForSetName_KeyUp(object sender, KeyEventArgs e)
        {
            if (tbForSetName.Visibility == Visibility.Visible)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        string newName = tbForSetName.Text;

                        if (fileNameContextMenueMode == FileNameContextMenueMode.Create)
                            client.CreateNewFile(newName);
                        else if (fileNameContextMenueMode == FileNameContextMenueMode.Edit)
                            client.EditFileName(lvFileNames.SelectedItems[0].ToString(), newName);
                        bGetFileNames_Click(null, null);

                        tbForSetName.Text = string.Empty;
                        tbForSetName.Visibility = Visibility.Hidden;
                        break;
                    case Key.Escape:
                        tbForSetName.Text = string.Empty;
                        tbForSetName.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        private void bDeleteFile_Click(object sender, RoutedEventArgs e)
        {
            client.DeleteFile(lvFileNames.SelectedItem.ToString());
            bGetFileNames_Click(null, null);
        }

        private void tbForSetName_MouseEnter(object sender, MouseEventArgs e)
        {
            tbForSetName.Text = string.Empty;
        }

        private void bGetFileNames_Click(object sender, RoutedEventArgs e)
        {
            GetFilesFromServer();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
        }

        private void tbInfo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

    enum FileNameContextMenueMode : byte
    {
        Create, Edit
    }
}
