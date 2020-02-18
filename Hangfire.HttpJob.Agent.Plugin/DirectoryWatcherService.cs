using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public class DirectoryWatcherService : IHostedService, IDisposable
    {
        private FileSystemWatcher _watcher;
        private readonly DirectoryWatcherHandler _watcherHandler;

        public DirectoryWatcherService(DirectoryWatcherHandler watcherHandler)
        {
            _watcherHandler = watcherHandler;
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "plugins");
            var filter = "*.dll";

            _watcher = new FileSystemWatcher();
            _watcher.Path = path;
            _watcher.Filter = filter;
            _watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName |
                              NotifyFilters.DirectoryName;

            _watcher.IncludeSubdirectories = true;

            _watcher.Changed += _watcherHandler.OnEvent;
            _watcher.Created += _watcherHandler.OnEvent;
            _watcher.Deleted += _watcherHandler.OnEvent;
            _watcher.Renamed += _watcherHandler.OnEvent;
            _watcher.Error += _watcherHandler.OnEventError;

            _watcher.EnableRaisingEvents = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _watcher.EnableRaisingEvents = false;

            return Task.CompletedTask;
        }

        //protected override Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    var path = Path.Combine(AppContext.BaseDirectory, "plugins");
        //    var filter = "*.txt";

        //    _watcher = new FileSystemWatcher();
        //    _watcher.Path = path;
        //    _watcher.Filter = filter;
        //    _watcher.NotifyFilter = NotifyFilters.LastWrite;

        //    DirectoryWatcherHandler Handler = new DirectoryWatcherHandler();
        //    _watcher.Changed += Handler.OnEvent;
        //    _watcher.Created += Handler.OnEvent;
        //    _watcher.Deleted += Handler.OnEvent;
        //    _watcher.Renamed += Handler.OnEvent;
        //    _watcher.Error += Handler.OnEventError;

        //    _watcher.EnableRaisingEvents = true;

        //    return Task.CompletedTask;
        //}
    }
}
