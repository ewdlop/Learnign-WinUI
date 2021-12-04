namespace WaveEngineDotNetLibrary
{
    public unsafe partial class VkContext
    {
        public ref struct QueueFamilyIndices
        {
            public uint? graphicsFamily;
            public uint? presentFamily;

            public bool IsComplete()
            {
                return graphicsFamily.HasValue && presentFamily.HasValue;
            }
        }
    }
}
