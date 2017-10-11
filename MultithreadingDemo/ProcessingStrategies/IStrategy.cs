using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultithreadingDemo.ProcessingStrategies
{
    /// <summary>
    /// Represents a given strategy for processing work. each strategy itself is a bit of an actor, in
    /// that it is told to start and told to stop, and it internally handles whatever threading strategy
    /// it wants. Additionally, all strategies with
    /// </summary>
    interface IStrategy : IDisposable
    {
        // A startup function is unnecessary, as these generally just process work as it is received.
        
        // Disposal through IDisposable is the shutdown procedure
        // ---- though instead of Dispose, it may have an async-await GUI method to shut-down, so that the GUI can continue to 
        //      run and be responsive until the strategy has indeed shut down.
        
            
        /// <summary>
        /// Method to hand new data to process (presumably from the DataGenerator). This is a blocking method,
        /// and will block if the strategy is sufficiently far behind in processing (which should normally
        /// be the case for performance-testing a strategy) such that it cannot yet receive the new data.
        /// 
        /// This method is expected to be fully thread-safe (though if the data being processed is
        /// chronological or sequential, then presumably it would be unsafe to hande it data via
        /// more than one thread)
        /// </summary>
        /// <param name="data"></param>
        /// <remarks>
        /// This is used instead of ONLY using a ConcurrentQueue. For example, if we use Tasks to break up
        /// the processing work, then there would have to be some separate thread running soley to dequeue
        /// units of data to process and then spawn tasks for them. So this method allows the strategy to
        /// use whatever storage method is best suited for it.
        /// 
        /// Also, by making the data an array, it allows the generator to supply a decently large chunk of data
        /// to process instead of incurring overhead of calling this method for every single new piece of data)
        /// </remarks>
        void OnDataAvailable(object[] data);

        /// <summary>
        /// Method to create a description of the running state of this strategy to display to the user.
        /// 
        /// This method is expcted to be fully thread-safe, as one or more GUI threads can request this
        /// information to display at any time.
        /// </summary>
        /// <returns></returns>
        string GetRunningInformation();





    }
}
