﻿using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary;

public unsafe partial class VkContext
{
    private ref struct VkSwapChainSupportDetails
    {
        public VkSurfaceCapabilitiesKHR surfaceCapabilities;
        public VkSurfaceFormatKHR[] surfaceFormats;
        public VkPresentModeKHR[] presentModes;
    }
}
