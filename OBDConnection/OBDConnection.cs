


using Android.App;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using Android.Content;
using Java.Lang;
using Android.Views;
using Android.Views.InputMethods;
using System;

using System.Threading;
using Android.Util;
//using System.Timers;

namespace OBDConnection
{
    [Activity(Label = "OBDConnection", MainLauncher = true, Icon = "@drawable/fiestaIcon")]
    class OBDConnection : Activity
    {
        // Debugging
        private const string TAG = "OBDConnection";
        private const bool Debug = true;

        // Intent request codes
        // TODO: Make into Enums
        private const int REQUEST_CONNECT_DEVICE = 1;
        private const int REQUEST_ENABLE_BT = 2;

        // Message types sent from the OBDConnectionService Handler
        // TODO: Make into Enums
        public const int MESSAGE_STATE_CHANGE = 1;
        public const int MESSAGE_READ = 2;
        public const int MESSAGE_WRITE = 3;
        public const int MESSAGE_DEVICE_NAME = 4;
        public const int MESSAGE_TOAST = 5;

        // Message types sent from the Timer Handler
        public const int TIMER_RPM_RAISED = 6;
        public const int TIMER_SPEED_RAISED = 7;

        // Key names received from the BluetoothChatService Handler
        public const string DEVICE_NAME = "device_name";
        public const string TOAST = "toast";

        // Local Bluetooth adapter
        private BluetoothAdapter bluetoothAdapter = null;
        // Member object for the chat services
        private OBDConnectionService connectionService = null;
        // String buffer for outgoing messages
        private StringBuffer outStringBuffer;
        // Name of the connected device
        protected string connectedDeviceName = null;

        //these will be modified:

        // Layout Views
        protected TextView title;
        private ListView conversationView;
        private EditText outEditText;
        private Button sendButton;
        // AT
        private Button button_atReset;
        private Button button_atAutomaticProtocol;
        private Button button_atReadVoltage;
        // OBD
        private Button button_obdRPM;
        private Button button_obdSpeed;
        // Multiple requests
        private Button button_startSendRPM;
        private Button button_stopSendRPM;
        private Button button_startSendSpeed;
        private Button button_stopSendSpeed;

        // Various
        private Button button_seeRPM;

        // Timers for the periodic requests
        Timer timer_rpm;
        Timer timer_speed;

        // Array adapter for the conversation thread
        protected ArrayAdapter<string> conversationArrayAdapter;

        // Input buffer from bt
        byte[] readBuf;
        int lenghtBuf;

        // Command objects
        RPMCommand r;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get local Bluetooth adapter
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            // If the adapter is null, then Bluetooth is not supported
            if (bluetoothAdapter == null)
            {
                Toast.MakeText(this, "Bluetooth is not available", ToastLength.Long).Show();
                Finish();
                return;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            // If BT is not on, request that it be enabled.
            // setupChat() will then be called during onActivityResult
            if (!bluetoothAdapter.IsEnabled)
            {
                Intent enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableIntent, REQUEST_ENABLE_BT);
                // Otherwise, setup the chat session
            }
            else
            {
                if (connectionService == null)
                    SetupConnection();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Performing this check in onResume() covers the case in which BT was
            // not enabled during onStart(), so we were paused to enable it...
            // onResume() will be called when ACTION_REQUEST_ENABLE activity returns.
            if (connectionService != null)
            {
                // Only if the state is STATE_NONE, do we know that we haven't started already
                if (connectionService.GetState() == OBDConnectionService.STATE_NONE)
                {
                    // Start the Bluetooth chat services
                    connectionService.Start();
                }
            }
        }

        private void SetupConnection()
        {
            // Initialize the BluetoothChatService to perform bluetooth connections
            Handler myHandler = new MyHandler(this);
            connectionService = new OBDConnectionService(this, myHandler);

            // Here initialize the main activity layout:

            // Initialize the array adapter for the conversation thread
            conversationArrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.message);
            conversationView = FindViewById<ListView>(Resource.Id.@in);
            conversationView.Adapter = conversationArrayAdapter;

            // Initialize the compose field with a listener for the return key
            outEditText = FindViewById<EditText>(Resource.Id.edit_text_out);
            // The action listener for the EditText widget, to listen for the return key
            outEditText.EditorAction += delegate (object sender, TextView.EditorActionEventArgs e) {
                // If the action is a key-up event on the return key, send the message
                if (e.ActionId == ImeAction.ImeNull && e.Event.Action == KeyEventActions.Up)
                {
                    var message = new Java.Lang.String(((TextView)sender).Text);
                    SendMessage(message);
                }
            };

            // Initialize the send button with a listener that for click events
            sendButton = FindViewById<Button>(Resource.Id.button_send);
            sendButton.Click += delegate (object sender, EventArgs e) {
                // Send a message using content of the edit text widget
                var view = FindViewById<TextView>(Resource.Id.edit_text_out);
                var message = new Java.Lang.String(view.Text);
                SendMessage(message);
            };

