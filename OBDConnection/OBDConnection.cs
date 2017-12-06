using Android.App;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using Android.Content;
using Java.Lang;
using Android.Views;
using Android.Views.InputMethods;
using System;

namespace OBDConnection
{
    [Activity(Label = "OBDConnection", MainLauncher = true, Icon = "@drawable/icon")]
    class OBDConnection : Activity
    {
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

        // Array adapter for the conversation thread
        protected ArrayAdapter<string> conversationArrayAdapter;

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

            // Here there are universal operations:

            // Initialize the BluetoothChatService to perform bluetooth connections
            connectionService = new OBDConnectionService(this, new MyHandler(this));

            // Initialize the buffer for outgoing messages
            outStringBuffer = new StringBuffer("");
            outEditText.Text = string.Empty;
        }

        protected override void OnPause()
        {
            base.OnPause();

            //if (Debug)
            //    Log.Error(TAG, "- ON PAUSE -");
        }

        protected override void OnStop()
        {
            base.OnStop();

            //if (Debug)
            //    Log.Error(TAG, "-- ON STOP --");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Stop the Bluetooth chat services
            if (connectionService != null)
                connectionService.Stop();

            //if (Debug)
            //    Log.Error(TAG, "--- ON DESTROY ---");
        }

        private void EnsureDiscoverable()
        {
            //if (Debug)
            //    Log.Debug(TAG, "ensure discoverable");

            if (bluetoothAdapter.ScanMode != ScanMode.ConnectableDiscoverable)
            {
                Intent discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
                discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, 300);
                StartActivity(discoverableIntent);
            }
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
                Toast.MakeText(this, "not_connected", ToastLength.Short).Show();
                return;
            }

            // Check that there's actually something to send
            if (message.Length() > 0)
            {
                // Get the message bytes and tell the BluetoothChatService to write
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
                        byte[] readBuf = (byte[])msg.Obj;
                        //here you can show the message you receive
                        // construct a string from the valid bytes in the buffer
                        var readMessage = new Java.Lang.String(readBuf, 0, msg.Arg1);
                        OBDConnection.conversationArrayAdapter.Add(OBDConnection.connectedDeviceName + ":  " + readMessage);
                        Toast.MakeText(Application.Context, "received", ToastLength.Short).Show();
                        break;
                    case MESSAGE_DEVICE_NAME:
                        // save the connected device's name
                        OBDConnection.connectedDeviceName = msg.Data.GetString(DEVICE_NAME);
                        Toast.MakeText(Application.Context, "Connected to " + OBDConnection.connectedDeviceName, ToastLength.Short).Show();
                        break;
                    case MESSAGE_TOAST:
                        Toast.MakeText(Application.Context, msg.Data.GetString(TOAST), ToastLength.Short).Show();
                        break;
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            //if (Debug)
            //    Log.Debug(TAG, "onActivityResult " + resultCode);

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
                        Toast.MakeText(this, "bt_not_enabled_leaving", ToastLength.Short).Show();
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