package md51e3231ad22bfbc5619bec090cfdb3a4c;


public class OBDConnectionService_ConnectedThread
	extends java.lang.Thread
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_run:()V:GetRunHandler\n" +
			"";
		mono.android.Runtime.register ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OBDConnectionService_ConnectedThread.class, __md_methods);
	}


	public OBDConnectionService_ConnectedThread () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OBDConnectionService_ConnectedThread.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public OBDConnectionService_ConnectedThread (java.lang.Runnable p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == OBDConnectionService_ConnectedThread.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public OBDConnectionService_ConnectedThread (java.lang.Runnable p0, java.lang.String p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == OBDConnectionService_ConnectedThread.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}


	public OBDConnectionService_ConnectedThread (java.lang.String p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == OBDConnectionService_ConnectedThread.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0 });
	}


	public OBDConnectionService_ConnectedThread (java.lang.ThreadGroup p0, java.lang.Runnable p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == OBDConnectionService_ConnectedThread.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.ThreadGroup, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public OBDConnectionService_ConnectedThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == OBDConnectionService_ConnectedThread.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.ThreadGroup, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public OBDConnectionService_ConnectedThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2, long p3) throws java.lang.Throwable
	{
		super (p0, p1, p2, p3);
		if (getClass () == OBDConnectionService_ConnectedThread.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.ThreadGroup, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int64, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public OBDConnectionService_ConnectedThread (java.lang.ThreadGroup p0, java.lang.String p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == OBDConnectionService_ConnectedThread.class)
			mono.android.TypeManager.Activate ("OBDConnection.OBDConnectionService+ConnectedThread, OBDConnection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.ThreadGroup, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}


	public void run ()
	{
		n_run ();
	}

	private native void n_run ();

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
