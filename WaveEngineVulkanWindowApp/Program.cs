using System.Diagnostics;
using WaveEngineVulkanWindowApp;

App app = new App();
try
{
    app.Run();
}
catch (Exception ex)
{
    Debug.WriteLine(ex.StackTrace);
}