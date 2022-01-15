using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private VkFramebuffer[] vkSwapChainFramebuffers;

    private void CreateFramebuffers()
    {
        vkSwapChainFramebuffers = new VkFramebuffer[vkSwapChainImageViews.Length];

        for (int i = 0; i < vkSwapChainImageViews.Length; i++)
        {

            VkFramebufferCreateInfo framebufferInfo = new VkFramebufferCreateInfo();
            framebufferInfo.sType = VkStructureType.VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO;
            framebufferInfo.renderPass = vkRenderPass;
            framebufferInfo.attachmentCount = 1;

            fixed (VkImageView* attachments = &vkSwapChainImageViews[i])
            {
                framebufferInfo.pAttachments = attachments;
            }

            framebufferInfo.width = vkSwapChainExtent.width;
            framebufferInfo.height = vkSwapChainExtent.height;
            framebufferInfo.layers = 1;

            fixed (VkFramebuffer* swapChainFramebufferPtr = &vkSwapChainFramebuffers[i])
            {
                VkHelper.CheckErrors(VulkanNative.vkCreateFramebuffer(vkDevice, &framebufferInfo, null, swapChainFramebufferPtr));
            }
        }
    }
}
