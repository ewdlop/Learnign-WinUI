namespace WaveEngineDotNetLibrary.Window;

public unsafe record struct WindowHandle(IntPtr hwnd, IntPtr hinstance);