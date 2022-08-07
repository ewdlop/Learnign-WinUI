using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private ref struct VkSwapChainSupportDetails
    {
        public VkSurfaceCapabilitiesKHR surfaceCapabilities;
        public Span<VkSurfaceFormatKHR> surfaceFormats;
        public Span<VkPresentModeKHR> presentModes;
    }
}
