using Microsoft.Extensions.Hosting;
using System;

namespace ConsoleApp
{
    public interface IApp : IHostedService, IDisposable
    {
    }
}