            /* Initialize the AT commands buttons with a listener that for click events */

            button_atReset = FindViewById<Button>(Resource.Id.button_atReset);
            button_atReset.Click += delegate (object sender, EventArgs e) {
                SendCommand(ListOfCommands.AT_Reset);
            };
            button_atAutomaticProtocol = FindViewById<Button>(Resource.Id.button_atAutomaticProtocol);
            button_atAutomaticProtocol.Click += delegate (object sender, EventArgs e) {
                SendCommand(ListOfCommands.AT_AutomaticProtocol);
            };
            button_atReadVoltage = FindViewById<Button>(Resource.Id.button_atReadVoltage);
            button_atReadVoltage.Click += delegate (object sender, EventArgs e) {
                SendCommand(ListOfCommands.AT_ReadVoltage);
            };

            /* Initialize the OBD commands buttons with a listener that for click events */

            button_obdRPM = FindViewById<Button>(Resource.Id.button_obdRPM);
            button_obdRPM.Click += delegate (object sender, EventArgs e) {
                //SendCommand(ListOfCommands.OBD_rpmCommand);
                r = new RPMCommand();
                r.Run(connectionService, readBuf, lenghtBuf);
            };
            button_obdSpeed = FindViewById<Button>(Resource.Id.button_obdSpeed);
            button_obdSpeed.Click += delegate (object sender, EventArgs e)
            {
                SendCommand(ListOfCommands.OBD_speedCommand);
            };

            button_seeRPM = FindViewById<Button>(Resource.Id.button_seeRPM);
            button_seeRPM.Click += delegate (object sender, EventArgs e)
            {
                conversationArrayAdapter.Add(r.GetFormattedResult());
            };

            /* Initialize the buttons for periodic requests with a listener that for click events */

            button_startSendRPM = FindViewById<Button>(Resource.Id.button_startSendRPM);
            button_startSendRPM.Click += delegate (object sender, EventArgs e) {
                timer_rpm = CreateTimer(myHandler, ListOfCommands.OBD_rpmCommand, 250);
            };
            button_stopSendRPM = FindViewById<Button>(Resource.Id.button_stopSendRPM);
            button_stopSendRPM.Click += delegate (object sender, EventArgs e) {
                timer_rpm.Dispose();
            };

            button_startSendSpeed = FindViewById<Button>(Resource.Id.button_startSendSpeed);
            button_startSendSpeed.Click += delegate (object sender, EventArgs e) {
                timer_speed = CreateTimer(myHandler, ListOfCommands.OBD_speedCommand, 1000);
            };
            button_stopSendSpeed = FindViewById<Button>(Resource.Id.button_stopSendSpeed);
            button_stopSendSpeed.Click += delegate (object sender, EventArgs e) {
                timer_speed.Dispose();
            };

            // Here there are universal operations:

            // Initialize the BluetoothChatService to perform bluetooth connections
            // -> done at the beginning of this function

            // Initialize the buffer for outgoing messages
            outStringBuffer = new StringBuffer("");
            outEditText.Text = string.Empty;
        }

        /* Called by the timer delegate when timer expires */
        private void ManageTimer(object state)
        {
            PeriodicCommandRequest ts = (PeriodicCommandRequest)state;
            ts.handler.ObtainMessage(OBDConnection.TIMER_RPM_RAISED, -1, -1).SendToTarget();
        }

        /* Prepares and starts the desired timer */
        private Timer CreateTimer(Handler handler, string command, int period)
        {
            // Create the object which links timer and request
            PeriodicCommandRequest speedPeriodicRequest = new PeriodicCommandRequest(handler, command);
            // Create the delegate that invokes methods for the timer.
            TimerCallback timerDelegate = new TimerCallback(ManageTimer);
            // Create a timer that waits one second, then invokes every second.
            return new Timer(timerDelegate, speedPeriodicRequest, 1000, period);
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (Debug)
                Log.Error(TAG, "- ON PAUSE -");
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (Debug)
                Log.Error(TAG, "-- ON STOP --");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Stop the Bluetooth chat services
            if (connectionService != null)
                connectionService.Stop();

            if (Debug)
                Log.Error(TAG, "--- ON DESTROY ---");
        }

