﻿using Evergine.Bindings.Vulkan;
using WaveEngineDotNetLibrary.Vulkan;

namespace WaveEngineDotNetLibrary.Window;

public unsafe class VkWindowSurface : IVkSurface
{
    protected readonly WindowHandle _vkWindowHandle;

    protected VkSurfaceKHR _vkWindowSurface;
    public VkSurfaceKHR SurfaceKHR => _vkWindowSurface;

    public VkWindowSurface(WindowHandle vkWindowHandle)
    {
        _vkWindowHandle = vkWindowHandle;
    }

    public virtual void CreateSurface(VkInstance vkInstance)
    {
        VkWin32SurfaceCreateInfoKHR createInfo = new VkWin32SurfaceCreateInfoKHR()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
            hwnd = _vkWindowHandle.hwnd,
            hinstance = _vkWindowHandle.hinstance
        };

        fixed (VkSurfaceKHR* surfacePtr = &_vkWindowSurface)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateWin32SurfaceKHR(vkInstance, &createInfo, null, surfacePtr));
        }
    }
}
