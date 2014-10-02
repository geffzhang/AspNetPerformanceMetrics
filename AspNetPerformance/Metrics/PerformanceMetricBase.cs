using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AspNetPerformance.Metrics
{

    /// <summary>
    /// Base class for all performance metric objects
    /// </summary>
    public abstract class PerformanceMetricBase : IDisposable
    {

        /// <summary>
        /// Creates a new PerformanceMetricBase object that will update data for the
        /// action described by the given ActionInfo object
        /// </summary>
        /// <param name="info">An ActionInfo object that contains data about the action 
        /// that was called</param>
        public PerformanceMetricBase(ActionInfo info)
        {
            this.actionInfo = info;            
        }


        /// <summary>
        /// Gets the object that contains info on the action that was called and is 
        /// having perfomance tracked
        /// </summary>
        protected ActionInfo actionInfo;


        /// <summary>
        /// Method called by the custom action filter after the action completes
        /// </summary>
        /// <remarks>
        /// Performance metric objects should provide an implementation for either this method
        /// or the OnActionComplete() method, or potentially both if they have processing to perform
        /// both before and after the action executes.  This method is defined as virtual with an
        /// empty implementation so that if a derived object only wants to implement one method,
        /// it does not have to provide a default implemtation for the other
        /// </remarks>
        public virtual void OnActionStart()
        {

        }

        /// <summary>
        /// Method called by the custom action filter after the action completes
        /// </summary>
        /// <remarks>
        /// Performance metric objects should provide an implementation for either this method
        /// or the OnActionStart() method, or potentially both if they have processing to perform
        /// both before and after the action executes.  This method is defined as virtual with an
        /// empty implementation so that if a derived object only wants to implement one method,
        /// it does not have to provide a default implemtation for the other
        /// </remarks>
        /// <param name="elapsedTicks">A long of the ticks it took the action to complete</param>
        /// <param name="exceptionThrown">A bool if an uncaught exception was thrown during the processing of this action</param>
        public virtual void OnActionComplete(long elapsedTicks, bool exceptionThrown)
        {

        }
      

        /// <summary>
        /// Helper method to convert ticks from the Stopwatch class to milliseconds
        /// </summary>
        /// <remarks>
        /// This method will round, so 1.6 milliseconds would become 2 millisecons.  The
        /// method returns a long because that is what the performance counters want
        /// </remarks>
        /// <param name="elapsedTicks">A long of the number of ticks</param>
        /// <returns>A long of the corresponding value in milliseconds</returns>
        protected long ConvertTicksToMilliseconds(long elapsedTicks)
        {
            decimal d =  Math.Round(1000 * (decimal)elapsedTicks / Stopwatch.Frequency);
            return Convert.ToInt64(d);
        }


        /// <summary>
        /// Abstract method where concrete implementations should call Dispose() on 
        /// any performance counters they hold references to
        /// </summary>
        public virtual void Dispose()
        {
        }

    }
}
