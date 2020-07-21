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
            public static System.IntPtr halo3dll;
            public static System.IntPtr haloreachdll;
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





        string hardcodedpath = @"C:\dumpedcp.bin";




        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        public MainWindow()
        {
            //InitializeComponent();

            //Console.WriteLine("test: " + ReadLevelCode());
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

                    case "halo3.dll":
                        Globals.halo3dll = myProcessModule.BaseAddress;
                        Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "haloreach.dll":
                        Globals.haloreachdll = myProcessModule.BaseAddress;
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


        private void ReadLevelCode(object sender, RoutedEventArgs e)
        {
            string test = "empty lol";
            if (Convert.ToBoolean(radioH1.IsChecked))
            {


                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


                byte[] buffer = new byte[3];
                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.halo1dll + 0x0224D1D8;
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



                
            }
            else if (Convert.ToBoolean(radioH2.IsChecked))
            {

                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


                byte[] buffer = new byte[3];
                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.halo2dll + 0x062ADBE8;
                int[] offsets = { 0x17 };
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



               
            }
            else if (Convert.ToBoolean(radioH3.IsChecked))
            {

                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


                byte[] buffer = new byte[3];
                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.halo3dll + 0x00E71750;
                int[] offsets = { -0x7DFFE4 };
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




            }
            else if (Convert.ToBoolean(radioHR.IsChecked))
            {

                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


                byte[] buffer = new byte[3];
                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.haloreachdll + 0x0279F780;
                int[] offsets = { -0xA0FFEC };
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




            }
            Console.WriteLine("levelcode: " + test);
        }


        private void DumpCP_Click(object sender, RoutedEventArgs e)
        {
            //check which game is selected and call dat function
            if (Convert.ToBoolean(radioH1.IsChecked))
            {
                H1Dump(sender, e);
            }
            else if (Convert.ToBoolean(radioH2.IsChecked))
            {
                H2Dump(sender, e);
            }
            else if (Convert.ToBoolean(radioH3.IsChecked))
            {
                H3Dump(sender, e);
            }
            else if (Convert.ToBoolean(radioHR.IsChecked))
            {
                HRDump(sender, e);
            }

        }


        private void InjectCP_Click(object sender, RoutedEventArgs e)
        {
            //check which game is selected and call dat function
            //check which game is selected and call dat function
            if (Convert.ToBoolean(radioH1.IsChecked))
            {
                H1Inject(sender, e);
            }
            else if (Convert.ToBoolean(radioH2.IsChecked))
            {
                H2Inject(sender, e);
            }
            else if (Convert.ToBoolean(radioH3.IsChecked))
            {
                H3Inject(sender, e);
            }
            else if (Convert.ToBoolean(radioHR.IsChecked))
            {
                HRInject(sender, e);
            }

        }



        private void H1Dump(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("DUMPING H1 CP");
            GetBaseAddresses();

        Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
        IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


        byte[] buffer = new byte[4956160];
        //get levelname from loaded cp instead
        IntPtr baseaddy = Globals.halo1dll + 0x0224D1D8;
        int[] offsets = { 0x28, 0x14 };

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(hardcodedpath, buffer);
                FileInfo test = new FileInfo(hardcodedpath);
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED H1 CP, LENGTH: " + test.Length.ToString());
                }
            }
            else
                throw new Win32Exception();
        CloseHandle(processHandle);
        
        
        
        }



        private void H1Inject(object sender, RoutedEventArgs e)
        {

            if (File.Exists(hardcodedpath))
            {
                Console.WriteLine("Injecting H1 CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = hardcodedpath;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());

                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.halo1dll + 0x0224D1D8;
                int[] offsets = { 0x28, 0x14 };


                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesWritten))
                {
                    Console.WriteLine("Successfully injected H1 CP, bytes written: " + bytesWritten.ToString());
                }
                else
                    throw new Win32Exception();

                CloseHandle(processHandle);

            }
            else
                Console.WriteLine("file doesn't exist you silly");


        }




        private void H2Dump(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("DUMPING H2 CP");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


            byte[] buffer = new byte[4186112];
            IntPtr baseaddy = Globals.halo2dll + 0x062ADBE8;
            int[] offset = new int[1];
            if (Convert.ToBoolean(radioCP1.IsChecked))
            {
                offset[0] =   0x0 ; //first cp
            }
            else
            {
                offset[0] =  0x3FE000 ; //second cp
            }

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(hardcodedpath, buffer);
                FileInfo test = new FileInfo(hardcodedpath);
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED H2 CP, LENGTH: " + test.Length.ToString());
                }
            }
            else
                throw new Win32Exception();
            CloseHandle(processHandle);



        }


        private void H2Inject(object sender, RoutedEventArgs e)
        {

            if (File.Exists(hardcodedpath))
            {
                Console.WriteLine("Injecting H2 CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = hardcodedpath;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());

               
                IntPtr baseaddy = Globals.halo2dll + 0x062ADBE8;
                int[] offset = new int[1];
                if (Convert.ToBoolean(radioCP1.IsChecked))
                {
                    offset[0] = 0x0; //first cp
                }
                else
                {
                    offset[0] = 0x3FE000; //second cp
                }


                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesWritten))
                {
                    Console.WriteLine("Successfully injected H2 CP, bytes written: " + bytesWritten.ToString());
                }
                else
                    throw new Win32Exception();

                CloseHandle(processHandle);

            }
            else
                Console.WriteLine("file doesn't exist you silly");


        }



        private void H3Dump(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("DUMPING H3 CP");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


            byte[] buffer = new byte[8257536];
            IntPtr baseaddy = Globals.halo3dll + 0x00E71750;
            int[] offset = new int[1];
            if (Convert.ToBoolean(radioCP1.IsChecked))
            {
                offset[0] = -0x7E0000; //first cp
            }
            else
            {
                offset[0] = 0x0; //second cp
            }

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(hardcodedpath, buffer);
                FileInfo test = new FileInfo(hardcodedpath);
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED H3 CP, LENGTH: " + test.Length.ToString());
                }
            }
            else
                throw new Win32Exception();
            CloseHandle(processHandle);



        }


        private void H3Inject(object sender, RoutedEventArgs e)
        {

            if (File.Exists(hardcodedpath))
            {
                Console.WriteLine("Injecting H3 CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = hardcodedpath;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());


                IntPtr baseaddy = Globals.halo3dll + 0x00E71750;
                int[] offset = new int[1];
                if (Convert.ToBoolean(radioCP1.IsChecked))
                {
                    offset[0] = -0x7E0000; //first cp
                }
                else
                {
                    offset[0] = 0x0; //second cp
                }


                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesWritten))
                {
                    Console.WriteLine("Successfully injected H3 CP, bytes written: " + bytesWritten.ToString());
                }
                else
                    throw new Win32Exception();

                CloseHandle(processHandle);

            }
            else
                Console.WriteLine("file doesn't exist you silly");


        }


        private void HRDump(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("DUMPING HR CP");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


            byte[] buffer = new byte[10551296];
            IntPtr baseaddy = Globals.haloreachdll + 0x0279F780;
            int[] offset = new int[1];
            if (Convert.ToBoolean(radioCP1.IsChecked))
            {
                offset[0] = -0xA10000; //first cp
            }
            else
            {
                offset[0] = 0x0; //second cp
            }

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(hardcodedpath, buffer);
                FileInfo test = new FileInfo(hardcodedpath);
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED HR CP, LENGTH: " + test.Length.ToString());
                }
            }
            else
                throw new Win32Exception();
            CloseHandle(processHandle);



        }


        private void HRInject(object sender, RoutedEventArgs e)
        {

            if (File.Exists(hardcodedpath))
            {
                Console.WriteLine("Injecting HR CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = hardcodedpath;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());


                IntPtr baseaddy = Globals.haloreachdll + 0x0279F780;
                int[] offset = new int[1];
                if (Convert.ToBoolean(radioCP1.IsChecked))
                {
                    offset[0] = -0xA10000; //first cp
                }
                else
                {
                    offset[0] = 0x0; //second cp
                }

                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesWritten))
                {
                    Console.WriteLine("Successfully injected HR CP, bytes written: " + bytesWritten.ToString());
                }
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
