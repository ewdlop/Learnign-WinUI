using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private VkSwapchainKHR vkSwapChain;
    private VkImage[] vkSwapChainImages;
    private VkFormat vkSwapChainImageFormat;
    private VkExtent2D vkSwapChainExtent;

    private VkSwapChainSupportDetails QuerySwapChainSupport(VkPhysicalDevice vkPhysicalDevice)
    {
        VkSwapChainSupportDetails details = default;

        // Capabilities
        VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfaceCapabilitiesKHR(vkPhysicalDevice, _vkSurface.SurfaceKHR, &details.surfaceCapabilities));

        // Formats
        uint surfaceFormatCount;
        VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfaceFormatsKHR(vkPhysicalDevice, _vkSurface.SurfaceKHR, &surfaceFormatCount, null));

        if (surfaceFormatCount != 0)
        {
            details.surfaceFormats = new VkSurfaceFormatKHR[surfaceFormatCount];
            fixed (VkSurfaceFormatKHR* surfaceFormatsPtr = &details.surfaceFormats[0])
            {
                VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfaceFormatsKHR(vkPhysicalDevice, _vkSurface.SurfaceKHR, &surfaceFormatCount, surfaceFormatsPtr));
            }
        }

        // Present Modes
        uint presentModeCount;
        VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfacePresentModesKHR(vkPhysicalDevice, _vkSurface.SurfaceKHR, &presentModeCount, null));

        if (presentModeCount != 0)
        {
            details.presentModes = new VkPresentModeKHR[presentModeCount];
            fixed (VkPresentModeKHR* presentModesPtr = &details.presentModes[0])
            {
                VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfacePresentModesKHR(vkPhysicalDevice, _vkSurface.SurfaceKHR, &presentModeCount, presentModesPtr));
            }
        }

        return details;
    }

    private static VkSurfaceFormatKHR ChooseSwapSurfaceFormat(VkSurfaceFormatKHR[] availableFormats)
    {
        foreach (var availableFormat in availableFormats)
        {
            if (availableFormat.format == VkFormat.VK_FORMAT_B8G8R8A8_SRGB && availableFormat.colorSpace == VkColorSpaceKHR.VK_COLOR_SPACE_SRGB_NONLINEAR_KHR)
            {
                return availableFormat;
            }
        }

        return availableFormats[0];
    }

    private VkPresentModeKHR ChooseSwapPresentMode(VkPresentModeKHR[] availablePresentModes)
    {
        foreach (var availablePresentMode in availablePresentModes)
        {
            if (availablePresentMode == VkPresentModeKHR.VK_PRESENT_MODE_MAILBOX_KHR)
            {
                return availablePresentMode;
            }
        }

        return VkPresentModeKHR.VK_PRESENT_MODE_FIFO_KHR;
    }

    VkExtent2D ChooseSwapExtent(VkSurfaceCapabilitiesKHR capabilities)
    {
        if (capabilities.currentExtent.width != uint.MaxValue)
        {
            return capabilities.currentExtent;
        }
        else
        {
            VkExtent2D actualExtent = new VkExtent2D(_width, _height);

            actualExtent.width = Math.Max(capabilities.minImageExtent.width, Math.Min(capabilities.maxImageExtent.width, actualExtent.width));
            actualExtent.height = Math.Max(capabilities.minImageExtent.height, Math.Min(capabilities.maxImageExtent.height, actualExtent.height));

            return actualExtent;
        }
    }

    private void CreateSwapChain()
    {
        // Create SwapChain
        VkSwapChainSupportDetails swapChainSupport = QuerySwapChainSupport(vkPhysicalDevice);
        VkSurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.surfaceFormats);
        VkPresentModeKHR presentMode = ChooseSwapPresentMode(swapChainSupport.presentModes);
        VkExtent2D extent = this.ChooseSwapExtent(swapChainSupport.surfaceCapabilities);

        uint imageCount = swapChainSupport.surfaceCapabilities.minImageCount + 1;
        if (swapChainSupport.surfaceCapabilities.maxImageCount > 0 && imageCount > swapChainSupport.surfaceCapabilities.maxImageCount)
        {
            imageCount = swapChainSupport.surfaceCapabilities.maxImageCount;
        }

        VkSwapchainCreateInfoKHR createInfo = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
            pNext = null,
            flags = VkSwapchainCreateFlagsKHR.None,
            surface = default,
            minImageCount = 0,
            imageFormat = VkFormat.VK_FORMAT_UNDEFINED,
            imageColorSpace = VkColorSpaceKHR.VK_COLOR_SPACE_SRGB_NONLINEAR_KHR,
            imageExtent = default,
            imageArrayLayers = 0,
            imageUsage = VkImageUsageFlags.None,
            imageSharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE,
            queueFamilyIndexCount = 0,
            pQueueFamilyIndices = null,
            preTransform = VkSurfaceTransformFlagsKHR.None,
            compositeAlpha = VkCompositeAlphaFlagsKHR.None,
            presentMode = VkPresentModeKHR.VK_PRESENT_MODE_IMMEDIATE_KHR,
            clipped = default,
            oldSwapchain = default
        };
        createInfo.sType = VkStructureType.VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR;
        createInfo.surface = _vkSurface.SurfaceKHR;
        createInfo.minImageCount = imageCount;
        createInfo.imageFormat = surfaceFormat.format;
        createInfo.imageColorSpace = surfaceFormat.colorSpace;
        createInfo.imageExtent = extent;
        createInfo.imageArrayLayers = 1;
        createInfo.imageUsage = VkImageUsageFlags.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT;

        QueueFamilyIndices indices = FindQueueFamilies(vkPhysicalDevice);
        uint* queueFamilyIndices = stackalloc uint[] { indices.graphicsFamily.Value, indices.presentFamily.Value };

        if (indices.graphicsFamily != indices.presentFamily)
        {
            createInfo.imageSharingMode = VkSharingMode.VK_SHARING_MODE_CONCURRENT;
            createInfo.queueFamilyIndexCount = 2;
            createInfo.pQueueFamilyIndices = queueFamilyIndices;
        }
        else
        {
            createInfo.imageSharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE;
            createInfo.queueFamilyIndexCount = 0; //Optional
            createInfo.pQueueFamilyIndices = null; //Optional
        }

        createInfo.preTransform = swapChainSupport.surfaceCapabilities.currentTransform;
        createInfo.compositeAlpha = VkCompositeAlphaFlagsKHR.VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR;
        createInfo.presentMode = presentMode;
        createInfo.clipped = true;
        createInfo.oldSwapchain = 0;

        fixed (VkSwapchainKHR* swapChainPtr = &vkSwapChain)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateSwapchainKHR(vkDevice, &createInfo, null, swapChainPtr));
        }

        // SwapChain Images
        VulkanNative.vkGetSwapchainImagesKHR(vkDevice, vkSwapChain, &imageCount, null);
        vkSwapChainImages = new VkImage[imageCount];
        fixed (VkImage* swapChainImagesPtr = &vkSwapChainImages[0])
        {
            VulkanNative.vkGetSwapchainImagesKHR(vkDevice, vkSwapChain, &imageCount, swapChainImagesPtr);
        }

        vkSwapChainImageFormat = surfaceFormat.format;
        vkSwapChainExtent = extent;
    }
}
