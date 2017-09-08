﻿#if !NETSTANDARD1_3
using System.Runtime.InteropServices;

namespace MsieJavaScriptEngine.ActiveScript
{
	/// <summary>
	/// The <see cref="IActiveScriptSiteInterruptPoll"/> interface allows a host to specify
	/// that ascript should terminate
	/// </summary>
	[ComImport]
	[Guid("539698a0-cdca-11cf-a5eb-00aa0047a063")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptSiteInterruptPoll
	{
		/// <summary>
		/// Allows a host to specify that a script should terminate
		/// </summary>
		/// <returns>The method returns an HRESULT</returns>
		[PreserveSig]
		uint QueryContinue();
	}
}
#endif