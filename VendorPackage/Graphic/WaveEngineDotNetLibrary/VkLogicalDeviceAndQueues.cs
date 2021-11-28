using System.Collections.Immutable;
using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary
{
    public unsafe partial class VkContext
    {
        private readonly VkDevice VkDevice;
        private readonly VkQueue VkGraphicsQueue;
        private readonly VkQueue VkPresentQueue;

        private ImmutableArray<string> VkDeviceExtensionsName { get; init; } = ImmutableArray.Create(new string[]
        {
            "VK_KHR_swapchain"
        });

        private void CreateLogicalDevice()
        {
        }
    }
}
