using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetPerformance
{

    /// <summary>
    /// A marker attribute used to decorate actions or controllers where performance data should
    /// not be gathered
    /// </summary>
    /// <remarks>
    /// It is generally easier to add the filters to track performane globally, but there may be
    /// cases where you want to have certain actions or even whole controllers "opt out" of
    /// tracking performance.  This attrinute provides a convenient way to do so.  The filters
    /// to track performance can still be added globally, but by decorating an action or
    /// controller with this attribute will exclude that item from performance tracking.
    /// </remarks>
    public class DoNotTrackPerformanceAttribute : Attribute
    {
    }
}
