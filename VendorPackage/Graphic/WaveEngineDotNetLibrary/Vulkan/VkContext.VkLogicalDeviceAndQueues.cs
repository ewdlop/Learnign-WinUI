using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Evergine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private VkDevice vkDevice;
    private VkQueue vkGraphicsQueue;
    private VkQueue vkPresentQueue;

    private ImmutableArray<string> VkDeviceExtensionNames { get; } =
    [
        "VK_KHR_swapchain"
    ];

    private void CreateLogicalDevice()
    {
        float queuePriority = 1.0f;

        QueueFamilyIndices queueFamilyIndices = FindQueueFamilies(vkPhysicalDevice);

        if(!queueFamilyIndices.graphicsFamily.HasValue || !queueFamilyIndices.presentFamily.HasValue)
        {
            throw new InvalidOperationException("Queue family indices are not complete");
        }

        Span<VkDeviceQueueCreateInfo> queueCreateInfos = stackalloc VkDeviceQueueCreateInfo[2];

        uint queueCreateInfoCount = 0;

        if (queueFamilyIndices.graphicsFamily.Value == queueFamilyIndices.presentFamily.Value)
        {
            queueCreateInfos[0] = new VkDeviceQueueCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
                queueFamilyIndex = queueFamilyIndices.graphicsFamily.Value,
                queueCount = 1,
                pQueuePriorities = &queuePriority,
            };
            queueCreateInfoCount = 1;
        }
        else
        {
            queueCreateInfos[0] = new VkDeviceQueueCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
                queueFamilyIndex = queueFamilyIndices.graphicsFamily.Value,
                queueCount = 1,
                pQueuePriorities = &queuePriority,
            };
            queueCreateInfos[1] = new VkDeviceQueueCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
                queueFamilyIndex = queueFamilyIndices.presentFamily.Value,
                queueCount = 1,
                pQueuePriorities = &queuePriority,
            };
            queueCreateInfoCount = 2;
        }

        VkPhysicalDeviceFeatures deviceFeatures = default;

        int deviceExtensionsCount = VkDeviceExtensionNames.Length;
        IntPtr* deviceExtensionsArray = stackalloc IntPtr[deviceExtensionsCount];
        for (int i = 0; i < deviceExtensionsCount; i++)
        {
            string extension = VkDeviceExtensionNames[i];
            deviceExtensionsArray[i] = Marshal.StringToHGlobalAnsi(extension);
        }

        VkDeviceCreateInfo createInfo = new VkDeviceCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
            pEnabledFeatures = &deviceFeatures,
            enabledExtensionCount = (uint)VkDeviceExtensionNames.Length,
            ppEnabledExtensionNames = (byte**)deviceExtensionsArray
        };

        fixed (VkDeviceQueueCreateInfo* queueCreateInfosArrayPtr = queueCreateInfos)
        {
            createInfo.queueCreateInfoCount = queueCreateInfoCount;
            createInfo.pQueueCreateInfos = queueCreateInfosArrayPtr;
        }

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
