using Microsoft.Extensions.Hosting;
using System;

namespace SilkDotNetLibrary.OpenGL.Apps;

public interface IHostedApp : IHostedService, IApp
{
}


public interface IApp : IDisposable
{
    void Start();
    void Stop();
}