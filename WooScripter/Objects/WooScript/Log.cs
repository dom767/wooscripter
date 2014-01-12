using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooScripter.Objects.WooScript
{
    public class Log
    {
        string _Log;
        int _Indent;
        public Log()
        {
            _Log = "";
            _Indent = 0;
        }
        public void Clear()
        {
            _Log = "";
            _Indent = 0;
        }
        public void AddMsg(string msg)
        {
            _Log += new string(' ', _Indent);
            _Log += msg;
            _Log += "\n";
        }
        public void Indent()
        {
            _Indent++;
        }
        public void UnIndent()
        {
            _Indent--;
        }
        public string GetLog()
        {
            return _Log;
        }
    }
}
