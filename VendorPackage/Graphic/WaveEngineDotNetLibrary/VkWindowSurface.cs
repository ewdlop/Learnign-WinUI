using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary
{
    public unsafe partial class VkContext
    {
        private readonly VkSurfaceKHR VkWindowSurface;
        private readonly IntPtr hwnd;
        private readonly IntPtr hinstance;

        public VkContext()
        {

        }
        public VkContext(IntPtr WindowHandle, IntPtr InstanceHandle)
        {
            hwnd = WindowHandle;
            hinstance = InstanceHandle;
        }
        private void CreateSurface()
        {
            VkWin32SurfaceCreateInfoKHR createInfo = new VkWin32SurfaceCreateInfoKHR()
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
                hwnd = hwnd,
                //hinstance = Process.GetCurrentProcess().Handle,
                hinstance = hinstance
            };

            fixed (VkSurfaceKHR* surfacePtr = &VkWindowSurface)
            {
                VkHelper.CheckErrors(VulkanNative.vkCreateWin32SurfaceKHR(VkInstance, &createInfo, null, surfacePtr));
            }
        }
    }
}
