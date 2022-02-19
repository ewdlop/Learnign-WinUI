using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public static class VkHelper
{
    //[Conditional("DEBUG")]
    public static void CheckErrors(VkResult result)
    {
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException(result.ToString());
        }
    }
}