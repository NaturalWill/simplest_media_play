using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDirect3DDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("simplest_video_play_direct3d.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Init(IntPtr resourcePointer, UInt32 width, UInt32 height);

        [DllImport("simplest_video_play_direct3d.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Play(IntPtr resourcePointer);

        [DllImport("simplest_video_play_direct3d.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Render();
        [DllImport("simplest_video_play_direct3d.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Cleanup();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Play(new WindowInteropHelper(this).Handle);
            initialize();
        }
        #region D3D
        private void initialize()
        {
            //    vm.LiveConfigList[0].Initialize(new WindowInteropHelper(this).Handle,
            //               (int)ImageHost.ActualWidth, (int)ImageHost.ActualHeight);
            IntPtr surface =
            Init(new WindowInteropHelper(this).Handle, (uint)gImageHost.ActualWidth, (uint)gImageHost.ActualHeight);

            if (surface != IntPtr.Zero)
            {
                d3dImage.Lock();
                d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface);
                d3dImage.Unlock();

                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        private void UninitializeRendering()
        {

            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (d3dImage.IsFrontBufferAvailable)
            {
                d3dImage.Lock();
                Render();
                // Invalidate the whole area:
                d3dImage.AddDirtyRect(new Int32Rect(0, 0, d3dImage.PixelWidth, d3dImage.PixelHeight));
                d3dImage.Unlock();
            }
        }
        private void d3dImage_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3dImage.IsFrontBufferAvailable)
            {
                initialize();
            }
            else
            {
                // Cleanup:
                UninitializeRendering();
                Cleanup();
            }
        }
        #endregion
    }
}
