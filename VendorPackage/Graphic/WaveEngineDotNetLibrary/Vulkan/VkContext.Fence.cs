using Evergine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private VkFence vkFence;
    
    private void CreateFence()
    {
        VkFenceCreateInfo vkFenceCreateInfo = new VkFenceCreateInfo()
        {
            flags = VkFenceCreateFlags.VK_FENCE_CREATE_SIGNALED_BIT,
            sType = VkStructureType.VK_STRUCTURE_TYPE_FENCE_CREATE_INFO
        };
        fixed (VkFence* fencePtr = &vkFence)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateFence(vkDevice, &vkFenceCreateInfo, null, fencePtr));
        }
    }
}
