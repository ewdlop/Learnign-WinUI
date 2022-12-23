using Evergine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private VkCommandPool vkCommandPool;
    private VkCommandBuffer[] vkCommandBuffers;

    private void CreateCommandPool()
    {
        QueueFamilyIndices queueFamilyIndices = FindQueueFamilies(vkPhysicalDevice);

        VkCommandPoolCreateInfo poolInfo = new VkCommandPoolCreateInfo()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO,
            queueFamilyIndex = queueFamilyIndices.graphicsFamily.Value,
            flags = 0, // Optional,
        };

        fixed (VkCommandPool* commandPoolPtr = &vkCommandPool)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateCommandPool(vkDevice,
                                                                  &poolInfo,
                                                                  null,
                                                                  commandPoolPtr));
        }
    }

    private void CreateCommandBuffers()
    {
        vkCommandBuffers = new VkCommandBuffer[vkSwapChainFramebuffers.Length];

        VkCommandBufferAllocateInfo allocInfo = new VkCommandBufferAllocateInfo()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            commandPool = vkCommandPool,
            level = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandBufferCount = (uint)vkCommandBuffers.Length,
        };

        fixed (VkCommandBuffer* commandBuffersPtr = &vkCommandBuffers[0])
        {
            VkHelper.CheckErrors(VulkanNative.vkAllocateCommandBuffers(vkDevice,
                                                                       &allocInfo,
                                                                       commandBuffersPtr));
        }

        for (uint i = 0; i < vkCommandBuffers.Length; i++)
        {
            VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo()
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO,
                flags = 0, // Optional
                pInheritanceInfo = null, // Optional
            };

            VkHelper.CheckErrors(VulkanNative.vkBeginCommandBuffer(vkCommandBuffers[i],
                                                                   &beginInfo));

            // Pass
            VkClearValue clearColor = new VkClearValue()
            {
                color = new VkClearColorValue(0.0f, 0.0f, 0.0f, 1.0f),
            };

            VkRenderPassBeginInfo renderPassInfo = new VkRenderPassBeginInfo()
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO,
                renderPass = vkRenderPass,
                framebuffer = vkSwapChainFramebuffers[i],
                renderArea = new VkRect2D(0,
                                          0,
                                          vkSwapChainExtent.width,
                                          vkSwapChainExtent.height),
                clearValueCount = 1,
                pClearValues = &clearColor,
            };

            VulkanNative.vkCmdBeginRenderPass(vkCommandBuffers[i],
                                              &renderPassInfo,
                                              VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE);

            // Draw
            VulkanNative.vkCmdBindPipeline(vkCommandBuffers[i],
                                           VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
                                           vkGraphicsPipeline);

            VulkanNative.vkCmdDraw(vkCommandBuffers[i], 3, 1, 0, 0);

            VulkanNative.vkCmdEndRenderPass(vkCommandBuffers[i]);

            VkHelper.CheckErrors(VulkanNative.vkEndCommandBuffer(vkCommandBuffers[i]));
        }
    }
}
