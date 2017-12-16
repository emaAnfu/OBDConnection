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

        public PeriodicCommandRequest(Handler h, string command)
        {
            handler = h;
            cmd = command;
        }
    }
}