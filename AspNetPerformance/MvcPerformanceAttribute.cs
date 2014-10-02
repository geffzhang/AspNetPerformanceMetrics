using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Diagnostics;
using AspNetPerformance.Metrics;
using System.Web;
using System.Threading.Tasks;


namespace AspNetPerformance
{
    /// <summary>
    /// Custom action filter to track the performance of MVC actions
    /// </summary>    
    public class MvcPerformanceAttribute : ActionFilterAttribute
    {

        public MvcPerformanceAttribute()
        {
        }

        /// <summary>
        /// Constant to identify MVC Action Types (used in the instance name)
        /// </summary>
        public const String ACTION_TYPE = "MVC";


        /// <summary>
        /// Method called before the action method starts processing
        /// </summary>
        /// <param name="filterContext">An ActionExecutingContext object</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // First thing is to check if performance is enabled globally.  If not, return
            if ( ConfigInfo.Value.PerformanceEnabled == false)
            {
                return;
            }
            
            // Second thing, check if performance tracking has been turned off for this action
            // If the DoNotTrackAttribute is present, then return
            ActionDescriptor actionDescriptor = filterContext.ActionDescriptor;

            if (actionDescriptor.GetCustomAttributes(typeof(DoNotTrackPerformanceAttribute), true).Length > 0
                || actionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(DoNotTrackPerformanceAttribute), true).Length > 0)
            {
                return;
            }

            // ActionInfo encapsulates all the info about the action being invoked
            ActionInfo info = this.CreateActionInfo(filterContext);

            // PerformanceTracker is the object that tracks performance and is attached to the request
            PerformanceTracker tracker = new PerformanceTracker(info);
           
            // Store this on the request
            String contextKey = this.GetUniqueContextKey(filterContext.ActionDescriptor.UniqueId);
            HttpContext.Current.Items.Add(contextKey, tracker);
                        
            // Process the action start - this is what starts the timer and increments any
            // required counters before the action executes
            tracker.ProcessActionStart();
        }


        /// <summary>
        /// Method called after the action method has completed executing
        /// </summary>
        /// <remarks>
        /// This method first checks to make sure we are indeed tracking performance.  If so, it stops
        /// the stopwatch and then calls the OnActionComplete() method of all of the performance metric
        /// objects attached to this action filter
        /// </remarks>
        /// <param name="filterContext">An ActionExecutedConext object</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // This is the unique key the PerformanceTracker object would be stored under
            String contextKey = this.GetUniqueContextKey(filterContext.ActionDescriptor.UniqueId);

            // Check if there is an object on the request.  If not, must not be tracking performance
            // for this action, so just go ahead and return
            if (HttpContext.Current.Items.Contains(contextKey) == false)
            {
                return;
            }

            // If we are here, we are tracking performance.  Extract the object from the request and call
            // ProcessActionComplete.  This will stop the stopwatch and update the performance metrics
            PerformanceTracker tracker = HttpContext.Current.Items[contextKey] as PerformanceTracker;

            if (tracker != null)
            {
                bool exceptionThrown = (filterContext.Exception != null);
                tracker.ProcessActionComplete(exceptionThrown);
            }
        }


        #region Helper Methdos

        /// <summary>
        /// Helper method to create the ActionInfo object containing the info about the action that is getting called
        /// </summary>
        /// <param name="actionContext">The ActionExecutingContext from the OnActionExecuting() method</param>
        /// <returns>An ActionInfo object that contains all the information pertaining to what action is being executed</returns>
        private ActionInfo CreateActionInfo(ActionExecutingContext actionContext)
        {
            var parameters = actionContext.ActionDescriptor.GetParameters().Select(p => p.ParameterName);
            String parameterString = String.Join(",", parameters);

            int processId = ConfigInfo.Value.ProcessId;
            String controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            String actionName = actionContext.ActionDescriptor.ActionName;
            String httpMethod = HttpContext.Current.Request.HttpMethod;
            int contentLength = HttpContext.Current.Request.ContentLength;

            ActionInfo info = new ActionInfo(processId, ACTION_TYPE,
                controllerName, actionName, httpMethod, parameterString,contentLength);

            return info;
        }


        /// <summary>
        /// Helper method to form the key that will be used to store/retrieve the PerformanceTracker object
        /// off if the HttpContext
        /// </summary>
        /// <remarks>
        /// To minimize any chance of collisions, this method concatenates the full name of this class
        /// with the UniqueID of the MVC action to get a unique key to use
        /// </remarks>
        /// <param name="actionUniqueId">A String of the unique id assigned by ASP.NET to the MVC action</param>
        /// <returns>A Strin suitable to be used for the key</returns>
        private String GetUniqueContextKey(String actionUniqueId)
        {
            return this.GetType().FullName + ":" + actionUniqueId;
        }

        #endregion
    }
}
