using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using WaveEngineDotNetLibrary;
using WaveEngineDotNetLibrary.Vulkan;
using WaveEngineDotNetLibrary.Window;
//using Windows.Win32;
//using Windows.Win32.Foundation;
//using Windows.Win32.UI.WindowsAndMessaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3WindowApp;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    //protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    //{
    //    m_window = new MainWindow();
    //    hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
    //    hInstance = Process.GetCurrentProcess().Handle;
    //    SetWindowSize(hWnd, WIDTH, HEIGHT);
    //    m_window.Activate();

    //    vkWindowSurface = new VkWindowSurface(new VkWindowHandle(hWnd, hInstance));
    //    vkContext = new VkContext(WIDTH, HEIGHT, vkWindowSurface);
    //    vkContext.Init();
    //    MainLoop();
    //    CleanUp();
    //}

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        hWnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
        hInstance = Process.GetCurrentProcess().Handle;
        SetWindowSize(hWnd, WIDTH, HEIGHT);
        m_window.Activate();
        vkWindowSurface = new VkWindowSurface(new WindowHandle(hWnd, hInstance));
        vkContext = new VkContext(WIDTH, HEIGHT, vkWindowSurface);
        vkContext.Init();
        MainLoop();
        CleanUp();
    }

    private static void SetWindowSize(IntPtr hwnd, int width, int height)
    {
        // Win32 uses pixels and WinUI 3 uses effective pixels, so you should apply the DPI scale factor
        int dpi = PInvoke.User32.GetDpiForWindow(hwnd);
        float scalingFactor = (float)dpi / 96;
        width = (int)(width * scalingFactor);
        height = (int)(height * scalingFactor);

        PInvoke.User32.SetWindowPos(hwnd, PInvoke.User32.SpecialWindowHandles.HWND_TOP,
                                    0, 0, width, height,
                                    PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE);
    }

    private void MainLoop()
    {
        bool isClosing = false;
        m_window.Closed += (s, e) =>
        {
            isClosing = true;
        };

        while (!isClosing)
        {
            vkContext.DrawFrame();
        }

        vkContext.CheckDeviceWaitIdleError();
    }

    private void CleanUp()
    {
        vkContext.CleanUp();
        m_window.Close();
    }

    private Window m_window;
    private IntPtr hWnd;
    private IntPtr hInstance;
    private IVkSurface vkWindowSurface;
    private VkContext vkContext;
    const int WIDTH = 800;
    const int HEIGHT = 600;
}
