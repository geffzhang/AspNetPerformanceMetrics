using Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetPerformance.Metrics
{
    public class PostAndPutRequestSizeMetric : PerformanceMetricBase
    {
        public PostAndPutRequestSizeMetric(ActionInfo info)
            : base(info)
        {
            this.histogram = Metric.Context(this.actionInfo.ActionType).Histogram(COUNTER_NAME, Unit.Bytes, SamplingType.FavourRecent);
        }


        /// <summary>
        /// Constant defining the name of this counter
        /// </summary>
        public const String COUNTER_NAME = "Post & Put Request Size";


        /// <summary>
        /// Reference to the performance counter 
        /// </summary>
        private Histogram histogram;

        public override void OnActionStart()
        {
            var method = this.actionInfo.HttpMethod.ToUpper();
            if (method == "POST" || method == "PUT")
            {
                histogram.Update(this.actionInfo.ContentLength);
            }
        }

        public override void Dispose()
        {
           
        }
    }
}
