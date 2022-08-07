namespace WaveEngineDotNetLibrary.Window;

public unsafe struct WindowHandle
{
    public IntPtr hwnd { get; }
    public IntPtr hinstance { get; }

    public WindowHandle(IntPtr hwnd, IntPtr hinstance) : this()
    {
        this.hinstance = hinstance;
        this.hwnd = hwnd;
    }
}
