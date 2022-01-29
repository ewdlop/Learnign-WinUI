﻿using System.Collections.Immutable;
using System.Runtime.InteropServices;
using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private VkDevice vkDevice;
    private VkQueue vkGraphicsQueue;
    private VkQueue vkPresentQueue;

    private ImmutableArray<string> VkDeviceExtensionNames { get; init; } = ImmutableArray.Create(new string[]
    {
        "VK_KHR_swapchain"
    });

    private void CreateLogicalDevice()
    {
        float queuePriority = 1.0f;

        QueueFamilyIndices queueFamilyIndices = FindQueueFamilies(vkPhysicalDevice);
        VkDeviceQueueCreateInfo[] queueCreateInfos = new VkDeviceQueueCreateInfo[]
        {
            new VkDeviceQueueCreateInfo()
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
                queueFamilyIndex = queueFamilyIndices.graphicsFamily.Value,
                queueCount = 1,
                pQueuePriorities = &queuePriority,
            },
            new VkDeviceQueueCreateInfo()
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
                queueFamilyIndex = queueFamilyIndices.presentFamily.Value,
                queueCount = 1,
                pQueuePriorities = &queuePriority,
            }
        };

        VkPhysicalDeviceFeatures deviceFeatures = default;

        int deviceExtensionsCount = VkDeviceExtensionNames.Length;
        IntPtr* deviceExtensionsArray = stackalloc IntPtr[deviceExtensionsCount];
        for (int i = 0; i < deviceExtensionsCount; i++)
        {
            string extension = VkDeviceExtensionNames[i];
            deviceExtensionsArray[i] = Marshal.StringToHGlobalAnsi(extension);
        }

        VkDeviceCreateInfo createInfo = new VkDeviceCreateInfo();
        createInfo.sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO;

        fixed (VkDeviceQueueCreateInfo* queueCreateInfosArrayPtr = &queueCreateInfos[0])
        {
            createInfo.queueCreateInfoCount = (uint)queueCreateInfos.Length;
            createInfo.pQueueCreateInfos = queueCreateInfosArrayPtr;
        }

        createInfo.pEnabledFeatures = &deviceFeatures;
        createInfo.enabledExtensionCount = (uint)VkDeviceExtensionNames.Length;
        createInfo.ppEnabledExtensionNames = (byte**)deviceExtensionsArray;

        fixed (VkDevice* devicePtr = &vkDevice)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateDevice(vkPhysicalDevice, &createInfo, null, devicePtr));
        }

        fixed (VkQueue* graphicsQueuePtr = &vkGraphicsQueue)
        {
            VulkanNative.vkGetDeviceQueue(vkDevice, queueFamilyIndices.graphicsFamily.Value, 0, graphicsQueuePtr);
        }

        fixed (VkQueue* presentQueuePtr = &vkPresentQueue)
        {
            VulkanNative.vkGetDeviceQueue(vkDevice, queueFamilyIndices.presentFamily.Value, 0, presentQueuePtr); // TODO queue index 0 ?¿?¿
        }
    }
}