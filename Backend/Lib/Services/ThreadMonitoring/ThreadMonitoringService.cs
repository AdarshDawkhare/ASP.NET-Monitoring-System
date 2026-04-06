using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Contracts.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;

namespace Lib.Services.ThreadMonitoring
{
    //This is the background service used produce data continuously
    //Get snapshots from ThreadFlowTracker ------> Detect changes using ThreadStateManager -------> send via SignalR
    public class ThreadMonitoringService : BackgroundService
    {
        private readonly ThreadFlowTracker _tracker;
        private readonly ThreadStateManager _stateManager;
        private readonly IEventDispatcher _dispatcher;

        public ThreadMonitoringService(ThreadFlowTracker tracker,ThreadStateManager stateManager,IEventDispatcher eventDispatcher)
        {
            _tracker = tracker;
            _stateManager = stateManager;
            _dispatcher = eventDispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1. Get current snapshots
                    var snapshots = _tracker.GetSnapshots();

                    // 2. Detect changes
                    var events = _stateManager.DetectChanges(snapshots);

                    // 3. (Next step) Send events via SignalR
                    if (events.Any())
                    {
                        // We will implement this in next step
                        Console.WriteLine($"Events Count: {events.Count}");
                        await _dispatcher.DispatchAsync(events);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Monitoring Error: {ex.Message}");
                }

                // 4. Wait 100ms
                await Task.Delay(100, stoppingToken);
            }
        }
    }
}
