using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Window;

public unsafe class VkWindowSurface : IVkSurface
{
    private readonly VkWindowHandle _vkWindowHandle;

    private VkSurfaceKHR vkWindowSurface;
    public VkSurfaceKHR SurfaceKHR => vkWindowSurface;

    public VkWindowSurface(VkWindowHandle vkWindowHandle)
    {
        _vkWindowHandle = vkWindowHandle;
    }

    public void CreateSurface(VkInstance vkInstance)
    {
        VkWin32SurfaceCreateInfoKHR createInfo = new VkWin32SurfaceCreateInfoKHR()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
            hwnd = _vkWindowHandle.hwnd,
            hinstance = _vkWindowHandle.hinstance
        };

        fixed (VkSurfaceKHR* surfacePtr = &vkWindowSurface)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateWin32SurfaceKHR(vkInstance, &createInfo, null, surfacePtr));
        }
    }
}
