﻿using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary;

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
        VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfaceCapabilitiesKHR(vkPhysicalDevice, vkSurfaceKHR, &details.surfaceCapabilities));

        // Formats
        uint surfaceFormatCount;
        VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfaceFormatsKHR(vkPhysicalDevice, vkSurfaceKHR, &surfaceFormatCount, null));

        if (surfaceFormatCount != 0)
        {
            details.surfaceFormats = new VkSurfaceFormatKHR[surfaceFormatCount];
            fixed (VkSurfaceFormatKHR* surfaceformatsPtr = &details.surfaceFormats[0])
            {
                VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfaceFormatsKHR(vkPhysicalDevice, vkSurfaceKHR, &surfaceFormatCount, surfaceformatsPtr));
            }
        }

        // Present Modes
        uint presentModeCount;
        VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfacePresentModesKHR(vkPhysicalDevice, vkSurfaceKHR, &presentModeCount, null));

        if (presentModeCount != 0)
        {
            details.presentModes = new VkPresentModeKHR[presentModeCount];
            fixed (VkPresentModeKHR* presentModesPtr = &details.presentModes[0])
            {
                VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfacePresentModesKHR(vkPhysicalDevice, vkSurfaceKHR, &presentModeCount, presentModesPtr));
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
        VkSwapChainSupportDetails swapChainSupport = QuerySwapChainSupport(vkphysicalDevice);
        VkSurfaceFormatKHR surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport.surfaceFormats);
        VkPresentModeKHR presentMode = ChooseSwapPresentMode(swapChainSupport.presentModes);
        VkExtent2D extent = this.ChooseSwapExtent(swapChainSupport.surfaceCapabilities);

        uint imageCount = swapChainSupport.surfaceCapabilities.minImageCount + 1;
        if (swapChainSupport.surfaceCapabilities.maxImageCount > 0 && imageCount > swapChainSupport.surfaceCapabilities.maxImageCount)
        {
            imageCount = swapChainSupport.surfaceCapabilities.maxImageCount;
        }

        VkSwapchainCreateInfoKHR createInfo = new VkSwapchainCreateInfoKHR();
        createInfo.sType = VkStructureType.VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR;
        createInfo.surface = vkSurfaceKHR;
        createInfo.minImageCount = imageCount;
        createInfo.imageFormat = surfaceFormat.format;
        createInfo.imageColorSpace = surfaceFormat.colorSpace;
        createInfo.imageExtent = extent;
        createInfo.imageArrayLayers = 1;
        createInfo.imageUsage = VkImageUsageFlags.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT;

        QueueFamilyIndices indices = FindQueueFamilies(vkphysicalDevice);
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
            VkHelper.CheckErrors(VulkanNative.vkCreateSwapchainKHR(VkDevice, &createInfo, null, swapChainPtr));
        }

        // SwapChain Images
        VulkanNative.vkGetSwapchainImagesKHR(VkDevice, vkSwapChain, &imageCount, null);
        vkSwapChainImages = new VkImage[imageCount];
        fixed (VkImage* swapChainImagesPtr = &vkSwapChainImages[0])
        {
            VulkanNative.vkGetSwapchainImagesKHR(VkDevice, vkSwapChain, &imageCount, swapChainImagesPtr);
        }

        vkSwapChainImageFormat = surfaceFormat.format;
        vkSwapChainExtent = extent;
    }
}
