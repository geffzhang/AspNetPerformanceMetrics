using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Metrics;

namespace AspNetPerformance.Metrics
{
    /// <summary>
    /// Performance Metric that updates the counters that track the average time a method took
    /// </summary>
    public class TimerForEachRequestMetric : PerformanceMetricBase
    {

        public TimerForEachRequestMetric(ActionInfo info)
            : base(info)
        {
            String controllerName = this.actionInfo.ControllerName;
            String actionName = this.actionInfo.ActionName;
            string counterName = string.Format("{0} {1}", controllerName, actionName);

            this.averageTimeCounter = Metric.Context(this.actionInfo.ActionType).Timer(counterName, Unit.Requests, SamplingType.FavourRecent,
                TimeUnit.Seconds, TimeUnit.Milliseconds);
        }


 
        #region Member Variables

        private Timer averageTimeCounter;
     

        #endregion

        /// <summary>
        /// Method called by the custom action filter after the action completes
        /// </summary>
        /// <remarks>
        /// This method increments the Average Time per Call counter by the number of ticks
        /// </remarks>
        /// <param name="elapsedTicks">A long of the number of ticks it took to complete the action</param>
        public override void OnActionComplete(long elapsedTicks, bool exceptionThrown)
        {
            averageTimeCounter.Record(elapsedTicks, TimeUnit.Nanoseconds);
        }

        /// <summary>
        /// Disposes of the  PerformanceCounter objects when the metric object is disposed
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
