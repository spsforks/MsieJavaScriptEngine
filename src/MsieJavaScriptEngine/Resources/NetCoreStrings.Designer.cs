//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated by a tool.
//
//	 Changes to this file may cause incorrect behavior and will be lost if
//	 the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace MsieJavaScriptEngine.Resources
{
	using System;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;

	/// <summary>
	/// A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	public class NetCoreStrings
	{
		private static Lazy<ResourceManager> _resourceManager =
			new Lazy<ResourceManager>(() => new ResourceManager(
				"MsieJavaScriptEngine.Resources.NetCoreStrings",
#if NET40
				typeof(NetCoreStrings).Assembly
#else
				typeof(NetCoreStrings).GetTypeInfo().Assembly
#endif
			));

		private static CultureInfo _resourceCulture;

		/// <summary>
		/// Returns a cached ResourceManager instance used by this class
		/// </summary>
		public static ResourceManager ResourceManager
		{
			get
			{
				return _resourceManager.Value;
			}
		}

		/// <summary>
		/// Overrides a current thread's CurrentUICulture property for all
		/// resource lookups using this strongly typed resource class
		/// </summary>
		public static CultureInfo Culture
		{
			get
			{
				return _resourceCulture;
			}
			set
			{
				_resourceCulture = value;
			}
		}

		/// <summary>
		/// Looks up a localized string similar to "During invocation of the host delegate an error has occurred - “{0}”."
		/// </summary>
		public static string Runtime_HostDelegateInvocationFailed
		{
			get { return GetString("Runtime_HostDelegateInvocationFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During getting value of '{0}' field of the host object an error has occurred - “{1}”."
		/// </summary>
		public static string Runtime_HostObjectFieldGettingFailed
		{
			get { return GetString("Runtime_HostObjectFieldGettingFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During setting value of '{0}' field of the host object an error has occurred - “{1}”."
		/// </summary>
		public static string Runtime_HostObjectFieldSettingFailed
		{
			get { return GetString("Runtime_HostObjectFieldSettingFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During invocation of '{0}' method of the host object an error has occurred - “{1}”."
		/// </summary>
		public static string Runtime_HostObjectMethodInvocationFailed
		{
			get { return GetString("Runtime_HostObjectMethodInvocationFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During getting value of '{0}' property of the host object an error has occurred - “{1}”."
		/// </summary>
		public static string Runtime_HostObjectPropertyGettingFailed
		{
			get { return GetString("Runtime_HostObjectPropertyGettingFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During setting value of '{0}' property of the host object an error has occurred - “{1}”."
		/// </summary>
		public static string Runtime_HostObjectPropertySettingFailed
		{
			get { return GetString("Runtime_HostObjectPropertySettingFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During invocation of constructor of the `{0}` host type an error has occurred - “{1}”."
		/// </summary>
		public static string Runtime_HostTypeConstructorInvocationFailed
		{
			get { return GetString("Runtime_HostTypeConstructorInvocationFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Could not create instance of the `{0}` host type, because it does not have any public constructor."
		/// </summary>
		public static string Runtime_HostTypeConstructorNotFound
		{
			get { return GetString("Runtime_HostTypeConstructorNotFound"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During getting value of '{0}' field of the `{1}` host type an error has occurred - “{2}”."
		/// </summary>
		public static string Runtime_HostTypeFieldGettingFailed
		{
			get { return GetString("Runtime_HostTypeFieldGettingFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During setting value of '{0}' field of the `{1}` host type an error has occurred - “{2}”."
		/// </summary>
		public static string Runtime_HostTypeFieldSettingFailed
		{
			get { return GetString("Runtime_HostTypeFieldSettingFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During invocation of '{0}' method of the `{1}` host type an error has occurred - “{2}”."
		/// </summary>
		public static string Runtime_HostTypeMethodInvocationFailed
		{
			get { return GetString("Runtime_HostTypeMethodInvocationFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During getting value of '{0}' property of the `{1}` host type an error has occurred - “{2}”."
		/// </summary>
		public static string Runtime_HostTypePropertyGettingFailed
		{
			get { return GetString("Runtime_HostTypePropertyGettingFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During setting value of '{0}' property of the host type `{1}` an error has occurred - “{2}”."
		/// </summary>
		public static string Runtime_HostTypePropertySettingFailed
		{
			get { return GetString("Runtime_HostTypePropertySettingFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Could not retrieve field '{0}' of the host object, because there was an invalid `this` context."
		/// </summary>
		public static string Runtime_InvalidThisContextForHostObjectField
		{
			get { return GetString("Runtime_InvalidThisContextForHostObjectField"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Could not call method '{0}' of the host object, because there was an invalid `this` context."
		/// </summary>
		public static string Runtime_InvalidThisContextForHostObjectMethod
		{
			get { return GetString("Runtime_InvalidThisContextForHostObjectMethod"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Could not retrieve property '{0}' of the host object, because there was an invalid `this` context."
		/// </summary>
		public static string Runtime_InvalidThisContextForHostObjectProperty
		{
			get { return GetString("Runtime_InvalidThisContextForHostObjectProperty"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The '{0}' mode of JavaScript engine is not compatible with .NET Core."
		/// </summary>
		public static string Runtime_JsEngineModeNotCompatibleWithNetCore
		{
			get { return GetString("Runtime_JsEngineModeNotCompatibleWithNetCore"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Could not found none of the JavaScript engines, which would be compatible with .NET Core. Perhaps..."
		/// </summary>
		public static string Runtime_JsEnginesNotFound
		{
			get { return GetString("Runtime_JsEnginesNotFound"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Could not find suitable constructor or not enough arguments to invoke of constructor of the `{0}`..."
		/// </summary>
		public static string Runtime_SuitableConstructorOfHostTypeNotFound
		{
			get { return GetString("Runtime_SuitableConstructorOfHostTypeNotFound"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Could not find suitable method or not enough arguments to invoke of '{0}' method of the host object."
		/// </summary>
		public static string Runtime_SuitableMethodOfHostObjectNotFound
		{
			get { return GetString("Runtime_SuitableMethodOfHostObjectNotFound"); }
		}

			private static string GetString(string name)
			{
				string value = ResourceManager.GetString(name, _resourceCulture);

				return value;
			}
		}
	}