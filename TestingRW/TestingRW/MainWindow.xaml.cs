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
//using System.Windows.Shapes;
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
using System.Windows.Forms;
using System.Windows.Threading;

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

            public static int gameindicator = 0x038E7CC8;

            public static int haloreachDRflag = 0x259DACC;
            public static int haloreachCPaddy = 0x2839810;
           public static int haloreachSeedAddy = 0x1119E18;

            public static int halo2DRflag = 0xdf0b04;
            public static int halo2CPaddy = 0x01412EF8;

            public static int halo3DRflag = 0x1C5DF48;
            public static int halo3CPaddy = 0x01DB5B40;

            public static Int32 ProcessID;
            public static IntPtr GlobalProcessHandle;

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

        [DllImport("kernel32.dll")]
        static extern bool GetHandleInformation(IntPtr hObject, out uint lpdwFlags);




        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;





        public MainWindow()
        {
            InitializeComponent();

            //Console.WriteLine("test: " + ReadLevelCode());
            //System.Windows.Application.Current.Shutdown();

            DispatcherTimer dtClockTime = new DispatcherTimer();

            //dtClockTime.Interval = new TimeSpan(0, 0, 1); //in Hour, Minutes, Second.
            //dtClockTime.Tick += dtClockTime_Tick;

            //dtClockTime.Start();





            this.Title = "Prototype HCM";

        }

        private void dtClockTime_Tick(object sender, EventArgs e)
        {

           

        }



        private void DumpBrowseClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result.ToString() == "OK" && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    if (Directory.Exists(dialog.SelectedPath))
                    {
                        ChosenDump.Text = dialog.SelectedPath;
                    }
                    else
                    {
                        Console.WriteLine("folder doesn't exist you silly");
                        System.Windows.MessageBox.Show("folder doesn't exist you silly");
                    }
                }
            }
        }

        private void InjectBrowseClick(object sender, RoutedEventArgs e)
        {

            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result.ToString() == "OK" && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    if (File.Exists(dialog.FileName))
                    {
                        ChosenInject.Text = dialog.FileName;
                    }
                    else
                    {
                        Console.WriteLine("file doesn't exist you silly");
                        System.Windows.MessageBox.Show("file doesn't exist you silly");
                    }
                }
            }
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
                        //Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "halo2.dll":
                        Globals.halo2dll = myProcessModule.BaseAddress;
                        //Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "halo3.dll":
                        Globals.halo3dll = myProcessModule.BaseAddress;
                        //Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "haloreach.dll":
                        Globals.haloreachdll = myProcessModule.BaseAddress;
                        //Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "MCC-Win64-Shipping.exe":
                        Globals.MCCexe = myProcessModule.BaseAddress;
                        //Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    case "MCC-Win64-Shipping-Winstore.exe":
                        Globals.MCCexe = myProcessModule.BaseAddress;
                        //Console.WriteLine(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress);
                        break;

                    default:
                        break;


                }

            }

        }

        public static string CheckGame(object sender, RoutedEventArgs e)
        {
            string loadedgame = null;


            Console.WriteLine("checking game");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);

            byte[] buffer = new byte[1];
            IntPtr baseaddy = Globals.MCCexe + Globals.gameindicator;
            int[] offset = { 0x0 };

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int DRbytesRead))
            {
                switch (buffer[0])
                {
                    case 0:
                        loadedgame = "halo 1";
                        Console.WriteLine("game is halo 1");
                        break;

                    case 1:
                        loadedgame = "halo 2";
                        Console.WriteLine("game is halo 2");
                        break;

                    case 2:
                        loadedgame = "halo 3";
                        Console.WriteLine("game is halo 3");
                        break;

                    case 6:
                        loadedgame = "halo reach";
                        Console.WriteLine("game is halo reach");
                        break;



                    default:
                        break;

                }
            }
            else
                throw new Win32Exception();


            //CloseHandle(processHandle);



            return loadedgame;


        }

        private void DebugClick(object sender, RoutedEventArgs e)
        {

            IntPtr processHandle;
            //yes yes shouldn't use exceptions as control flow but lazy
            //need to make this better in proper HCM

            //the only part that REALLY needs the try-catch is OpenProcess
            //that or i could just check if the process exists first but eh could still have access issues. 
            //actually yeah should still do it so I can popup warning about needing to run as admin


            //so code should be

            /*
                        check if process exists
                        check if our global process id matches it's id
                        if so do nothing (we're attached) (well actually proceed to next checks but eh)
                        if not (but process exists)
                        then try opening it and setting global process id etc
                        catch - failed to open - popup warning about needing admin priv + mcc eac off. then do nothing (not attached)
                        if process didn't exist
                        do nothing (not attached)
            */

            try
            {
                Process testProcess = Process.GetProcessById(Globals.ProcessID);
                if (testProcess.ProcessName == "MCC-Win64-Shipping")
                {
                    Log.Content = "MCC already attached at " + (Convert.ToString(Globals.ProcessID, 16)).ToUpper();
                }
                else
                {
                    throw new Exception("blah");
                }

            }
            catch
            {
                try
                {
                    Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                    processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);
                    Globals.ProcessID = myProcess.Id;
                    Globals.GlobalProcessHandle = processHandle;
                    Log.Content = "MCC found with ID " + (Convert.ToString(myProcess.Id, 16)).ToUpper();
                }
                catch
                {
                    Log.Content = "MCC not found";
                    return;
                }
            }
            //let's check if we can check whether the process is open





        }


        private void ReadLevelCode(object sender, RoutedEventArgs e)
        {
            string test = "unsupported game! hr only for now";
            string loadedgame = CheckGame(sender, e);
            if (loadedgame == "halo 1")
            {
                return; //unsupporting non hr for now

                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


                byte[] buffer = new byte[3];
                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.halo1dll + 0x0224C1D8;
                int[] offsets = { 0x28, 0x23 };
                ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesRead);
                test = (Encoding.ASCII.GetString(buffer) + " (" + bytesRead.ToString() + "bytes)");
                CloseHandle(processHandle);





            }
            else if (loadedgame == "halo 2")
            {
               
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


                byte[] buffer = new byte[3];
                IntPtr baseaddy = Globals.halo2dll + 0xE34303;
                ReadProcessMemory(processHandle, baseaddy, buffer, buffer.Length, out int bytesRead);
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
            else if (loadedgame == "halo 3")
            {
                //unsupporting non hr for now
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


                byte[] buffer = new byte[3];
                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.halo3dll + 0x1D4F22B;
                ReadProcessMemory(processHandle, baseaddy, buffer, buffer.Length, out int bytesRead);
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
            else if (loadedgame == "halo reach")
            {
                return;
                GetBaseAddresses();

                IntPtr processHandle = Globals.GlobalProcessHandle;


                byte[] buffer = new byte[3];
                //get levelname from loaded cp instead
                //IntPtr baseaddy = Globals.haloreachdll + Globals.haloreachCPaddy;
                //int[] offsets = { -0xA0FFEC };
                //ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesRead);
                //test = (Encoding.ASCII.GetString(buffer) + " (" + bytesRead.ToString() + "bytes)");
                //CloseHandle(processHandle);




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
            else
            {
                return; //unsupporting non hr for now
            }
            Log.Content = "Log: " + loadedgame + " levelcode " + test;
            Console.WriteLine("levelcode: " + test);
        }


        private void DumpCP_Click(object sender, RoutedEventArgs e)
        {
            string loadedgame = CheckGame(sender, e);
            //check which game is selected and call dat function
            if (loadedgame == "halo 1")
            {
                //H1Dump(sender, e);
            }
            else if (loadedgame == "halo 2")
            {
                H2Dump(sender, e);
            }
            else if (loadedgame == "halo 3")
            {
                H3Dump(sender, e);
            }
            else if (loadedgame == "halo reach")
            {
                //HRDump(sender, e);
            }

        }


        private void InjectCP_Click(object sender, RoutedEventArgs e)
        {
            //check which game is selected and call dat function
            string loadedgame = CheckGame(sender, e);
            if (loadedgame == "halo 1")
            {
                //H1Inject(sender, e);
            }
            else if (loadedgame == "halo 2")
            {
                H2Inject(sender, e);
            }
            else if (loadedgame == "halo 3")
            {
                H3Inject(sender, e);
            }
            else if (loadedgame == "halo reach")
            {
                //HRInject(sender, e);
            }

        }



        private void H1Dump(object sender, RoutedEventArgs e)
        {
            string path = ChosenDump.Text + "\\" + ChosenFilename.Text + ".bin";

            if (!IsValidPath(path))
            {
                System.Windows.MessageBox.Show("There was something wrong with your chosen dumping file path");
                return;
            }


            Console.WriteLine("DUMPING H1 CP");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


            byte[] buffer = new byte[4956160];
            //get levelname from loaded cp instead
            IntPtr baseaddy = Globals.halo1dll + 0x0224C1D8;
            int[] offsets = { 0x28, 0x14 };

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(path, buffer);
                FileInfo test = new FileInfo(path);
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED H1 CP, LENGTH: " + test.Length.ToString());
                    Log.Content = "Log: H1: Successfully dumped " + "\\" + ChosenFilename.Text + ".bin";
                }
            }
            else
                throw new Win32Exception();
            CloseHandle(processHandle);



        }



        private void H1Inject(object sender, RoutedEventArgs e)
        {

            string path = ChosenInject.Text;


            if (File.Exists(path))
            {
                Console.WriteLine("Injecting H1 CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = path;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());

                //get levelname from loaded cp instead
                IntPtr baseaddy = Globals.halo1dll + 0x0224C1D8;
                int[] offsets = { 0x28, 0x14 };


                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offsets), buffer, buffer.Length, out int bytesWritten))
                {
                    Console.WriteLine("Successfully injected H1 CP, bytes written: " + bytesWritten.ToString());
                    Log.Content = "Log: H1: Successfully injected " + "\\" + Path.GetFileName(path);
                }
                else
                    throw new Win32Exception();

                CloseHandle(processHandle);

            }
            else
            {
                Console.WriteLine("file doesn't exist you silly");
                System.Windows.MessageBox.Show("file doesn't exist you silly");
            }

        }




        private void H2Dump(object sender, RoutedEventArgs e)
        {

            string path = ChosenDump.Text + "\\" + ChosenFilename.Text + ".bin";

            if (!IsValidPath(path))
            {
                System.Windows.MessageBox.Show("There was something wrong with your chosen dumping file path");
                return;
            }

            Console.WriteLine("DUMPING H2 CP");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


            bool DRflag;
            byte[] DRbuffer = new byte[1];
            IntPtr DRbaseaddy = Globals.halo2dll + Globals.halo2DRflag;

            if (ReadProcessMemory(processHandle, DRbaseaddy, DRbuffer, DRbuffer.Length, out int DRbytesRead))
            {
                DRflag = Convert.ToBoolean(DRbuffer[0]);
            }
            else
                throw new Win32Exception();


            byte[] buffer = new byte[4186112];
            IntPtr baseaddy = Globals.halo2dll + Globals.halo2CPaddy;
            int[] offset = new int[1];
            if (!DRflag)
            {
                offset[0] = 0x0; //first cp
            }
            else
            {
                offset[0] = 0x3FE000; //second cp
            }

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(path, buffer);
                FileInfo test = new FileInfo(path);
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED H2 CP, LENGTH: " + test.Length.ToString());
                    Log.Content = "Log: H2: Successfully dumped " + "\\" + ChosenFilename.Text + ".bin";
                }
            }
            else
                throw new Win32Exception();
            CloseHandle(processHandle);


        }


        private void H2Inject(object sender, RoutedEventArgs e)
        {
            string path = ChosenInject.Text;

            if (File.Exists(path))
            {
                Console.WriteLine("Injecting H2 CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = path;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());



                bool DRflag;
                byte[] DRbuffer = new byte[1];
                IntPtr DRbaseaddy = Globals.halo2dll + Globals.halo2DRflag;

                if (ReadProcessMemory(processHandle, DRbaseaddy, DRbuffer, DRbuffer.Length, out int DRbytesRead))
                {
                    DRflag = Convert.ToBoolean(DRbuffer[0]);
                }
                else
                    throw new Win32Exception();



                IntPtr baseaddy = Globals.halo2dll + Globals.halo2CPaddy;
                int[] offset = new int[1];
                if (!DRflag)
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
                    Log.Content = "Log: H2: Successfully injected " + "\\" + Path.GetFileName(path);
                }
                else
                    throw new Win32Exception();


                //binary search stuff, remove this for releases
                /*
                string hardcodesafepath = @"E:\scripts\moredumps\Burnt_WCstart.bin";
                if (File.Exists(hardcodesafepath))
                { Console.WriteLine("well the file is valid"); }
                FileStream fs3 = new FileStream(hardcodesafepath, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer3 = System.IO.File.ReadAllBytes(hardcodesafepath);
                //Read block of bytes from stream into the byte array
                fs3.Read(buffer3, 0, System.Convert.ToInt32(fs3.Length));
                //Close the File Stream
                fs3.Close();
                int startoffset = int.Parse(Startoffset.Text, System.Globalization.NumberStyles.HexNumber);
                int endoffset = int.Parse(Endoffset.Text, System.Globalization.NumberStyles.HexNumber);
                Console.Write("parsing correct? A: " + startoffset);
                Console.Write("parsing correct? B: " + endoffset);
                Console.WriteLine("length of new buffer4; " + Convert.ToString(endoffset - startoffset, 16));
                var buffer4 = new byte[endoffset - startoffset];
                Array.Copy(buffer3, startoffset, buffer4, 0, endoffset - startoffset);
                offset[0] = offset[0] + startoffset;
                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer4, buffer4.Length, out int bytesWritten3))
                {
                    Console.WriteLine("successfully pasted binary search thing from: " + Startoffset.Text + " to " + Endoffset.Text);
                    Log.Content = "Log: HR: Successfully injected " + "\\" + Path.GetFileName(path) + " with clean from " + Startoffset.Text + " to " + Endoffset.Text;
                }
                else
                    throw new Win32Exception();
*/

                CloseHandle(processHandle);

            }
            else
            {
                Console.WriteLine("file doesn't exist you silly");
                System.Windows.MessageBox.Show("file doesn't exist you silly");
            }

        }



        private void H3Dump(object sender, RoutedEventArgs e)
        {

            string path = ChosenDump.Text + "\\" + ChosenFilename.Text + ".bin";

            if (!IsValidPath(path))
            {
                System.Windows.MessageBox.Show("There was something wrong with your chosen dumping file path");
                return;
            }

            Console.WriteLine("DUMPING H3 CP");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


            bool DRflag;
            byte[] DRbuffer = new byte[1];
            IntPtr DRbaseaddy = Globals.halo3dll + Globals.halo3DRflag;

            if (ReadProcessMemory(processHandle, DRbaseaddy, DRbuffer, DRbuffer.Length, out int DRbytesRead))
            {
                DRflag = Convert.ToBoolean(DRbuffer[0]);
            }
            else
                throw new Win32Exception();


            byte[] buffer = new byte[8257536];
            IntPtr baseaddy = Globals.halo3dll + Globals.halo3CPaddy;
            int[] offset = new int[1];
            if (!DRflag)
            {
                offset[0] = 0x0; //first cp
            }
            else
            {
                offset[0] = 0x7E0000; //second cp
            }

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(path, buffer);
                FileInfo test = new FileInfo(path);
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED H3 CP, LENGTH: " + test.Length.ToString());
                    Log.Content = "Log: H3: Successfully dumped " + "\\" + ChosenFilename.Text + ".bin";
                }
            }
            else
                throw new Win32Exception();
            CloseHandle(processHandle);


        }


        private void H3Inject(object sender, RoutedEventArgs e)
        {
            string path = ChosenInject.Text;

            if (File.Exists(path))
            {
                Console.WriteLine("Injecting H3 CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = path;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());



                bool DRflag;
                byte[] DRbuffer = new byte[1];
                IntPtr DRbaseaddy = Globals.halo3dll + Globals.halo3DRflag;

                if (ReadProcessMemory(processHandle, DRbaseaddy, DRbuffer, DRbuffer.Length, out int DRbytesRead))
                {
                    DRflag = Convert.ToBoolean(DRbuffer[0]);
                }
                else
                    throw new Win32Exception();



                IntPtr baseaddy = Globals.halo3dll + Globals.halo3CPaddy;
                int[] offset = new int[1];
                if (!DRflag)
                {
                    offset[0] = 0x0; //first cp
                }
                else
                {
                    offset[0] = 0x7E0000; //second cp
                }

                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesWritten))
                {
                    Console.WriteLine("Successfully injected H3 CP, bytes written: " + bytesWritten.ToString());
                    Log.Content = "Log: H3: Successfully injected " + "\\" + Path.GetFileName(path);
                }
                else
                    throw new Win32Exception();


                //binary search stuff, remove this for releases
                /*
                string hardcodesafepath = @"E:\scripts\moredumps\Burnt_WCstart.bin";
                if (File.Exists(hardcodesafepath))
                { Console.WriteLine("well the file is valid"); }
                FileStream fs3 = new FileStream(hardcodesafepath, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer3 = System.IO.File.ReadAllBytes(hardcodesafepath);
                //Read block of bytes from stream into the byte array
                fs3.Read(buffer3, 0, System.Convert.ToInt32(fs3.Length));
                //Close the File Stream
                fs3.Close();
                int startoffset = int.Parse(Startoffset.Text, System.Globalization.NumberStyles.HexNumber);
                int endoffset = int.Parse(Endoffset.Text, System.Globalization.NumberStyles.HexNumber);
                Console.Write("parsing correct? A: " + startoffset);
                Console.Write("parsing correct? B: " + endoffset);
                Console.WriteLine("length of new buffer4; " + Convert.ToString(endoffset - startoffset, 16));
                var buffer4 = new byte[endoffset - startoffset];
                Array.Copy(buffer3, startoffset, buffer4, 0, endoffset - startoffset);
                offset[0] = offset[0] + startoffset;
                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer4, buffer4.Length, out int bytesWritten3))
                {
                    Console.WriteLine("successfully pasted binary search thing from: " + Startoffset.Text + " to " + Endoffset.Text);
                    Log.Content = "Log: HR: Successfully injected " + "\\" + Path.GetFileName(path) + " with clean from " + Startoffset.Text + " to " + Endoffset.Text;
                }
                else
                    throw new Win32Exception();
*/

                CloseHandle(processHandle);

            }
            else
            {
                Console.WriteLine("file doesn't exist you silly");
                System.Windows.MessageBox.Show("file doesn't exist you silly");
            }

        }



        private void HRDump(object sender, RoutedEventArgs e)
        {

            string path = ChosenDump.Text + "\\" + ChosenFilename.Text + ".bin";

            if (!IsValidPath(path))
            {
                System.Windows.MessageBox.Show("There was something wrong with your chosen dumping file path");
                return;
            }

            Console.WriteLine("DUMPING HR CP");
            GetBaseAddresses();

            Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, myProcess.Id);


            bool DRflag;
            byte[] DRbuffer = new byte[1];
            IntPtr DRbaseaddy = Globals.haloreachdll + Globals.haloreachDRflag;

            if (ReadProcessMemory(processHandle, DRbaseaddy, DRbuffer, DRbuffer.Length, out int DRbytesRead))
            {
                DRflag = Convert.ToBoolean(DRbuffer[0]);
            }
            else
                throw new Win32Exception();


            byte[] buffer = new byte[10551296];
            IntPtr baseaddy = Globals.haloreachdll + Globals.haloreachCPaddy;
            int[] offset = new int[1];
            if (!DRflag)
            {
                offset[0] = -0xA10000; //first cp
            }
            else
            {
                offset[0] = 0x0; //second cp
            }

            if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer, buffer.Length, out int bytesRead))
            {
                File.WriteAllBytes(path, buffer);
                FileInfo test = new FileInfo(path);
                if (File.Exists(test.ToString()) && test.Length > 1000)
                {
                    Console.WriteLine("SUCESSFULLY DUMPED HR CP, LENGTH: " + test.Length.ToString());
                    Log.Content = "Log: HR: Successfully dumped " + "\\" + ChosenFilename.Text + ".bin";
                }
            }
            else
                throw new Win32Exception();
            CloseHandle(processHandle);



        }


        private void HRInject(object sender, RoutedEventArgs e)
        {
            string path = ChosenInject.Text;

            if (File.Exists(path))
            {
                Console.WriteLine("Injecting HR CP");
                GetBaseAddresses();

                Process myProcess = Process.GetProcessesByName("MCC-Win64-Shipping")[0];
                IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, myProcess.Id);

                string filename = path;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer = System.IO.File.ReadAllBytes(filename);
                //Read block of bytes from stream into the byte array
                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();
                Console.WriteLine("ready to inject, buffer length: " + buffer.Length.ToString());



                bool DRflag;
                byte[] DRbuffer = new byte[1];
                IntPtr DRbaseaddy = Globals.haloreachdll + Globals.haloreachDRflag;

                if (ReadProcessMemory(processHandle, DRbaseaddy, DRbuffer, DRbuffer.Length, out int DRbytesRead))
                {
                    DRflag = Convert.ToBoolean(DRbuffer[0]);
                }
                else
                    throw new Win32Exception();


                //need to read and store player int
                //int numberofbytestopaste = 84;


                byte[] buffer2 = new byte[4];
                IntPtr baseaddy2 = Globals.haloreachdll + Globals.haloreachCPaddy;
                int[] offset2 = new int[1];
                if (!DRflag)
                {
                    offset2[0] = -0xA10000 + 0x594A74; //first cp
                }
                else
                {
                    offset2[0] = 0x0 + 0x594A74; //second cp
                }

                if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy2, offset2), buffer2, buffer2.Length, out int bytesRead))
                {
                    Console.WriteLine("Successfully read hr playerint" + buffer2[0].ToString() + buffer2[1].ToString() + buffer2[2].ToString() + buffer2[3].ToString());
                    //Log.Content = "Successfully read hr playerint";
                }
                else
                    throw new Win32Exception();

                byte[] buffer5 = new byte[16];
                IntPtr baseaddy5 = Globals.haloreachdll + Globals.haloreachCPaddy;
                int[] offset5 = new int[1];
                if (!DRflag)
                {
                    offset5[0] = -0xA10000 + 0x58F4F0; //first cp
                }
                else
                {
                    offset5[0] = 0x0 + 0x58F4F0; //second cp
                }

                if (ReadProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy5, offset5), buffer5, buffer5.Length, out int bytesRead5))
                {
                    Console.WriteLine("Successfully read hr playerint 2" + buffer5[0].ToString());
                    //Log.Content = "Successfully read hr playerint";
                }
                else
                    throw new Win32Exception();


                /////


                IntPtr baseaddy = Globals.haloreachdll + Globals.haloreachCPaddy;
                int[] offset = new int[1];
                if (!DRflag)
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
                    Log.Content = "Log: HR: Successfully injected " + "\\" + Path.GetFileName(path);
                }
                else
                    throw new Win32Exception();

                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset2), buffer2, buffer2.Length, out int bytesWritten2))
                {
                    Console.WriteLine("Successfully pasted hr player int");
                    //Log.Content = "Successfully pasted hr player int";
                }
                else
                    throw new Win32Exception();

                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset5), buffer5, buffer5.Length, out int bytesWritten3))
                {
                    Console.WriteLine("Successfully pasted hr player int 2");
                    //Log.Content = "Successfully pasted hr player int";
                }
                else
                    throw new Win32Exception();

                //binary search stuff, remove this for releases
                /*
                string hardcodesafepath = @"E:\scripts\moredumps\Burnt_WCstart.bin";
                if (File.Exists(hardcodesafepath))
                { Console.WriteLine("well the file is valid"); }
                FileStream fs3 = new FileStream(hardcodesafepath, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] buffer3 = System.IO.File.ReadAllBytes(hardcodesafepath);
                //Read block of bytes from stream into the byte array
                fs3.Read(buffer3, 0, System.Convert.ToInt32(fs3.Length));
                //Close the File Stream
                fs3.Close();
                int startoffset = int.Parse(Startoffset.Text, System.Globalization.NumberStyles.HexNumber);
                int endoffset = int.Parse(Endoffset.Text, System.Globalization.NumberStyles.HexNumber);
                Console.Write("parsing correct? A: " + startoffset);
                Console.Write("parsing correct? B: " + endoffset);
                Console.WriteLine("length of new buffer4; " + Convert.ToString(endoffset - startoffset, 16));
                var buffer4 = new byte[endoffset - startoffset];
                Array.Copy(buffer3, startoffset, buffer4, 0, endoffset - startoffset);
                offset[0] = offset[0] + startoffset;
                if (WriteProcessMemory(processHandle, FindPointerAddy(processHandle, baseaddy, offset), buffer4, buffer4.Length, out int bytesWritten3))
                {
                    Console.WriteLine("successfully pasted binary search thing from: " + Startoffset.Text + " to " + Endoffset.Text);
                    Log.Content = "Log: HR: Successfully injected " + "\\" + Path.GetFileName(path) + " with clean from " + Startoffset.Text + " to " + Endoffset.Text;
                }
                else
                    throw new Win32Exception();
*/

                CloseHandle(processHandle);

            }
            else
            {
                Console.WriteLine("file doesn't exist you silly");
                System.Windows.MessageBox.Show("file doesn't exist you silly");
            }


        }

        // down here is where I put code I nicked from StackOverflow

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


        private bool IsValidPath(string path, bool exactPath = true)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (exactPath)
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
                else
                {
                    isValid = Path.IsPathRooted(path);
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }

            return isValid;
        }





    }




}