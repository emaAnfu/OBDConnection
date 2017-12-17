using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OBDConnection
{
    public class PeriodicCommandRequest
    {
        public Handler handler;
        public string cmd;
        public int messageType;

        public PeriodicCommandRequest(Handler h, string command, int messageType)
        {
            handler = h;
            cmd = command;
            this.messageType = messageType;
        }
    }
}