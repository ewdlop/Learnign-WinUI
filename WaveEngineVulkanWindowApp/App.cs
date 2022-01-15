using System.Diagnostics;
using WaveEngineDotNetLibrary;
using WaveEngineDotNetLibrary.Vulkan;
using WaveEngineDotNetLibrary.Window;

namespace WaveEngineVulkanWindowApp;

internal class App
{
    private readonly Form window;
    private readonly IVkSurface vkWindowSurface;
    private readonly VkContext vkContext;
    const uint WIDTH = 800;
    const uint HEIGHT = 600;

    public App()
    {
        window = new Form()
        {
            Text = "Vulkan Triangle Rasterization",
            Size = new Size((int)WIDTH, (int)HEIGHT),
            FormBorderStyle = FormBorderStyle.FixedToolWindow
        };
        IntPtr hInstance = Process.GetCurrentProcess().Handle;
        vkWindowSurface = new VkWindowSurface(new VkWindowHandle(window.Handle, hInstance));
        vkContext = new VkContext(WIDTH, HEIGHT, vkWindowSurface);
    }
    public void Run()
    {
        window.Show();
        vkContext.Init();
        MainLoop();
        CleanUp();
    }

    private void MainLoop()
    {
        bool isClosing = false;
        window.FormClosing += (s, e) =>
        {
            isClosing = true;
        };

        while (!isClosing)
        {
            Application.DoEvents(); //Applicaiton event

            vkContext.DrawFrame();
        }

        vkContext.CheckDeviceWaitIdleError();
    }

    private void CleanUp()
    {
        vkContext.CleanUp();
        window.Dispose();
        window.Close();
    }
}
