using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Lib.Contracts.Models;

namespace Lib.Services.ThreadMonitoring
{
    //This class is used for data collection ---> Data related to all currently active threads will be stored in this class 
    //Methods of this class will be called from Middleware(Request change Identification) and Interceptor(MethodName change identification)
    public class ThreadFlowTracker
    {
        private readonly ConcurrentDictionary<int, ThreadSnapshot> _threads = new();

        public ThreadFlowTracker() 
        {

        }

        //Why we need this?
        //We don’t want logic duplicated in: Request start , Method start , Method end ----> So we centralize updates
        private void UpdateThread(int threadId,Action<ThreadSnapshot> updateAction)
        {
            _threads.AddOrUpdate(
                threadId,
                id =>
                {
                    var snapshot = CreateNewSnapshot(id);
                    updateAction(snapshot);
                    snapshot.Timestamp = DateTime.UtcNow;
                    return snapshot;
                },
                (id, existing) =>
                {
                    updateAction(existing);
                    existing.Timestamp = DateTime.UtcNow;
                    return existing;
                }
            );
        }

        //Method to create Default Snapshot : When a thread appears first time (no previous state exists) : We initialize it safely
        private ThreadSnapshot CreateNewSnapshot(int threadId)
        {
            return new ThreadSnapshot
            {
                ThreadId = threadId,
                RequestId = null,
                ClassName = null,
                MethodName = null,
                Activity = null,
                IsBusy = false,
                Timestamp = DateTime.UtcNow
            };
        }

        private string ResolveActivity(string className)
        {
            if (className.EndsWith("Controller"))
            {
                return "Controller";
            }

            if (className.EndsWith("Service"))
            {
                return "Service";
            }

            if (className.EndsWith("Repository"))
            {
                return "Repository";
            }

            return "Other";
        }

        //This method will be trigger from : Middleware --> need to be public
        public void OnRequestStart(int threadId , string requestId)
        {
            UpdateThread(threadId, snapshot =>
            {
                snapshot.RequestId = requestId;
                snapshot.IsBusy = true;

                // Clear previous execution
                snapshot.ClassName = null;
                snapshot.MethodName = null;
                snapshot.Activity = "Request";
            });
        }

        //This method will be trigger from : Middleware  --> need to be public
        //Why NOT clear method? UI can still show: “last executed method before becoming idle”
        public void OnRequestEnd(int threadId)
        {
            UpdateThread(threadId, snapshot => {
                snapshot.RequestId = null;
                snapshot.IsBusy = false;

                // Keep last method for UI visibility
                snapshot.Activity = "Idle";
            });
        }

        //This method will be trigger from : Interceptor --> need to be public
        public void onStartMethod(int threadId, string className, string MethodName)
        {
            UpdateThread(threadId, snapshot => {
                snapshot.ClassName = className;
                snapshot.MethodName = MethodName;
                snapshot.Activity = ResolveActivity(className);
                snapshot.IsBusy = true;
            });
        }

        //This method will be used by : ThreadStateManager --> need to be public
        public List<ThreadSnapshot> GetSnapshots()
        {
            return _threads.Values
                .Select(t => new ThreadSnapshot
                {
                    ThreadId = t.ThreadId,
                    RequestId = t.RequestId,
                    ClassName = t.ClassName,
                    MethodName = t.MethodName,
                    Activity = t.Activity,
                    IsBusy = t.IsBusy,
                    Timestamp = t.Timestamp
                })
                .ToList();
        }
    }
}
