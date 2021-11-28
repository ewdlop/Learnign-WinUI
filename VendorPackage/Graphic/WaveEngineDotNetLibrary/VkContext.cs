using Serilog;
using SharedLibrary.Helpers;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary;

public unsafe partial class VkContext
{
    private readonly VkInstance VkInstance;
    private ImmutableArray<string> VkValidationLayerNames { get; init; } = ImmutableArray.Create(new string[] 
    { 
        "VK_LAYER_KHRONOS_validation" 
    });
    private ImmutableArray<string> VkExtensionNames { get; init; } = ImmutableArray.Create(new string[] 
    {
        "VK_KHR_surface",
        "VK_KHR_win32_surface",
        "VK_EXT_debug_utils",
    });

    public void CreateInstance()
    {
        VkApplicationInfo vkApplicationInfo = new VkApplicationInfo()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
            pApplicationName = "Hello Triangle".ToPointer(),
            applicationVersion = Helper.Version(1,0,0),
            pEngineName = "No Engine".ToPointer(),
            engineVersion = Helper.Version(1, 0, 0),
            apiVersion = Helper.Version(1, 2, 0)
        };

        VkInstanceCreateInfo createInfo = default;
        createInfo.sType = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO;
        createInfo.pApplicationInfo = &vkApplicationInfo;

        // Extensions
        GetAllInstanceExtensionsAvailables();

        IntPtr* extensionsToBytesArray = stackalloc IntPtr[VkExtensionNames.Length];
        for (int i = 0; i < VkExtensionNames.Length; i++)
        {
            extensionsToBytesArray[i] = Marshal.StringToHGlobalAnsi(VkExtensionNames[i]);
        }
        createInfo.enabledExtensionCount = (uint)VkExtensionNames.Length;
        createInfo.ppEnabledExtensionNames = (byte**)extensionsToBytesArray;

        // Validation layers
        #if DEBUG
        if (CheckValidationLayerSupport())
        {
            IntPtr* layersToBytesArray = stackalloc IntPtr[VkValidationLayerNames.Length];
            for (int i = 0; i < VkValidationLayerNames.Length; i++)
            {
                layersToBytesArray[i] = Marshal.StringToHGlobalAnsi(VkValidationLayerNames[i]);
            }

            createInfo.enabledLayerCount = (uint)VkValidationLayerNames.Length;
            createInfo.ppEnabledLayerNames = (byte**)layersToBytesArray;
        }
        else
        {
            createInfo.enabledLayerCount = 0;
            createInfo.pNext = null;
        }
        #else
            createInfo.enabledLayerCount = 0;
            createInfo.pNext = null;
        #endif
        fixed (VkInstance* instancePtr = &VkInstance)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateInstance(&createInfo, null, instancePtr));
        }
    }

    private void GetAllInstanceExtensionsAvailables()
    {
        uint extensionCount;
        VkHelper.CheckErrors(VulkanNative.vkEnumerateInstanceExtensionProperties(null, &extensionCount, null));
        VkExtensionProperties* extensions = stackalloc VkExtensionProperties[(int)extensionCount];
        VkHelper.CheckErrors(VulkanNative.vkEnumerateInstanceExtensionProperties(null, &extensionCount, extensions));

        for (int i = 0; i < extensionCount; i++)
        {
            Log.Information($"Extension: {Helper.GetString(extensions[i].extensionName)} version: {extensions[i].specVersion}");
        }

        // Return
        //Extension: VK_KHR_device_group_creation version: 1
        //Extension: VK_KHR_external_fence_capabilities version: 1
        //Extension: VK_KHR_external_memory_capabilities version: 1
        //Extension: VK_KHR_external_semaphore_capabilities version: 1
        //Extension: VK_KHR_get_physical_device_properties2 version: 2
        //Extension: VK_KHR_get_surface_capabilities2 version: 1
        //Extension: VK_KHR_surface version: 25
        //Extension: VK_KHR_surface_protected_capabilities version: 1
        //Extension: VK_KHR_win32_surface version: 6
        //Extension: VK_EXT_debug_report version: 9
        //Extension: VK_EXT_debug_utils version: 2
        //Extension: VK_EXT_swapchain_colorspace version: 4
        //Extension: VK_NV_external_memory_capabilities version: 1
    }
}
