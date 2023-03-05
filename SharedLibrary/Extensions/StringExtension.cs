namespace SharedLibrary.Extensions;

public static unsafe class StringExtension
{
    public static byte* ToPointer(this string text)
    {
        return (byte*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(text);
    }
}