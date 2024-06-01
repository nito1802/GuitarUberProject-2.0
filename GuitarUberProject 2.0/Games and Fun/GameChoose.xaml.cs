using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace GitarUberProject
{
    /// <summary>
    /// Interaction logic for ChordEdit.xaml
    /// </summary>
    public partial class GameChoose : Window, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public GameChoose()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();

            double screenWidth = System.Windows.SystemParameters.WorkArea.Width;
            double screenHeight = System.Windows.SystemParameters.WorkArea.Height;

            double windowRight = this.Left + this.ActualWidth;

            if (windowRight > screenWidth)
            {
                double tresholdX = windowRight - screenWidth;
                this.Left -= tresholdX;
            }
        }

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton1 || e.ChangedButton == MouseButton.XButton2)
            {
                Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; //INotifyPropertyChanged

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void btnRecognizeChords_Click(object sender, RoutedEventArgs e)
        {
            RecognizeChord gameWindow = new RecognizeChord();

            gameWindow.Owner = System.Windows.Application.Current.MainWindow;
            gameWindow.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
            gameWindow.Width *= App.CustomScaleX;
            gameWindow.Height *= App.CustomScaleY;
            gameWindow.ShowDialog();
        }

        private void btnFindChrodsOnGuitar_Click(object sender, RoutedEventArgs e)
        {
            FindChordsOnGuitar gameWindow = new FindChordsOnGuitar();

            gameWindow.Owner = System.Windows.Application.Current.MainWindow;
            gameWindow.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
            gameWindow.Width *= App.CustomScaleX;
            gameWindow.Height *= App.CustomScaleY;
            gameWindow.ShowDialog();
        }

        private void btnRecognizeNotes_Click(object sender, RoutedEventArgs e)
        {
            RecognizeNote gameWindow = new RecognizeNote();

            gameWindow.Owner = System.Windows.Application.Current.MainWindow;
            gameWindow.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
            gameWindow.Width *= App.CustomScaleX;
            gameWindow.Height *= App.CustomScaleY;
            gameWindow.ShowDialog();
        }

        private void btnFindNotesOnGuitar_Click(object sender, RoutedEventArgs e)
        {
            FindNotesOnGuitar gameWindow = new FindNotesOnGuitar();

            gameWindow.Owner = System.Windows.Application.Current.MainWindow;
            gameWindow.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
            gameWindow.Width *= App.CustomScaleX;
            gameWindow.Height *= App.CustomScaleY;
            gameWindow.ShowDialog();
        }
    }
}