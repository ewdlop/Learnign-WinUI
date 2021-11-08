using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Maths;
using SilkDotNetLibrary.OpenGL.Services;

namespace CoreLibrary.Services
{
    public static class VeryMiniEngineService
    {
        public static IServiceCollection UseVeryMiniEngine(this IServiceCollection services)
        {
            return services.UseSilkDotNetOpenGLWindow(options =>
            {
                options.Title = "LearnOpenGL with Silk.NET";
                options.Size = new Vector2D<int>(800, 600);
            });
        }
    }
}
