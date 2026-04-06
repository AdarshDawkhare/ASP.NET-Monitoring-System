using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Contracts.Models;

namespace Lib.Services.ThreadMonitoring
{
    //This is the class where actual comparison happens between current and previous states of all currently monitored threads
    public class ThreadStateManager
    {
        private readonly Dictionary<int, ThreadSnapshot> _previousStates = new();

        public ThreadStateManager()
        {

        }

        public List<ThreadEvent> DetectChanges(List<ThreadSnapshot> currentSnapshots)
        {
            var events = new List<ThreadEvent>();

            foreach (var snapshot in currentSnapshots)
            {
                if (!_previousStates.ContainsKey(snapshot.ThreadId))
                {
                    //_previousStates does not contain this thread
                    HandleNewThread(snapshot, events);
                }
                else
                {
                    var PreviousSnapshot = _previousStates[snapshot.ThreadId];
                    DetectChangesForThread(PreviousSnapshot, snapshot, events);
                }

                // Update latest state
                _previousStates[snapshot.ThreadId] = Clone(snapshot);
            }

            return events;
        }

        //This method is used to detect the changes between current and previos snapshots of specific thread
        private void DetectChangesForThread(ThreadSnapshot prev,ThreadSnapshot curr,List<ThreadEvent> events)
        {
            DetectRequestChange(prev, curr, events);
            DetectThreadStateChange(prev, curr, events);
            DetectMethodChange(prev, curr, events);
        }

        //This method is used to raise the event if request of the current thread changes 
        private void DetectRequestChange(ThreadSnapshot prev,ThreadSnapshot curr,List<ThreadEvent> events)
        {
            if (prev.RequestId == curr.RequestId)
            {
                return;
            }

            if (prev.RequestId == null && curr.RequestId != null)
            {
                events.Add(CreateEvent(curr, ThreadEventType.RequestStarted));
            }
            else if (prev.RequestId != null && curr.RequestId == null)
            {
                events.Add(CreateEvent(curr, ThreadEventType.RequestCompleted));
            }
            else
            {
                events.Add(CreateEvent(curr, ThreadEventType.RequestSwitched));
            }
        }

        //This method is used to raise the event if state of the current thread changes 
        private void DetectThreadStateChange(ThreadSnapshot prev,ThreadSnapshot curr,List<ThreadEvent> events)
        {
            if (prev.IsBusy == curr.IsBusy)
            {
                return;
            }

            if (curr.IsBusy)
            {
                events.Add(CreateEvent(curr, ThreadEventType.ThreadBecameBusy));
            }
            else
            {
                events.Add(CreateEvent(curr, ThreadEventType.ThreadBecameIdle));
            }
        }

        //This method is used to raise the event if method which current thread was executing changes
        private void DetectMethodChange(ThreadSnapshot prev , ThreadSnapshot curr , List<ThreadEvent> events)
        {
            if (prev.MethodName == curr.MethodName && prev.ClassName == curr.ClassName)
            {
                //This means current thread still executing the same method which it was executing 
                return;
            }

            if (curr.MethodName != null)
            {
                //This means current thread is now executing the new method
                events.Add(CreateEvent(curr, ThreadEventType.MethodEntered));
            }
        }

        //This is common method used to raise event and create the object of ThreadEvent
        private ThreadEvent CreateEvent(ThreadSnapshot snapshot, ThreadEventType eventType)
        {
            return new ThreadEvent
            {
                ThreadId = snapshot.ThreadId,
                RequestId = snapshot.RequestId,
                EventType = eventType,
                Execution = snapshot.ClassName == null ? null : new ExecutionInfo
                {
                    ClassName = snapshot.ClassName,
                    MethodName = snapshot.MethodName,
                    Activity = snapshot.Activity
                },
                State = new ThreadStateInfo
                {
                    IsBusy = snapshot.IsBusy
                },
                Timestamp = snapshot.Timestamp
            };
        }

        // This method get called from DetectChanges method if new thread is used for new request which is not yet montitored 
        private void HandleNewThread(ThreadSnapshot snapshot, List<ThreadEvent> events)
        {
            if (snapshot.RequestId != null)
            {
                events.Add(CreateEvent(snapshot, ThreadEventType.RequestStarted));
            }

            if (snapshot.IsBusy)
            {
                events.Add(CreateEvent(snapshot, ThreadEventType.ThreadBecameBusy));
            }

            if (snapshot.MethodName != null)
            {
                events.Add(CreateEvent(snapshot, ThreadEventType.MethodEntered));
            }
        }

        private ThreadSnapshot Clone(ThreadSnapshot snapshot)
        {
            return new ThreadSnapshot
            {
                ThreadId = snapshot.ThreadId,
                RequestId = snapshot.RequestId,
                ClassName = snapshot.ClassName,
                MethodName = snapshot.MethodName,
                Activity = snapshot.Activity,
                IsBusy = snapshot.IsBusy,
                Timestamp = snapshot.Timestamp
            };
        }
    }
}
