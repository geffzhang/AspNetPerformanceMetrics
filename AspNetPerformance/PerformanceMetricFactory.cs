using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspNetPerformance.Metrics;
using System.Diagnostics;


namespace AspNetPerformance
{


    /// <summary>
    /// Factory class 
    /// </summary>
    public static class PerformanceMetricFactory
    {

        #region Static Variables

        /// <summary>
        /// Dictionry of all the metrics that have been created through the life of the application
        /// </summary>
        private static Dictionary<ActionInfo, PerformanceMetricContainer> performanceMetrics;

        /// <summary>
        /// List of Custom Metrics that have been registered by the app to 
        /// </summary>
        private static List<Func<PerformanceMetricBase>> customMetrics;

        /// <summary>
        /// Object used for locking when we check to see if an ActionInfo already has its metrics created
        /// </summary>
        private static Object lockObject;

        #endregion


        static PerformanceMetricFactory()
        {
            performanceMetrics = new Dictionary<ActionInfo, PerformanceMetricContainer>();
            customMetrics = new List<Func<PerformanceMetricBase>>();
            lockObject = new Object();
        }


        /// <summary>
        /// Gets a List of performance metrics that will be measured on the action whose data is 
        /// represented by the given action info
        /// </summary>
        /// <param name="info">An ActionInfo object that contains info about the action whose performance
        /// is being measured</param>
        /// <returns>A List of PerformanceMetricBase objects of the metrics to be measured on this action</returns>
        public static List<PerformanceMetricBase> GetPerformanceMetrics(ActionInfo info)
        {
            if (performanceMetrics.ContainsKey(info) == false)
            {
                lock (lockObject)
                {
                    // Check Again
                    if (performanceMetrics.ContainsKey(info) == false)
                    {
                        List<PerformanceMetricBase> metrics = CreateMetricsForAction(info);
                         PerformanceMetricContainer pmc = new PerformanceMetricContainer(info, metrics);
                        performanceMetrics.Add(info, pmc);
                    }
                }
            }

            return performanceMetrics[info].GetPerformanceMetrics();
        }



        private static List<PerformanceMetricBase> CreateMetricsForAction(ActionInfo actionInfo)
        {
            List<PerformanceMetricBase> metrics = new List<PerformanceMetricBase>();

            // Add the standard metrics
            metrics.Add(new DeltaCallsMetric(actionInfo));
            metrics.Add(new TimerForEachRequestMetric(actionInfo));
            metrics.Add(new ActiveRequestsMetric(actionInfo));
            metrics.Add(new LastCallElapsedTimeMetric(actionInfo));
            metrics.Add(new DeltaExceptionsThrownMetric(actionInfo));
            metrics.Add(new PostAndPutRequestSizeMetric(actionInfo));

            // Now add any custom metrics the user may have added
            foreach (var x in customMetrics)
            {
                PerformanceMetricBase customMetric = x();
                metrics.Add(customMetric);
            }

            return metrics;
        }





        public static void AddCustomPerformanceMetric(Func<PerformanceMetricBase> customMetricCreator)
        {
            customMetrics.Add(customMetricCreator);
        }



        /// <summary>
        /// Method to clean up the performance counters on application exit
        /// </summary>
        /// <remarks>
        /// This method should only be called on application exit
        /// </remarks>
        public static void CleanupPerformanceMetrics()
        {
            // We'll make sure no one is trying to add while we are doing this, but should not
            // really be an issue
            lock (lockObject)
            {
                foreach (var pmc in performanceMetrics.Values)
                {
                    pmc.DisposePerformanceMetrics();
                }

                performanceMetrics.Clear();
                PerformanceCounter.CloseSharedResources();
            }
        }

    }




}
