package md567801e8983f40f5b4eb7852e7805aef4;


public class CustomTabActivityHelper
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Android.Support.CustomTabs.Chromium.SharedUtilities.CustomTabActivityHelper, Xamarin.Auth, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null", CustomTabActivityHelper.class, __md_methods);
	}


	public CustomTabActivityHelper () throws java.lang.Throwable
	{
		super ();
		if (getClass () == CustomTabActivityHelper.class)
			mono.android.TypeManager.Activate ("Android.Support.CustomTabs.Chromium.SharedUtilities.CustomTabActivityHelper, Xamarin.Auth, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
