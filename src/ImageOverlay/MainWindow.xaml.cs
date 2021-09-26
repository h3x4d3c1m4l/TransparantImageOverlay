using Gma.System.MouseKeyHook;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace ImageOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // select image
            OpenFileDialog ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if ((ofd.ShowDialog() ?? false) == false)
            {
                Environment.Exit(1);
            }

            // load image
            _imageControl.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));

            // setup key listener for resizing and moving window
            var mm_GlobalHook = Hook.GlobalEvents();
            mm_GlobalHook.MouseDown += Mm_GlobalHook_MouseDown;
            mm_GlobalHook.MouseUp += Mm_GlobalHook_MouseUp;
            mm_GlobalHook.MouseMove += Mm_GlobalHook_MouseMove;
        }

        private int _oldMouseX, _oldMouseY;

        private MouseButtons _oldMouseButtons;

        private void Mm_GlobalHook_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) && (e.Button.HasFlag(MouseButtons.Left) || e.Button.HasFlag(MouseButtons.Right) || e.Button.HasFlag(MouseButtons.Middle)))
            {
                _oldMouseX = e.X;
                _oldMouseY = e.Y;
                _oldMouseButtons = e.Button;
            }
        }

        private void Mm_GlobalHook_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _oldMouseButtons -= _oldMouseButtons;
        }

        private void Mm_GlobalHook_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if (_oldMouseButtons.HasFlag(MouseButtons.Left))
                {
                    // move
                    Left += e.X - _oldMouseX;
                    Top += e.Y - _oldMouseY;

                    _oldMouseX = e.X;
                    _oldMouseY = e.Y;
                }
                else if (_oldMouseButtons.HasFlag(MouseButtons.Right))
                {
                    // resize
                    Width += e.X - _oldMouseX;
                    Height += e.Y - _oldMouseY;

                    _oldMouseX = e.X;
                    _oldMouseY = e.Y;
                }
                else if (_oldMouseButtons.HasFlag(MouseButtons.Middle))
                {
                    Opacity += (e.Y - _oldMouseY) / 100f;
                    if (Opacity > 1)
                    {
                        Opacity = 1;
                    }
                    else if (Opacity < 0)
                    {
                        Opacity = 0;
                    }

                    _oldMouseX = e.X;
                    _oldMouseY = e.Y;
                }
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Make entire window and everything in it "transparent" to the Mouse
            IntPtr windowHwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(windowHwnd);

            // Make the button "visible" to the Mouse
            //var buttonHwndSource = (HwndSource)HwndSource.FromVisual(btn);
            //var buttonHwnd = buttonHwndSource.Handle;
            //WindowsServices.SetWindowExNotTransparent(buttonHwnd);
        }
    }
}
