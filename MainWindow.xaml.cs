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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

            tbIp.Text = client.Ip;
            tbPort.Text = client.Port.ToString();
        }

        private void bCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            
            client.CreateEvent(dpDate.ToString(),tbInfo.Text);
            foreach (var log in client.ServerStatusOperation)
            {
                tbLogs.AppendText("\n" + log);
            }
        }

        private void bGetFileNames_Click(object sender, RoutedEventArgs e)
        {
            lvFileNames.Items.Clear();

            string[] files = client.GetFilesName();

            foreach (var log in client.ServerStatusOperation)
            {
                tbLogs.AppendText("\n"+log);
            }
            if (files != null)
                foreach (var file in files)
                {
                    lvFileNames.Items.Add(file.Remove(0, "ServiceFiles/".Length));
                }
        }

        private string lastSelectFileName;
        private void lvFileNames_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                lastSelectFileName = lvFileNames.SelectedItems[0].ToString();
                if (tbInfo != null) 
                tbInfo.Text = client.GetTextFromFile(lastSelectFileName);

                ShowLogs();
            }
        }

        private void bSendInfo_Click(object sender, RoutedEventArgs e)
        {
            client.SetTextFromFile(lastSelectFileName, tbInfo.Text);
            ShowLogs();
        }

        private void ShowLogs()
        {
            foreach (var log in client.ServerStatusOperation)
            {
                tbLogs.AppendText('\n' + log);
            }
        }

        private void tbLogs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tbLogs.Text = "Logs:";
        }


        private void lvFileNames_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Right)
            {
               
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
            client.Connect(tbIp.Text, int.Parse(tbPort.Text));
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
    }

    enum FileNameContextMenueMode : byte
    {
        Create, Edit
    }
}
