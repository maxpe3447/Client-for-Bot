using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientForBot
{
    class WebSetting
    {
        public static string Ip{ get; private set;}
        public static  string Port { get; private set; }
        public static void SetIpPort(string ip, string port)
        {
            using (RegistryKey registry = Registry.CurrentUser.OpenSubKey(CheckAndCreate(), writable: true))
            {
                registry.SetValue("IP", ip);
                registry.SetValue("Port", port);
            }      
        }

        public static (string, string) GetIpPort()
        {
            using (RegistryKey registry = Registry.CurrentUser.OpenSubKey(CheckAndCreate(), writable: true))
            {
                Ip = registry.GetValue("IP").ToString();
                Port = registry.GetValue("Port").ToString();
            }
            return (Ip, Port);
        }
        private static string CheckAndCreate()
        {
            using (RegistryKey registry = Registry.CurrentUser.OpenSubKey(MainDirGer))
            {
                if (!registry.GetSubKeyNames().ToList().Contains(ProgDirReg))
                    registry.CreateSubKey(ProgDirReg);
            }
            using(RegistryKey registry = Registry.CurrentUser.OpenSubKey($@"{MainDirGer}\{ProgDirReg}", writable: true))
            {
                if(!(registry.GetValueNames().ToList().Where(v=> v == "IP" || v== "Port").ToList().Count == 2))
                {
                    registry.SetValue("IP", "1.1.1.1");
                    registry.SetValue("Port", "1");
                }

            }
            return $@"{MainDirGer}\{ProgDirReg}";
        }

        private static string MainDirGer = @"SOFTWARE";
        private static string ProgDirReg = @"ClientForBot";
    }
}
