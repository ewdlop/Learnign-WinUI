using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    public class VkFrame
    {
        public VkFence VkFence { get; set; }
        public VkSemaphore vkImageAvailableSemaphore { get; set; }
        public VkSemaphore vkRenderFinishedSemaphore { get; set; }
    }
}
