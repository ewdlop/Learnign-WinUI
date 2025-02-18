using Evergine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    protected VkFramebuffer[] vkSwapChainFramebuffers;

    protected virtual void CreateFramebuffers()
    {
        vkSwapChainFramebuffers = new VkFramebuffer[vkSwapChainImageViews.Length];


        for (int i = 0; i < vkSwapChainImageViews.Length; i++)
        {

            VkFramebufferCreateInfo framebufferInfo = new VkFramebufferCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO,
                renderPass = vkRenderPass,
                attachmentCount = 1,
                width = vkSwapChainExtent.width,
                height = vkSwapChainExtent.height,
                layers = 1
            };

            fixed (VkImageView* attachments = &vkSwapChainImageViews[i])
            {
                framebufferInfo.pAttachments = attachments;
            }

            fixed (VkFramebuffer* swapChainFramebufferPtr = &vkSwapChainFramebuffers[i])
            {
                VkHelper.CheckErrors(VulkanNative.vkCreateFramebuffer(vkDevice, &framebufferInfo, null, swapChainFramebufferPtr));
            }
        }
    }
}
