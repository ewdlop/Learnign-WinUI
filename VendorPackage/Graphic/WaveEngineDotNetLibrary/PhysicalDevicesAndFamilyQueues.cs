using SharedLibrary.Helpers;
using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary;

public unsafe partial class VkContext
{
    private VkPhysicalDevice physicalDevice;

    private void PickPhysicalDevice()
    {
        uint deviceCount = 0;
        VkHelper.CheckErrors(VulkanNative.vkEnumeratePhysicalDevices(VkInstance, &deviceCount, null));
        if (deviceCount == 0)
        {
            throw new Exception("Failed to find GPUs with Vulkan support!");
        }

        VkPhysicalDevice* devices = stackalloc VkPhysicalDevice[(int)deviceCount];
        VkHelper.CheckErrors(VulkanNative.vkEnumeratePhysicalDevices(VkInstance, &deviceCount, devices));

        for (int i = 0; i < deviceCount; i++)
        {
            var device = devices[i];
            if (IsPhysicalDeviceSuitable(device))
            {
                physicalDevice = device;
                break;
            }
        }

        if (physicalDevice == default)
        {
            throw new Exception("failed to find a suitable GPU!");
        }
    }

    private bool IsPhysicalDeviceSuitable(VkPhysicalDevice physicalDevice)
    {
        QueueFamilyIndices indices = FindQueueFamilies(physicalDevice);

        bool extensionsSupported = CheckPhysicalDeviceExtensionSupport(physicalDevice);

        bool swapChainAdequate = false;
        //if (extensionsSupported)
        //{
        //    SwapChainSupportDetails swapChainSupport = QuerySwapChainSupport(physicalDevice);
        //    swapChainAdequate = (swapChainSupport.formats.Length != 0 && swapChainSupport.presentModes.Length != 0);
        //}

        return indices.IsComplete() && extensionsSupported && swapChainAdequate;
    }

    private bool CheckPhysicalDeviceExtensionSupport(VkPhysicalDevice physicalDevice)
    {
        uint extensionCount;
        VkHelper.CheckErrors(VulkanNative.vkEnumerateDeviceExtensionProperties(physicalDevice, null, &extensionCount, null));

        VkExtensionProperties* availableExtensions = stackalloc VkExtensionProperties[(int)extensionCount];
        VkHelper.CheckErrors(VulkanNative.vkEnumerateDeviceExtensionProperties(physicalDevice, null, &extensionCount, availableExtensions));

        HashSet<string> requiredExtensions = new (VkDeviceExtensionNames);

        for (int i = 0; i < extensionCount; i++)
        {
            var extension = availableExtensions[i];
            requiredExtensions.Remove(Helper.GetString(extension.extensionName));
        }

        return requiredExtensions.Count == 0;
    }

    private QueueFamilyIndices FindQueueFamilies(VkPhysicalDevice physicalDevice)
    {
        QueueFamilyIndices queueFamilyIndices = default;

        uint queueFamilyCount = 0;
        VulkanNative.vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, null);

        VkQueueFamilyProperties* queueFamilies = stackalloc VkQueueFamilyProperties[(int)queueFamilyCount];
        VulkanNative.vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, queueFamilies);

        for (uint i = 0; i < queueFamilyCount; i++)
        {
            var queueFamily = queueFamilies[i];
            if ((queueFamily.queueFlags & VkQueueFlags.VK_QUEUE_GRAPHICS_BIT) != 0)
            {
                queueFamilyIndices.graphicsFamily = i;
            }

            VkBool32 presentSupport = false;
            //VkHelper.CheckErrors(VulkanNative.vkGetPhysicalDeviceSurfaceSupportKHR(physicalDevice, i, surface, &presentSupport));

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

    private bool IsDeviceSuitable(VkPhysicalDevice physicalDevice)
    {
        QueueFamilyIndices indices = FindQueueFamilies(physicalDevice);

        return indices.IsComplete();
    }
}
