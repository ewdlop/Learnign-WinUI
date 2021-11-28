using Serilog;
using SharedLibrary.Helpers;
using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary;

public unsafe partial class VkContext
{
    public bool CheckValidationLayerSupport()
    {
        uint layerCount;
        VkHelper.CheckErrors(VulkanNative.vkEnumerateInstanceLayerProperties(&layerCount, null));
        VkLayerProperties* availableLayers = stackalloc VkLayerProperties[(int)layerCount];
        VkHelper.CheckErrors(VulkanNative.vkEnumerateInstanceLayerProperties(&layerCount, availableLayers));

        for (int i = 0; i < layerCount; i++)
        {
            Log.Information($"ValidationLayer: {Helper.GetString(availableLayers[i].layerName)} version: {availableLayers[i].specVersion} description: {Helper.GetString(availableLayers[i].description)}");
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
}
