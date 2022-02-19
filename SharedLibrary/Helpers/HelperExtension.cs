namespace SharedLibrary.Helpers;

public static unsafe class HelperExtension
{
    public static byte* ToPointer(this string text)
    {
        return (byte*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(text);
    }
}