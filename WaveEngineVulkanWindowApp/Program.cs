// See https://aka.ms/new-console-template for more information
using WaveEngineDotNetLibrary;
using System.Windows.Forms;
using System.Diagnostics;

const uint WIDTH = 800;
const uint HEIGHT = 600;

Form window = new Form()
{
    Text = "Vulkan Triangle Rasterization",
    Size = new System.Drawing.Size((int)WIDTH, (int)HEIGHT),
    FormBorderStyle = FormBorderStyle.FixedToolWindow
};
window.Show();
IntPtr hInstance = Process.GetCurrentProcess().Handle;
VkContext vkContext = new VkContext(window.Handle, hInstance);
vkContext.CreateInstance();