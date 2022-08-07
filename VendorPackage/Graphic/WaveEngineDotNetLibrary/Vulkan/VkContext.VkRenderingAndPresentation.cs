using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private VkSemaphore vkImageAvailableSemaphore;
    private VkSemaphore vkRenderFinishedSemaphore;
    private VkFence vkFence;
    private void CreateFrames()
    {
        CreateSemaphores();
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
    
    private void CreateSemaphores()
    {
        VkSemaphoreCreateInfo semaphoreInfo = new VkSemaphoreCreateInfo()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO,
        };

        fixed (VkSemaphore* imageAvailableSemaphorePtr = &vkImageAvailableSemaphore)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateSemaphore(vkDevice, &semaphoreInfo, null, imageAvailableSemaphorePtr));
        }

        fixed (VkSemaphore* renderFinishedSemaphorePtr = &vkRenderFinishedSemaphore)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateSemaphore(vkDevice, &semaphoreInfo, null, renderFinishedSemaphorePtr));
        }
    }

    public void DrawFrame()
    {
        // Acquiring and image from the swap chain
        uint imageIndex;
        VkHelper.CheckErrors(VulkanNative.vkAcquireNextImageKHR(vkDevice, vkSwapChain, ulong.MaxValue, vkImageAvailableSemaphore, 0, &imageIndex));
        VkSemaphore* waitSemaphores = stackalloc VkSemaphore[] { vkImageAvailableSemaphore };
        VkPipelineStageFlags* waitStages = stackalloc VkPipelineStageFlags[] { VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT };
        VkSemaphore* signalSemaphores = stackalloc VkSemaphore[] { vkRenderFinishedSemaphore };
        VkCommandBuffer* commandBuffersPtr = stackalloc VkCommandBuffer[] { vkCommandBuffers[imageIndex] };
        VkSubmitInfo submitInfo = new VkSubmitInfo()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO,
            waitSemaphoreCount = 1,
            pWaitSemaphores = waitSemaphores,
            pWaitDstStageMask = waitStages,
            commandBufferCount = 1,
            pCommandBuffers = commandBuffersPtr,
            signalSemaphoreCount = 1,
            pSignalSemaphores = signalSemaphores,
        };
        VkHelper.CheckErrors(VulkanNative.vkQueueSubmit(vkGraphicsQueue, 1, &submitInfo, 0));

        // Presentation
        VkSwapchainKHR* swapChains = stackalloc VkSwapchainKHR[] { vkSwapChain };
        VkPresentInfoKHR presentInfo = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PRESENT_INFO_KHR,
            waitSemaphoreCount = 1,
            pWaitSemaphores = signalSemaphores,
            swapchainCount = 1,
            pSwapchains = swapChains,
            pImageIndices = &imageIndex,
            pResults = null, // Optional
        };

        VkHelper.CheckErrors(VulkanNative.vkQueuePresentKHR(vkPresentQueue, &presentInfo));

        VkHelper.CheckErrors(VulkanNative.vkQueueWaitIdle(vkPresentQueue));
    }
}
