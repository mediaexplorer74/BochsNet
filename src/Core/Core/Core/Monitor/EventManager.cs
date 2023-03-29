using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Monitor
{
    public class EventManager
    {

        

        #region "Constructors"

        public EventManager()
        {
        }

        #endregion 


        #region "Delegates"
                public delegate void CallBackFunction (String EventName, Component Sender, EventArgument Arg);
        #endregion 

        #region "Methods"

        /// <summary>
        /// Third party is called when an event is raised.
        /// <para>This is ued to subscribe in the event.</para>
        /// </summary>
        /// <param name="EventName">event name that we want to capture.</param>
        /// <param name="CallMe">function to be called when this event is raised.</param>
        public static void CallMeOn(string EventName, CallBackFunction CallMe)
        {
            List<EventManager.CallBackFunction> CallBackList;
            EventRegisterar.EventList.TryGetValue(EventName, out CallBackList);
            if (CallBackList == null)
            {
                throw new InvalidOperationException(String.Format("Invalid event name [{%0}] or event is not supported.", EventName));
            }

            CallBackList.Add(CallMe);
        }

        #endregion 

    }
}
