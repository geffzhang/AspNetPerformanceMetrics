using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetPerformance
{
    /// <summary>
    /// Class to hold information about the action method which performance metrics are being gathered
    /// </summary>
    public class ActionInfo
    {

        public ActionInfo(int processId, String actionType, String controllerName,
            String actionName, String httpMethod, String paramters, int contentLength)
        {
            this.ProcessId = processId;
            this.ActionType = actionType;
            this.ControllerName = controllerName;
            this.ActionName = actionName;
            this.HttpMethod = httpMethod;
            this.Parameters = paramters;
            this.ContentLength = contentLength;

            this.InstanceName = this.DetermineRawInstanceName();
            this.SanitizedInstanceName = 
                InstanceNameRegistry.GetSanitizedInstanceName(this.InstanceName);
        }


        /// <summary>
        /// Gets/Sets the name of the performance counter category.
        /// </summary>
        public String PerformaneCounterCategory { get; private set; }


        /// <summary>
        /// Gets/Sets the int of the Process ID the application is running under
        /// </summary>
        /// <remarks>
        /// This is the process id of the ASP.NET worker process.  It is used as the
        /// first element in the instance name for the performance counters.  It is
        /// also useful to know to match up these statistics with data in the Process
        /// category
        /// </remarks>
        public int ProcessId { get; private set; }

        /// <summary>
        /// Gets/Sets a String which indicates if the Action was an MVC or WebApi action
        /// </summary>
        public String ActionType { get; private set; }

        /// <summary>
        /// Gets/Sets the nme of the controller this action belongs to
        /// </summary>
        public String ControllerName { get; private set; }

        /// <summary>
        /// Gets/sets the name of this action
        /// </summary>
        public String ActionName { get; private set; }

        /// <summary>
        /// Gets/Sets the HttpMethod (GET, POST, PUT, DELETE, etc) used for this action.
        /// </summary>
        /// <remarks>
        /// In MVC, some actions (typically an EDIT) have both definition for both GET and
        /// POST.  This value helps differentiate between those two calls
        /// </remarks>
        public String HttpMethod { get; private set; }


        /// <summary>
        /// Gets/Sets a String that represents the parameters passed to this action
        /// </summary>
        /// <remarks></remarks>
        public String Parameters { get; private set; }


        /// <summary>
        /// Gets the instance name that will be used to record performance for performance metrics
        /// (counters) on the action represented by this object
        /// </summary>
        public String InstanceName { get; private set; }


        /// <summary>
        /// Gets a sanitized version of the instane name that is safe to pass to the 
        /// PerformanceCounter object (i.e. will be 128 chars or less)
        /// </summary>
        public String SanitizedInstanceName { get; set; }

        /// <summary>
        /// ContentLength
        /// </summary>
        public int ContentLength { get; set; }


        /// <summary>
        /// Helper method to determine the instance name to use from all the paramters
        /// </summary>
        /// <returns></returns>
        private String DetermineRawInstanceName()
        {
            String rawInstanceName = String.Format("[{0}]-{1} {2}.{3}[{5}] {4}",
                this.ProcessId,
                this.ActionType,
                this.ControllerName,
                this.ActionName,
                this.HttpMethod,
                this.Parameters);
            return rawInstanceName;
        }


        #region Utility Methods

        /// <summary>
        /// Checks to see if the given object is equivalent to this object
        /// </summary>
        /// <remarks>
        /// Need to do this since we are overridin GetHashCode().
        /// </remarks>
        /// <param name="obj">Another ActionInfo object</param>
        /// <returns>True if the objects represent the same controller action.  False otherwise</returns>
        public override bool Equals(object obj)
        {
            ActionInfo other = obj as ActionInfo;
            if (obj == null)
                return false;

            return this.InstanceName.Equals(other.InstanceName);
        }

        /// <summary>
        /// Gets the hash code for this object
        /// </summary>
        /// <remarks>
        /// ActionInfo will be used as the key for a Dictionary, so we need to define
        /// how we want to hash code to behave so we don't have any unexpected behavior.
        /// In this case, it will be based off of the instance name
        /// </remarks>
        /// <returns>An int of the hash code</returns>
        public override int GetHashCode()
        {
            return this.InstanceName.GetHashCode();
        }

        #endregion

    }
}
