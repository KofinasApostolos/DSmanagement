using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.CustomModel;
using static WebApi.CustomModel.CustomMessageLogger;

namespace WebApi.Helpers
{
    public class MessageLogger : IMessageLogger
    {
        public List<CustomMessageLogger> msg = new List<CustomMessageLogger>();

        public void AddMessage(string message, object obj, MessageCode messagecode)
        {
            msg.Add(new CustomMessageLogger(message, obj, messagecode));
        }

        public void RemoveMessage(CustomMessageLogger logger)
        {
            msg.Remove(logger);
        }

        public void RemoveAllMessages()
        {
            msg.Clear();
        }


        public enum MessageCode
        {
            Information = 0,
            Warning = 1,
            Error = 2
        }
    }
}
