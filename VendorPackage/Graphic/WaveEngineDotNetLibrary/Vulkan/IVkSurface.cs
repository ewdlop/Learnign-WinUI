using Evergine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary;

public interface IVkSurface
{
    abstract void CreateSurface(VkInstance vkInstance);
    VkSurfaceKHR SurfaceKHR { get; }
}
