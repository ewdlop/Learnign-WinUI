namespace WaveEngineDotNetLibrary.Window;

public unsafe struct VkWindowHandle
{
    public IntPtr hwnd { get; }
    public IntPtr hinstance { get; }

    public VkWindowHandle(IntPtr hwnd, IntPtr hinstance) : this()
    {
        this.hinstance = hinstance;
        this.hwnd = hwnd;
    }
}
