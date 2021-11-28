namespace SharedLibrary.Helpers;

public unsafe static class Helper
{
    public static uint Version(uint major, uint minor, uint patch)
    {
        return major << 22 | minor << 12 | patch;
    }

    public static unsafe string GetString(byte* stringStart)
    {
        int characters = 0;
        while (stringStart[characters] != 0)
        {
            characters++;
        }

        return System.Text.Encoding.UTF8.GetString(stringStart, characters);
    }
}
