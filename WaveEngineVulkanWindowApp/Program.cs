using WaveEngineDotNetLibrary;
using System.Diagnostics;
using WaveEngineDotNetLibrary.Window;

const uint WIDTH = 800;
const uint HEIGHT = 600;

Form window = new Form()
{
    Text = "Vulkan Triangle Rasterization",
    Size = new Size((int)WIDTH, (int)HEIGHT),
    FormBorderStyle = FormBorderStyle.FixedToolWindow
};
window.Show();
IntPtr hInstance = Process.GetCurrentProcess().Handle;

IVkSurface vkWindowSurface = new VkWindowSurface(new VkWindowHandle(window.Handle, hInstance));
VkContext vkContext = new VkContext(WIDTH, HEIGHT, vkWindowSurface);
vkContext.Init();