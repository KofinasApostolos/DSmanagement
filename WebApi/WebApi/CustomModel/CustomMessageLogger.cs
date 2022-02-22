using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebApi.Helpers.MessageLogger;

namespace WebApi.CustomModel
{
    public class CustomMessageLogger
    {
        public string Message { get; set; }
        public object Obj { get; set; }
        public MessageCode Messagecode { get; set; }

        public CustomMessageLogger(string message, object obj, MessageCode messageCode)
        {
            Message = message;
            Obj = obj;
            Messagecode = messageCode;
        }
    }
}
