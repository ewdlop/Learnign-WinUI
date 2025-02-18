using Evergine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    protected ref struct VkSwapChainSupportDetails
    {
        public VkSurfaceCapabilitiesKHR surfaceCapabilities;
        public Span<VkSurfaceFormatKHR> surfaceFormats;
        public Span<VkPresentModeKHR> presentModes;
    }
}
