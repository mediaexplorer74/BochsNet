using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Monitor
{
    public static class EventRegisterar
    {

        #region "Attributes"

        public static Dictionary<string, List<EventManager.CallBackFunction>> EventList = new Dictionary<string, List<EventManager.CallBackFunction>>();

        #endregion


        #region "Constructors"

        static EventRegisterar()
        {

            #region "Register Events"
            Core.Monitor.EventRegisterar.AddEvent("CPU.OnChangeMode");
            Core.Monitor.EventRegisterar.AddEvent("CPU.OnExecuteInstruction");
            #endregion

            #region "Register Events"
            Core.Monitor.EventRegisterar.AddEvent("Memory.Write");
            Core.Monitor.EventRegisterar.AddEvent("Memory.Read");
            #endregion 

        }

        #endregion





        #region "Methods"

        /// <summary>
        /// This function is used to raise the event
        /// <para>Subscribers in the List are all called.</para>
        /// </summary>
        /// <param name="EventName"></param>
        /// <param name="Sender"></param>
        /// <param name="Argument">Event Parameter</param>
        public static void RaiseEvent(string EventName, Component Sender, EventArgument Argument)
        {
            List<EventManager.CallBackFunction> CallBackList;
            EventList.TryGetValue(EventName, out CallBackList);
            if (CallBackList == null)
            {
                throw new InvalidOperationException(String.Format("Invalid event name [%1] or event is not supported.", EventName));
            }

            foreach (var CallBackFunction in CallBackList)
            {
                CallBackFunction(EventName, Sender, Argument);
            }
        }


        /// <summary>
        /// Used by event trigger to define a new event in the event list 
        /// <para>and enables others to subscribe in this event.</para>
        /// </summary>
        /// <param name="EventName"></param>
        public static void AddEvent(string EventName)
        {
            List<EventManager.CallBackFunction> CallBackList;
            EventList.TryGetValue(EventName, out CallBackList);
            if (CallBackList != null)
            {
                throw new InvalidOperationException(String.Format("Event [%1] already exists", EventName));
            }
            EventList.Add(EventName, new List<EventManager.CallBackFunction>());
        }

        #endregion


    }
}
