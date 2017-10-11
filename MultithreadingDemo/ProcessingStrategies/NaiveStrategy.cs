using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadingDemo.ProcessingStrategies
{
    /// <summary>
    /// This uses a single thread, processing each event sequentially with no concurrency optimizations. 
    /// This best represents a straightforward implementation that might be run as a script or server process.
    /// It uses an Active Object implementation (because it has to have its own thread separate from the GUI thread)
    /// </summary>
    internal class NaiveStrategy : BaseActiveObject, IStrategy
    {
        private const int MAXIMUM_QUEUE_SIZE = 10000;
            
        private readonly ConcurrentQueue<Object> _eventQueue = new ConcurrentQueue<Object>();

        public NaiveStrategy()
        {            
            this.StartActiveObject("NaiveStrategy");
        }
                
        public string GetRunningInformation()
        {
            throw new NotImplementedException();
        }

        public void OnDataAvailable(object[] data)
        {
            foreach (Object datum in data)
            {
                while (this._eventQueue.Count > MAXIMUM_QUEUE_SIZE)
                {
                    // If this were a production application, this would need a ternimal condition if it waits for too long.
                    System.Threading.Thread.Sleep(1); // Wait a minimum amount of time and try to store it afterwards
                }

                this._eventQueue.Enqueue(datum);
            }
        }

        protected override void ACTIVE_implementation()
        {
            object nextEvent = null;
            while (true)
            {                
                if (base._signalShouldShutDown.Wait(0) == true)
                {
                    return;
                }

                if (this._eventQueue.TryDequeue(out nextEvent) == false)
                {
                    // ---- Queue is empty, so log that the processing thread is stalling
                    System.Threading.Thread.Sleep(1);
                    continue;
                }
                ProcessEvent(nextEvent);
            }
        }

        private void ProcessEvent(object data) {
            // ---- TODO
        }

    }
}
