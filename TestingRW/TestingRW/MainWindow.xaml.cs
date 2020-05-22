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
using System.IO;
using System.Reflection;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Collections.ObjectModel;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;

namespace TestingRW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private static class Globals
        {
            public static System.IntPtr halo1dll;
            public static System.IntPtr halo2dll;
            public static System.IntPtr MCCexe;

        }












        //for reading/writing from process memory
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);






        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        public MainWindow()
        {
            //InitializeComponent();

            Console.WriteLine("test: " + ReadLevelCode());
            //System.Windows.Application.Current.Shutdown();


        }

        public static void GetBaseAddresses()
        {
            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);
            ProcessModule myProcessModule;
            ProcessModuleCollection myProcessModuleCollection = myProcess.Modules;

            // Find the base address of our desired modules.
            for (int i = 0; i < myProcessModuleCollection.Count; i++)
            {
                myProcessModule = myProcessModuleCollection[i];

                switch (myProcessModule.ModuleName)
                {
                    case "halo1.dll":
                        Globals.halo1dll = myProcessModule.BaseAddress;
                        Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "halo2.dll":
                        Globals.halo2dll = myProcessModule.BaseAddress;
                        Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "MCC-Win64-Shipping.exe":
                        Globals.MCCexe = myProcessModule.BaseAddress;
                        Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "MCC-Win64-Shipping-Winstore.exe":
                        Globals.MCCexe = myProcessModule.BaseAddress;
                        Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    default:
                        break;


                }

            }

        }


        public static string ReadLevelCode()
        {
            string test = "empty lol";

            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);

            
            byte[] buffer = new byte[3];
            //get levelname from loaded cp instead
            IntPtr baseaddy = Globals.halo1dll + 0x0224B5F8;
            int[] offsets = { 0x28, 0x23 };
            ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesRead);
            test = (Encoding.ASCII.GetString(buffer) + " (" + bytesRead.ToString() + "bytes)");
            CloseHandle(processHandle);




            /*  ProcessModule myProcessModule;
              ProcessModuleCollection myProcessModuleCollection = myProcess.Modules;
              Console.WriteLine("Base addresses of the modules associated "
                  + "with 'mcc' are:");
              // Display the 'BaseAddress' of each of the modules.
              for (int i = 0; i < myProcessModuleCollection.Count; i++)
              {
                  myProcessModule = myProcessModuleCollection[i];
                  Console.WriteLine(myProcessModule.ModuleName + " : "
                      + myProcessModule.BaseAddress);
              }*/



            return test;
        }


        private void DumpH1CP_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("DUMPING CP");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


            byte[] buffer = new byte[4956160];
            //get levelname from loaded cp instead
            IntPtr baseaddy = Globals.halo1dll + 0x0224B5F8;
            int[] offsets = { 0x28, 0x14 };

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(@"E:\dumpedcp.bin", buffer);
                FileInfo test = new FileInfo(@"E:\scripts\dumpedfiles\dumpedcp.bin");
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED CP, LENGTH: " + test.Length.ToString());
                }
            }
            else
                throw new Win32Exception();
            CloseHandle(processHandle);
        }


        private void InjectH1CP_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"C:\dumpedcp.bin"))
            {
                Console.WriteLine("Injecting CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = @"C:\dumpedcp.bin";
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());

                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.halo1dll + 0x0224B5F8;
                int[] offsets = { 0x28, 0x14 };
                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesWritten))
                    Console.Write("Successfully inject CP, bytes written: " + bytesWritten.ToString());
                else
                    throw new Win32Exception();

                CloseHandle(processHandle);

            }
            else
                Console.WriteLine("file doesn't exist you silly");
            }



        public static IntPtr FindPointerAddy(IntPtr hProc, IntPtr ptr, int[] offsets)
        {
            var buffer = new byte[IntPtr.Size];

            foreach (int i in offsets)
            {
                ReadProcessMemory(hProc, ptr, buffer, buffer.Length, out int read);

                ptr = (IntPtr.Size == 4)
                ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i)
                : ptr = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);
            }
            return ptr;
        }

    }
}
