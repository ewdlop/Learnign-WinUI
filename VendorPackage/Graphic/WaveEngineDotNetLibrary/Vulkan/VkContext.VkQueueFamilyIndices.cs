namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    protected ref struct QueueFamilyIndices
    {
        public uint? graphicsFamily;
        public uint? presentFamily;

        public readonly bool IsComplete() => graphicsFamily.HasValue && presentFamily.HasValue;
    }
}