        private void EnsureDiscoverable()
        {
            if (Debug)
                Log.Debug(TAG, "ensure discoverable");

            if (bluetoothAdapter.ScanMode != ScanMode.ConnectableDiscoverable)
            {
                Intent discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
                discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, 300);
                StartActivity(discoverableIntent);
            }
        }

        /// <summary>
        /// Sends a command.
        /// </summary>
        /// <param name="cmd">
        /// The command to send
        /// </param>
        public void SendCommand(string cmd)
        {
            // adds the carriage return character
            var message = new Java.Lang.String(cmd + "\r");
            SendMessage(message);
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name='message'>
        /// A string of text to send.
        /// </param>
        private void SendMessage(Java.Lang.String message)
        {
            // Check that we're actually connected before trying anything
            if (connectionService.GetState() != OBDConnectionService.STATE_CONNECTED)
            {
                Toast.MakeText(this, Resource.String.not_connected, ToastLength.Short).Show();
                return;
            }

            // Check that there's actually something to send
            if (message.Length() > 0)
            {
                // Get the message bytes and tell the BluetoothConnectionService to write
                byte[] send = message.GetBytes();
                connectionService.Write(send);

                // Reset out string buffer to zero and clear the edit text field
                outStringBuffer.SetLength(0);
                outEditText.Text = string.Empty;
            }
            else
            {
                Toast.MakeText(this, "nothing to send", ToastLength.Short).Show();
            }
        }

        // The Handler that gets information back from the BluetoothChatService
        private class MyHandler : Handler
        {
            OBDConnection OBDConnection;

            public MyHandler(OBDConnection connection)
            {
                OBDConnection = connection;
            }

            public override void HandleMessage(Message msg)
            {
                switch (msg.What)
                {
                    /* Messages from OBDConnectionServices */
                    case MESSAGE_STATE_CHANGE:
                        switch (msg.Arg1)
                        {
                            case OBDConnectionService.STATE_CONNECTED:
                                //here you should indicate the state change in some way...
                                OBDConnection.conversationArrayAdapter.Clear();
                                break;
                            case OBDConnectionService.STATE_CONNECTING:
                                break;
                            case OBDConnectionService.STATE_LISTEN:
                            case OBDConnectionService.STATE_NONE:
                                break;
                        }
                        break;
                    case MESSAGE_WRITE:
                        byte[] writeBuf = (byte[])msg.Obj;
                        //here you can show the message you send
                        // construct a string from the buffer
                        var writeMessage = new Java.Lang.String(writeBuf);
                        OBDConnection.conversationArrayAdapter.Add("Me: " + writeMessage);
                        break;
                    case MESSAGE_READ:
                        OBDConnection.readBuf = (byte[])msg.Obj;
                        OBDConnection.lenghtBuf = msg.Arg1;
                        //here you can show the message you receive
                        // construct a string from the valid bytes in the buffer
                        var readMessage = new Java.Lang.String(OBDConnection.readBuf, 0, msg.Arg1);
                        //OBDConnection.conversationArrayAdapter.Add(OBDConnection.connectedDeviceName + ":  " + readMessage);
                        //Toast.MakeText(Application.Context, "Message received", ToastLength.Short).Show();
                        break;
                    case MESSAGE_DEVICE_NAME:
                        // save the connected device's name
                        OBDConnection.connectedDeviceName = msg.Data.GetString(DEVICE_NAME);
                        Toast.MakeText(Application.Context, "Connected to " + OBDConnection.connectedDeviceName, ToastLength.Short).Show();
                        break;
                    case MESSAGE_TOAST:
                        Toast.MakeText(Application.Context, msg.Data.GetString(TOAST), ToastLength.Short).Show();
                        break;
                    /* Messages from timer delegate */
                    case TIMER_RPM_RAISED:
                        OBDConnection.SendCommand(ListOfCommands.OBD_rpmCommand);
                        break;
                    case TIMER_SPEED_RAISED:
                        OBDConnection.SendCommand(ListOfCommands.OBD_speedCommand);
                        break;
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (Debug)
                Log.Debug(TAG, "onActivityResult " + resultCode);

            switch (requestCode)
            {
                case REQUEST_CONNECT_DEVICE:
                    // When DeviceListActivity returns with a device to connect
                    if (resultCode == Result.Ok)
                    {
                        // Get the device MAC address
                        var address = data.Extras.GetString(DeviceListActivity.EXTRA_DEVICE_ADDRESS);
                        // Get the BLuetoothDevice object
                        BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(address);
                        // Attempt to connect to the device
                        connectionService.Connect(device);
                    }
                    break;
                case REQUEST_ENABLE_BT:
                    // When the request to enable Bluetooth returns
                    if (resultCode == Result.Ok)
                    {
                        // Bluetooth is now enabled, so set up a chat session
                        SetupConnection();
                    }
                    else
                    {
                        // User did not enable Bluetooth or an error occured
                        //Log.Debug(TAG, "BT not enabled");
                        Toast.MakeText(this, Resource.String.bt_not_enabled_leaving, ToastLength.Short).Show();
                        Finish();
                    }
                    break;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.option_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.scan:
                    // Launch the DeviceListActivity to see devices and do scan
                    var serverIntent = new Intent(this, typeof(DeviceListActivity));
                    StartActivityForResult(serverIntent, REQUEST_CONNECT_DEVICE);
                    return true;
                case Resource.Id.discoverable:
                    // Ensure this device is discoverable by others
                    EnsureDiscoverable();
                    return true;
            }
            return false;
        }
    }
}