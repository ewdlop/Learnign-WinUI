﻿using SharedLibrary.Helpers;
using Evergine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    protected VkPhysicalDevice vkPhysicalDevice; //strange
    protected void PickPhysicalDevice()
    {
        uint deviceCount = 0;
        VkHelper.CheckErrors(VulkanNative.vkEnumeratePhysicalDevices(vkInstance, &deviceCount, null));
        if (deviceCount == 0)
        {
            throw new Exception("Failed to find GPUs with Vulkan support!");
        }

        VkPhysicalDevice* devices = stackalloc VkPhysicalDevice[(int)deviceCount];
        VkHelper.CheckErrors(VulkanNative.vkEnumeratePhysicalDevices(vkInstance, &deviceCount, devices));

        for (int i = 0; i < deviceCount; i++)
        {
            var device = devices[i];
            if (IsPhysicalDeviceSuitable(device))
            {
                vkPhysicalDevice = device;
                break;
            }
        }

        if (vkPhysicalDevice == default)
        {
            throw new Exception("failed to find a suitable GPU!");
        }
    }

    protected bool IsPhysicalDeviceSuitable(VkPhysicalDevice vkPhysicalDevice)
    {
        QueueFamilyIndices indices = FindQueueFamilies(vkPhysicalDevice);

        bool extensionsSupported = CheckPhysicalDeviceExtensionSupport(vkPhysicalDevice);

        bool swapChainAdequate = false;
        if (extensionsSupported)
        {
            VkSwapChainSupportDetails swapChainSupport = QuerySwapChainSupport(vkPhysicalDevice);
            swapChainAdequate = (swapChainSupport.surfaceFormats.Length != 0 && swapChainSupport.presentModes.Length != 0);
            #if DEBUG
            for (int i = 0; i < swapChainSupport.presentModes.Length; i++)
            {
                Console.WriteLine(swapChainSupport.presentModes[i]);
            }
            #endif
        }
        return indices.IsComplete() && extensionsSupported && swapChainAdequate;
    }

    protected bool CheckPhysicalDeviceExtensionSupport(VkPhysicalDevice vkPhysicalDevice)
    {
        uint extensionCount;
        VkHelper.CheckErrors(VulkanNative.vkEnumerateDeviceExtensionProperties(vkPhysicalDevice, null, &extensionCount, null));

        VkExtensionProperties* availableExtensions = stackalloc VkExtensionProperties[(int)extensionCount];
        VkHelper.CheckErrors(VulkanNative.vkEnumerateDeviceExtensionProperties(vkPhysicalDevice, null, &extensionCount, availableExtensions));

        HashSet<string> requiredExtensions = new (VkDeviceExtensionNames);

        for (int i = 0; i < extensionCount; i++)
        {
            var extension = availableExtensions[i];
            requiredExtensions.Remove(Helper.GetString(extension.extensionName));
        }

        return requiredExtensions.Count == 0;
    }

    protected QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice vkPhysicalDevice)
    {
        QueueFamilyIndices queueFamilyIndices = default;

        uint queueFamilyCount = 0;
        VulkanNative.vkGetPhysicalDeviceQueueFamilyProperties(vkPhysicalDevice, &queueFamilyCount, null);

        VkQueueFamilyProperties* queueFamilies = stackalloc VkQueueFamilyProperties[(int)queueFamilyCount];
        VulkanNative.vkGetPhysicalDeviceQueueFamilyProperties(vkPhysicalDevice, &queueFamilyCount, queueFamilies);

        for (uint i = 0; i < queueFamilyCount; i++)
        {
            var queueFamily = queueFamilies[i];
            if ((queueFamily.queueFlags & VkQueueFlags.VK_QUEUE_GRAPHICS_BIT) != 0)
            {
                queueFamilyIndices.graphicsFamily = i;
            }
            VkBool32 presentSupport = false;
            VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfaceSupportKHR(vkPhysicalDevice, i, _vkSurface.SurfaceKHR, &presentSupport));
            if (presentSupport)
            {
                queueFamilyIndices.presentFamily = i;
            }

            if (queueFamilyIndices.IsComplete())
            {
                break;
            }
        }

        return queueFamilyIndices;
    }

    protected bool IsDeviceSuitable(VkPhysicalDevice vkPhysicalDevice)
    {
        QueueFamilyIndices indices = FindQueueFamilies(vkPhysicalDevice);

        return indices.IsComplete();
    }
}
