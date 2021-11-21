using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Tg_Bot
{
    class Client
    {
        public string Ip { get; set; } = "1.1.1.1";
        public string Port { get; set; } = "1";
        public static string MainDir { get; } = "ServiceFiles/";
        private Socket clientSocket;
        private IPEndPoint endPoint;
        private string okCode;
        public List<string> ServerStatusOperation = new List<string>();
        public static string Separator { get; } = "________";
        public bool isConnect { get; set; } = false;
        
        private void StartSocket() { clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); }
        private void StopSocket()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        public string[] GetFilesName(string dir_ = null)
        {
            if (okCode == null) return null;
            StartSocket();
            clientSocket.Connect(endPoint);
            ServerStatusOperation.Clear();

            string dir = string.Empty;

            if (dir_ == null)
            {
                SendData(((int)CodeForServer.GetFiles).ToString());
                Logger_($"Operation {CodeForServer.GetFiles}", false);
            }
            else
            {
                SendData(((int)CodeForServer.GetFilesFromDir).ToString());
                Logger_($"Operation {CodeForServer.GetFilesFromDir}");

                SendData(dir_.Remove(dir_.IndexOf("..."), "...".Length));
                Logger_($"Send dir: {dir_}", false);

                dir = "...|";
            }
            string dirAns = AnswerFromServer();
            dirAns = dirAns == okCode ? string.Empty : dirAns;
            dir = dir.ToString() + dirAns;
                Logger_($"Get dir: {dir.Split('|').Length}", false);

                SendData(okCode);

                string files = AnswerFromServer();
                Logger_($"Get files: {files.Split('|').Length}", false);
            
            
            StopSocket();

            return (dir + $"|{Separator}|" + files).Split("|");
        }

        public string GetTextFromFile(string fileName)
        {
            if (okCode == null) return null;

            StartSocket();
            clientSocket.Connect(endPoint);
            ServerStatusOperation.Clear();

            SendData(((int)CodeForServer.GetTextFromFile).ToString());
            Logger_($"Operation {CodeForServer.GetTextFromFile}");

            SendData(fileName);

            string text = AnswerFromServer();
            Logger_("Geting text", false);

            StopSocket();
            return text;
        }

        public void SetTextFromFile(string fileName, string text)
        {
            if (okCode == null) return;

            StartSocket();
            clientSocket.Connect(endPoint);
            ServerStatusOperation.Clear();

            SendData(((int)CodeForServer.SetTextFromFile).ToString());
            Logger_($"Operation {CodeForServer.SetTextFromFile}");


            SendData(fileName);

            Logger_("Send file");

            SendData(text);

            StopSocket();
        }
        public void CreateNewFile(string fileName)
        {
            if (okCode == null) return;

            StartSocket();
            clientSocket.Connect(endPoint);
            ServerStatusOperation.Clear();

            SendData(((int)CodeForServer.CreateNewFile).ToString());
            Logger_($"Operation {CodeForServer.CreateNewFile}");


            SendData(fileName);

            Logger_("Create!");

            StopSocket();

        }

        public void CreateEvent(string dateTime, string message)
        {
            if (okCode == null) return;

            StartSocket();
            clientSocket.Connect(endPoint);
            ServerStatusOperation.Clear();

            SendData(((int)CodeForServer.CreateEvent).ToString());
            Logger_($"Operation {CodeForServer.CreateEvent}");

            SendData(dateTime);

            Logger_("Send DateTime");

            SendData(message);

            StopSocket();
        }

        public void EditFileName(string oldFileName, string newFileName)
        {
            if (okCode == null) return;

            StartSocket();
            clientSocket.Connect(endPoint);
            ServerStatusOperation.Clear();

            SendData(((int)CodeForServer.EditFileName).ToString());
            Logger_($"Operation {CodeForServer.EditFileName}");

            SendData(oldFileName);
            Logger_($"Send old name: {oldFileName}");

            SendData(newFileName);
            Logger_($"Send new name: {newFileName}");

            StopSocket();
        }
        public void DeleteFile(string fileName)
        {
            if (okCode == null) return;

            StartSocket();
            clientSocket.Connect(endPoint);
            ServerStatusOperation.Clear();

            SendData(((int)CodeForServer.DeleteFile).ToString());
            Logger_($"Operation {CodeForServer.EditFileName}");

            SendData(fileName);
            Logger_($"Delete {fileName}");

            StopSocket();
        }

        public List<string> GetTableNames()
        {
            if (okCode == null) return null;
            StartSocket();
            clientSocket.Connect(endPoint);
            ServerStatusOperation.Clear();

            SendData(((int)CodeForServer.GetTableNamesFromDB).ToString());
            Logger_($"Operation {CodeForServer.GetTableNamesFromDB}", false);

            var result = new List<string>(AnswerFromServer().Trim('\0').Split('|'));

            StopSocket();

            return result;
        }
        public void Connect(string ip = null, string port = null)
        {
            if (!isConnect)
            {
                ServerStatusOperation.Clear();
                if (ip != null && port != null)
                {
                    Port = port;
                    Ip = ip;
                }

                endPoint = new IPEndPoint(IPAddress.Parse(Ip), int.Parse(Port));

                StartSocket();
                try
                {
                    clientSocket.Connect(endPoint);
                }
                catch
                {
                    ServerStatusOperation.Add("ERROR Conected!");
                    return;
                }

                if (clientSocket.Poll(2000, SelectMode.SelectError))
                {
                    ServerStatusOperation.Add("ERROR Conected!");
                    return;
                }
                SendData(((int)CodeForServer.ForConnect).ToString());

                okCode = AnswerFromServer();
                isConnect = true;
                Logger_("CONNECTED!", false);

                StopSocket();
            }
        }
        private string AnswerFromServer()
        {
            int size = clientSocket.ReceiveBufferSize;
            var buffer = new byte[size];
            StringBuilder sizeOfAnswer = new StringBuilder();
            do
            {
                size = clientSocket.Receive(buffer);
                sizeOfAnswer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (clientSocket.Available > 0);

            return sizeOfAnswer.ToString();
        }
        private void Logger_(string msg = "", bool isWaitAnswer = true)
        {
            if (isWaitAnswer)
            {
                if (AnswerFromServer() == okCode)
                    ServerStatusOperation.Add(msg + "  -> Ok!");
                else
                    ServerStatusOperation.Add(msg + "  -> Bad!");
                return;
            }
            ServerStatusOperation.Add(msg + "  -> Ok!");


        }
        private void SendData(string data)
        {
            byte[] byteText = Encoding.UTF8.GetBytes(data);
            clientSocket.SendBufferSize = byteText.Length;
            clientSocket.Send(byteText);
        }
    }
    enum CodeForServer
    {
        None, ForConnect, GetFiles, GetTextFromFile, SetTextFromFile, CreateNewFile, CreateEvent, EditFileName, DeleteFile, GetFilesFromDir, GetTableNamesFromDB
    }
}
