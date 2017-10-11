using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MultithreadingDemo.ProcessingStrategies;

namespace MultithreadingDemo
{
    /// <summary>
    /// Imitates a source of data, either a real-time feed that has been subscribed, or some large data store that is being stream out of.
    /// It produces string FIX messages, which then each processing strategy must parse to use.
    /// </summary>
    internal class DataGenerator : BaseActiveObject
    {
        private const int MESSAGE_BATCH_COUNT = 1000;
        private const int MAXIMUM_CONCURRENT_ORDERS = 100;
        // Gives it priority over ProcessingStrategy threads, so it is more likley to produce data faster than
        // the strategy can consume it.
        private const ThreadPriority THREAD_PRIORITY = ThreadPriority.AboveNormal;
        
        private readonly IStrategy _strategy;
        private readonly GeneratorState _generatorState;

        public DataGenerator(IStrategy strategy)
        {            
            this._strategy = strategy;

            var randomSeed = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
            this._generatorState = new GeneratorState(MAXIMUM_CONCURRENT_ORDERS, randomSeed); 

            base.StartActiveObject("DataGenerator", THREAD_PRIORITY);                                       
        }
        
        protected override void ACTIVE_implementation()
        {
            while (true)
            {
                if (this._signalShouldShutDown.Wait(0) == true)
                {
                    // Generator has been told to shut down, generate file events (which close all open
                    // orders) and then exit.
                    var lastData = GenerateFinalEvents();
                    this._strategy.OnDataAvailable(lastData);
                    return;
                }

                var newData = this.GenerateMoreData();
                this._strategy.OnDataAvailable(newData);
            }
        }

        private string[] GenerateMoreData()
        {
            var results = new string[MESSAGE_BATCH_COUNT];
            for (int index = 0; index < MESSAGE_BATCH_COUNT; index++)
            {
                results[index] = this._generatorState.GenerateNextEvent();
            }
            return results;
        }

        private string[] GenerateFinalEvents()
        {
            return this._generatorState.GenerateFinalEvents();
        }
    }
}
