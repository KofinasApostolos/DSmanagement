using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.CustomModel;
using static WebApi.CustomModel.CustomMessageLogger;
using static WebApi.Helpers.MessageLogger;

namespace WebApi.Helpers
{
    public interface IMessageLogger
    {
        void AddMessage(string message, object obj, MessageCode messageCode);
        void RemoveMessage(CustomMessageLogger logger);
        void RemoveAllMessages();
    }
}
