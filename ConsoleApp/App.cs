﻿using CoreLibrary.SilkDotNet.Window;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public interface IApp
    {
    }

    public class App : IApp, IHostedService, IDisposable
    {
        private readonly IWindowEventHandler _windowEventHandler;
        private bool disposed;

        public App(SilkDotNetWindowEventHandler windowEventHandler)
        {
            _windowEventHandler = windowEventHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("App Starting...");
            await _windowEventHandler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("App Stopping...");
            await _windowEventHandler.Stop(cancellationToken);
        }

        public void Dispose()
        {
            Log.Information("App Disposing...");
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    _windowEventHandler.Dispose();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        ~App()
        {
            Dispose(disposing: false);
        }
    }
}