using WaveEngine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary;

public interface IVkSurface
{
    void CreateSurface(VkInstance vkInstance);
    VkSurfaceKHR SurfaceKHR { get; }
}
