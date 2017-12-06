package md51e3231ad22bfbc5619bec090cfdb3a4c;


public class OBDConnection_MyHandler
	extends android.os.Handler
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_handleMessage:(Landroid/os/Message;)V:GetHandleMessage_Landroid_os_Message_Handler\n" +
			"";
		mono.android.Runtime.register ("OBDConnection.OBDConnection+MyHandler, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OBDConnection_MyHandler.class, __md_methods);
	}


	public OBDConnection_MyHandler () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OBDConnection_MyHandler.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnection+MyHandler, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public OBDConnection_MyHandler (android.os.Handler.Callback p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == OBDConnection_MyHandler.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnection+MyHandler, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.OS.Handler+ICallback, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public OBDConnection_MyHandler (android.os.Looper p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == OBDConnection_MyHandler.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnection+MyHandler, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.OS.Looper, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public OBDConnection_MyHandler (android.os.Looper p0, android.os.Handler.Callback p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == OBDConnection_MyHandler.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnection+MyHandler, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.OS.Looper, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.OS.Handler+ICallback, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}

	public OBDConnection_MyHandler (md51e3231ad22bfbc5619bec090cfdb3a4c.OBDConnection p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == OBDConnection_MyHandler.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnection+MyHandler, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "OBDConnection.OBDConnection, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void handleMessage (android.os.Message p0)
	{
		n_handleMessage (p0);
	}

	private native void n_handleMessage (android.os.Message p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
