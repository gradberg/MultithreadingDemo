using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadingDemo
{
    public abstract class BaseActiveObject : IDisposable
    {
        protected ManualResetEventSlim _signalShouldShutDown = new ManualResetEventSlim(false);
        private Thread _activeThread;

        protected void StartActiveObject(string activeObjectName, ThreadPriority threadPriority = ThreadPriority.Normal)
        {
            // Start the Active Object
            this._activeThread = new Thread(ACTIVE_implementation);
            this._activeThread.SetApartmentState(ApartmentState.MTA);
            this._activeThread.IsBackground = true; // No reason for this thread to prevent shutdown
            this._activeThread.Name = activeObjectName + " ACTIVE Object";
            this._activeThread.Priority = threadPriority;
            this._activeThread.Start();
        }

        public void Dispose()
        {
            _signalShouldShutDown.Set();
            _activeThread.Join();
            _signalShouldShutDown.Dispose();
        }

        protected abstract void ACTIVE_implementation();
    }
}
