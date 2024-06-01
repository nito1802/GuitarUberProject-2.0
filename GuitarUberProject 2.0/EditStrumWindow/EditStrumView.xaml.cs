using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GitarUberProject.EditStrumWindow
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19

        // ...
    }

    /// <summary>
    /// Interaction logic for EditStrumView.xaml
    /// </summary>
    public partial class EditStrumView : Window
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public bool ResultDialog { get; set; }

        public EditStrumView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
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
            ResultDialog = false;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            ResultDialog = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResultDialog = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Enter && btnApply.IsEnabled)
            {
                ResultDialog = true;
                Close();
            }
        }
    }

    public class MouseWheelButtonEventTrigger : System.Windows.Interactivity.EventTrigger
    {
        public MouseWheelButtonEventTrigger()
        {
            EventName = "MouseDown";
        }

        protected override void OnEvent(EventArgs eventArgs)
        {
            MouseButtonEventArgs mbea = eventArgs as MouseButtonEventArgs;
            if (mbea != null && mbea.ChangedButton == MouseButton.Middle)
                base.OnEvent(eventArgs);
        }
    }
}