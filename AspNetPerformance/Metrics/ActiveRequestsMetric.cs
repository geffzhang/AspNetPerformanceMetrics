using Metrics;
using System;
using System.Diagnostics;


namespace AspNetPerformance.Metrics
{
    /// <summary>
    /// Metric to track how many calls are currently in progresss to this action
    /// </summary>
    public class ActiveRequestsMetric : PerformanceMetricBase
    {

        public ActiveRequestsMetric(ActionInfo info)
            : base(info)
        {
            String categoryName = this.actionInfo.ControllerName;
            String instanceName = this.actionInfo.ActionName;
            string counterName = string.Format("{0} {1} {2}", categoryName, instanceName, COUNTER_NAME);
            this.callsInProgressCounter = Metric.Context(this.actionInfo.ActionType).Counter(counterName, Unit.Custom(COUNTER_NAME));
        }


        /// <summary>
        /// Constant defining the name of this counter
        /// </summary>
        public const String COUNTER_NAME = "ActiveRequests";


        private Counter callsInProgressCounter;

        /// <summary>
        /// Method called by the custom action filter just prior to the action begining to execute
        /// </summary>
        /// <remarks>
        /// This method increments the Calls in Progress counter by 1
        /// </remarks>
        public override void OnActionStart()
        {
            this.callsInProgressCounter.Increment();
        }


        /// <summary>
        /// Method called by the custom action filter after the action completes
        /// </summary>
        /// <remarks>
        /// This method decrements the Calls in Progress counter by 1
        /// </remarks>
        public override void OnActionComplete(long elapsedTicks, bool exceptionThrown)
        {
            this.callsInProgressCounter.Decrement();
        }

        /// <summary>
        /// Disposes of the Performance Counter when the metric object is disposed
        /// </summary>
        public override void Dispose()
        {
            //this.callsInProgressCounter.Dispose();
        }
    }
}
