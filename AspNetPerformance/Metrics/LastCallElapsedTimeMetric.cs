using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Metrics;


namespace AspNetPerformance.Metrics
{
    /// <summary>
    /// Performance metric to update the counter that tracks the elapsed time of
    /// an action the last time it was called
    /// </summary>
    /// <remarks>
    /// This value is not an average.  It just just overwrites whatever value is
    /// in the counter, so you just have the last captured value
    /// </remarks>
    public class LastCallElapsedTimeMetric : PerformanceMetricBase
    {
        public LastCallElapsedTimeMetric(ActionInfo info)
            : base(info)
        {
        }

        /// <summary>
        /// Constant defining the name of this counter
        /// </summary>
        public const String COUNTER_NAME = "Last Call Elapsed Time";

      
        /// <summary>
        /// Method called by the custom action filter after the action completes
        /// </summary>
        /// <remarks>
        /// This method converts the elapsedTicks value to milliseconds, and then sets
        /// the counter "Last Call Elapsed Time" to this value, thereby overwriting whatever
        /// was in the counter previously.
        /// </remarks>
        /// <param name="elapsedTicks">A long of the ticks it took the action to complete</param>
        public override void OnActionComplete(long elapsedTicks, bool exceptionThrown)
        {
            // Need to convert the elapsed ticks into milliseconds for this counter
            long milliseconds = this.ConvertTicksToMilliseconds(elapsedTicks);
            String controllerName = this.actionInfo.ControllerName;
            String actionName = this.actionInfo.ActionName;
            string counterName = string.Format("{0} {1} {2}", controllerName, actionName, COUNTER_NAME);
            Metric.Context(this.actionInfo.ActionType).Gauge(counterName, () => milliseconds, Unit.Custom("Milliseconds"));
        }


        /// <summary>
        /// Disposes of the Performance Counter when the metric object is disposed
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
