﻿using Serilog;
using SharedLibrary.Helpers;
using System.Runtime.InteropServices;
using Evergine.Bindings.Vulkan;
using SharedLibrary.Extensions;
using BenchmarkDotNet.Portability;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    protected delegate VkBool32 DebugCallbackDelegate(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
                                                    VkDebugUtilsMessageTypeFlagsEXT messageType,
                                                    VkDebugUtilsMessengerCallbackDataEXT pCallbackData,
                                                    void* pUserData);
    protected static readonly DebugCallbackDelegate debugCallbackDelegate = new DebugCallbackDelegate(DebugCallback);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    protected delegate VkResult vkCreateDebugUtilsMessengerEXTDelegate(VkInstance instance,
                                                                     VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo,
                                                                     VkAllocationCallbacks* pAllocator,
                                                                     VkDebugUtilsMessengerEXT* pMessenger);
    protected static vkCreateDebugUtilsMessengerEXTDelegate? vkCreateDebugUtilsMessengerEXT_ptr;
    protected static VkResult vkCreateDebugUtilsMessengerEXT(VkInstance instance,
                                                          VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo,
                                                          VkAllocationCallbacks* pAllocator,
                                                          VkDebugUtilsMessengerEXT* pMessenger)
      => vkCreateDebugUtilsMessengerEXT_ptr?.Invoke(instance, pCreateInfo, pAllocator, pMessenger) ?? throw new InvalidOperationException("vkCreateDebugUtilsMessengerEXT_ptr is null");

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    protected delegate void vkDestroyDebugUtilsMessengerEXTDelegate(VkInstance instance,
                                                                  VkDebugUtilsMessengerEXT messenger,
                                                                  VkAllocationCallbacks* pAllocator);
    protected static vkDestroyDebugUtilsMessengerEXTDelegate? vkDestroyDebugUtilsMessengerEXT_ptr;
    protected static void vkDestroyDebugUtilsMessengerEXT(VkInstance instance, VkDebugUtilsMessengerEXT messenger, VkAllocationCallbacks* pAllocator)
        => vkDestroyDebugUtilsMessengerEXT_ptr?.Invoke(instance, messenger, pAllocator);


    protected readonly VkDebugUtilsMessengerEXT vkDebugMessenger;

    public virtual bool CheckValidationLayerSupport()
    {
        uint layerCount;
        VkHelper.CheckErrors(VulkanNative.vkEnumerateInstanceLayerProperties(&layerCount, null));
        VkLayerProperties* availableLayers = stackalloc VkLayerProperties[(int)layerCount];
        VkHelper.CheckErrors(VulkanNative.vkEnumerateInstanceLayerProperties(&layerCount, availableLayers));

        for (int i = 0; i < layerCount; i++)
        {
            Log.Debug($"ValidationLayer: {Helper.GetString(availableLayers[i].layerName)} version: {availableLayers[i].specVersion} description: {Helper.GetString(availableLayers[i].description)}");
        }

        //Return
        //ValidationLayer: VK_LAYER_NV_optimus version: 4202634 description: NVIDIA Optimus layer
        //ValidationLayer: VK_LAYER_RENDERDOC_Capture version: 4202627 description: Debugging capture layer for RenderDoc
        //ValidationLayer: VK_LAYER_VALVE_steam_overlay version: 4198473 description: Steam Overlay Layer
        //ValidationLayer: VK_LAYER_VALVE_steam_fossilize version: 4198473 description: Steam Pipeline Caching Layer
        //ValidationLayer: VK_LAYER_NV_nomad_release_public_2020_2_0 version: 4202627 description: NVIDIA Nsight Graphics interception layer
        //ValidationLayer: VK_LAYER_NV_GPU_Trace_release_public_2020_2_0 version: 4202627 description: NVIDIA Nsight Graphics GPU Trace interception layer
        //ValidationLayer: VK_LAYER_EOS_Overlay version: 4198473 description: Vulkan overlay layer for Epic Online Services
        //ValidationLayer: VK_LAYER_EOS_Overlay version: 4198473 description: Vulkan overlay layer for Epic Online Services
        //ValidationLayer: VK_LAYER_LUNARG_api_dump version: 4202631 description: LunarG API dump layer
        //ValidationLayer: VK_LAYER_LUNARG_device_simulation version: 4202631 description: LunarG device simulation layer
        //ValidationLayer: VK_LAYER_KHRONOS_validation version: 4202631 description: Khronos Validation Layer
        //ValidationLayer: VK_LAYER_LUNARG_monitor version: 4202631 description: Execution Monitoring Layer
        //ValidationLayer: VK_LAYER_LUNARG_screenshot version: 4202631 description: LunarG image capture layer
        //ValidationLayer: VK_LAYER_LUNARG_vktrace version: 4202631 description: Vktrace tracing library

        for (int i = 0; i < VkValidationLayerNames.Length; i++)
        {
            bool layerFound = false;
            string validationLayer = VkValidationLayerNames[i];
            for (int j = 0; j < layerCount; j++)
            {
                if (validationLayer.Equals(Helper.GetString(availableLayers[j].layerName)))
                {
                    layerFound = true;
                    break;
                }
            }

            if (!layerFound)
            {
                return false;
            }
        }

        return true;
    }

    public virtual void SetupDebugMessenger()
    {
        #if DEBUG
        fixed (VkDebugUtilsMessengerEXT* debugMessengerPtr = &vkDebugMessenger)
        {
            var funcPtr = VulkanNative.vkGetInstanceProcAddr(vkInstance, "vkCreateDebugUtilsMessengerEXT".ToPointer());
            if (funcPtr != IntPtr.Zero)
            {
                vkCreateDebugUtilsMessengerEXT_ptr = Marshal.GetDelegateForFunctionPointer<vkCreateDebugUtilsMessengerEXTDelegate>(funcPtr);

                PopulateDebugMessengerCreateInfo(out VkDebugUtilsMessengerCreateInfoEXT createInfo);
                VkHelper.CheckErrors(vkCreateDebugUtilsMessengerEXT(vkInstance, &createInfo, null, debugMessengerPtr));
            }
        }
        #endif
    }

    public static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageType, VkDebugUtilsMessengerCallbackDataEXT pCallbackData, void* pUserData)
    {
        Log.Debug($"<<Vulkan Validation Layer>> {Helper.GetString(pCallbackData.pMessage)}");
        return false;
    }

    protected static void PopulateDebugMessengerCreateInfo(out VkDebugUtilsMessengerCreateInfoEXT createInfo)
    {
        createInfo = default;
        createInfo.sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT;
        createInfo.messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT
                                     | VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT
                                     | VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT;
        createInfo.messageType = VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT
                                 | VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT
                                 | VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT;
        createInfo.pfnUserCallback = Marshal.GetFunctionPointerForDelegate(debugCallbackDelegate);
        createInfo.pUserData = null;
    }

    public virtual void DestroyDebugMessenger()
    {
        #if DEBUG
        var funcPtr = VulkanNative.vkGetInstanceProcAddr(vkInstance, "vkDestroyDebugUtilsMessengerEXT".ToPointer());
        if (funcPtr != IntPtr.Zero)
        {
            vkDestroyDebugUtilsMessengerEXT_ptr = Marshal.GetDelegateForFunctionPointer<vkDestroyDebugUtilsMessengerEXTDelegate>(funcPtr);
            vkDestroyDebugUtilsMessengerEXT(vkInstance, vkDebugMessenger, null);
        }
        #endif
    }
}
