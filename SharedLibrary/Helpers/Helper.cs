namespace SharedLibrary.Helpers;

public static unsafe class Helper
{
    public static uint IsValidVersion(uint major, uint minor, uint patch)
    {
        return major << 22 | minor << 12 | patch;
    }

    public static string GetString(byte* stringStart)
    {
        int characters = 0;
        while (stringStart[characters] != 0)
        {
            characters++;
        }

        return System.Text.Encoding.UTF8.GetString(stringStart, characters);
    }
}
