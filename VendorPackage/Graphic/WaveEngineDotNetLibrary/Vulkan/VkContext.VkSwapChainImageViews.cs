using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan
{
    public unsafe partial class VkContext
    {
        private VkImageView[] vkSwapChainImageViews;

        private void CreateImageViews()
        {
            vkSwapChainImageViews = new VkImageView[vkSwapChainImages.Length];

            for (int i = 0; i < vkSwapChainImages.Length; i++)
            {
                VkImageViewCreateInfo createInfo = new()
                {
                    sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
                    pNext = null,
                    flags = VkImageViewCreateFlags.None,
                    image = default,
                    viewType = VkImageViewType.VK_IMAGE_VIEW_TYPE_1D,
                    format = VkFormat.VK_FORMAT_UNDEFINED,
                    components = default,
                    subresourceRange = default
                };
                createInfo.sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO;
                createInfo.image = vkSwapChainImages[i];
                createInfo.viewType = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D;
                createInfo.format = vkSwapChainImageFormat;
                createInfo.components.r = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY;
                createInfo.components.g = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY;
                createInfo.components.b = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY;
                createInfo.components.a = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY;
                createInfo.subresourceRange.aspectMask = VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT;
                createInfo.subresourceRange.baseMipLevel = 0;
                createInfo.subresourceRange.levelCount = 1;
                createInfo.subresourceRange.baseArrayLayer = 0;
                createInfo.subresourceRange.layerCount = 1;

                fixed (VkImageView* swapChainImageViewPtr = &vkSwapChainImageViews[i])
                {
                    VkHelper.CheckErrors(VulkanNative.vkCreateImageView(vkDevice, &createInfo, null, swapChainImageViewPtr));
                }
            }
        }
    }
}
