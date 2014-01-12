using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class ParseException : System.ApplicationException
    {
        public string _WooMessage;

        public ParseException() { }
        public ParseException(string message) { _WooMessage = message; }
        public ParseException(string message, System.Exception inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected ParseException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class EvaluateException : System.ApplicationException
    {
        public EvaluateException() { }
        public EvaluateException(string message) { }
        public EvaluateException(string message, System.Exception inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected EvaluateException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
