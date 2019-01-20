﻿using System;
using System.Linq;

using MsieJavaScriptEngine.Constants;
using MsieJavaScriptEngine.Extensions;
using MsieJavaScriptEngine.Helpers;
using MsieJavaScriptEngine.Resources;

using WrapperCompilationException = MsieJavaScriptEngine.JsCompilationException;
using WrapperEngineException = MsieJavaScriptEngine.JsEngineException;
using WrapperEngineLoadException = MsieJavaScriptEngine.JsEngineLoadException;
using WrapperException = MsieJavaScriptEngine.JsException;
using WrapperFatalException = MsieJavaScriptEngine.JsFatalException;
using WrapperInterruptedException = MsieJavaScriptEngine.JsInterruptedException;
using WrapperRuntimeException = MsieJavaScriptEngine.JsRuntimeException;
using WrapperScriptException = MsieJavaScriptEngine.JsScriptException;
using WrapperUsageException = MsieJavaScriptEngine.JsUsageException;

using OriginalEngineException = MsieJavaScriptEngine.JsRt.JsEngineException;
using OriginalException = MsieJavaScriptEngine.JsRt.JsException;
using OriginalFatalException = MsieJavaScriptEngine.JsRt.JsFatalException;
using OriginalScriptException = MsieJavaScriptEngine.JsRt.Edge.EdgeJsScriptException;
using OriginalUsageException = MsieJavaScriptEngine.JsRt.JsUsageException;

namespace MsieJavaScriptEngine.JsRt.Edge
{
	/// <summary>
	/// “Edge” JsRT version of Chakra JS engine
	/// </summary>
	internal sealed class ChakraEdgeJsRtJsEngine : ChakraJsRtJsEngineBase
	{
		/// <summary>
		/// Instance of JS runtime
		/// </summary>
		private EdgeJsRuntime _jsRuntime;

		/// <summary>
		/// Instance of JS context
		/// </summary>
		private EdgeJsContext _jsContext;

		/// <summary>
		/// Type mapper
		/// </summary>
		private EdgeTypeMapper _typeMapper;

		/// <summary>
		/// Flag indicating whether this JS engine is supported
		/// </summary>
		private static bool? _isSupported;

		/// <summary>
		/// Support synchronizer
		/// </summary>
		private static readonly object _supportSynchronizer = new object();


		/// <summary>
		/// Constructs an instance of the Chakra “Edge” JsRT engine
		/// </summary>
		/// <param name="settings">JS engine settings</param>
		public ChakraEdgeJsRtJsEngine(JsEngineSettings settings)
			: base(settings)
		{
			_typeMapper = new EdgeTypeMapper();

			try
			{
				_dispatcher.Invoke(() =>
				{
					_jsRuntime = CreateJsRuntime();
					_jsContext = _jsRuntime.CreateContext();
					if (_jsContext.IsValid)
					{
						_jsContext.AddRef();
					}
				});
			}
			catch (DllNotFoundException e)
			{
				throw WrapDllNotFoundException(e);
			}
			catch (Exception e)
			{
				throw JsErrorHelpers.WrapEngineLoadException(e, _engineModeName, true);
			}
			finally
			{
				if (!_jsContext.IsValid)
				{
					Dispose();
				}
			}
		}

		/// <summary>
		/// Destructs an instance of the Chakra “Edge” JsRT engine
		/// </summary>
		~ChakraEdgeJsRtJsEngine()
		{
			Dispose(false);
		}


		/// <summary>
		/// Creates a instance of JS runtime with special settings
		/// </summary>
		/// <returns>Instance of JS runtime with special settings</returns>
		private static EdgeJsRuntime CreateJsRuntime()
		{
			return EdgeJsRuntime.Create(JsRuntimeAttributes.AllowScriptInterrupt, null);
		}

		/// <summary>
		/// Checks a support of the Chakra “Edge” JsRT engine
		/// </summary>
		/// <returns>Result of check (true - supports; false - does not support)</returns>
		public static bool IsSupported()
		{
			if (_isSupported.HasValue)
			{
				return _isSupported.Value;
			}

			lock (_supportSynchronizer)
			{
				if (_isSupported.HasValue)
				{
					return _isSupported.Value;
				}

				try
				{
					using (CreateJsRuntime())
					{
						_isSupported = true;
					}
				}
				catch (DllNotFoundException e)
				{
					if (e.Message.ContainsQuotedValue(DllName.Chakra))
					{
						_isSupported = false;
					}
					else
					{
						_isSupported = null;
					}
				}
				catch
				{
					_isSupported = null;
				}

				return _isSupported.HasValue && _isSupported.Value;
			}
		}

		/// <summary>
		/// Adds a reference to the value
		/// </summary>
		/// <param name="value">The value</param>
		private static void AddReferenceToValue(EdgeJsValue value)
		{
			if (CanHaveReferences(value))
			{
				value.AddRef();
			}
		}

		/// <summary>
		/// Removes a reference to the value
		/// </summary>
		/// <param name="value">The value</param>
		private static void RemoveReferenceToValue(EdgeJsValue value)
		{
			if (CanHaveReferences(value))
			{
				value.Release();
			}
		}

		/// <summary>
		/// Checks whether the value can have references
		/// </summary>
		/// <param name="value">The value</param>
		/// <returns>Result of check (true - may have; false - may not have)</returns>
		private static bool CanHaveReferences(EdgeJsValue value)
		{
			JsValueType valueType = value.ValueType;

			switch (valueType)
			{
				case JsValueType.Null:
				case JsValueType.Undefined:
				case JsValueType.Boolean:
					return false;
				default:
					return true;
			}
		}

		/// <summary>
		/// Creates a instance of JS scope
		/// </summary>
		/// <returns>Instance of JS scope</returns>
		private EdgeJsScope CreateJsScope()
		{
			if (_jsRuntime.Disabled)
			{
				_jsRuntime.Disabled = false;
			}

			var jsScope = new EdgeJsScope(_jsContext);

			if (_settings.EnableDebugging)
			{
				StartDebugging();
			}

			return jsScope;
		}

		#region Mapping

		private WrapperException WrapJsException(OriginalException originalException,
			string defaultDocumentName = null)
		{
			WrapperException wrapperException;
			JsErrorCode errorCode = originalException.ErrorCode;
			string description = originalException.Message;
			string message = description;
			string type = string.Empty;
			string documentName = defaultDocumentName ?? string.Empty;
			int lineNumber = 0;
			int columnNumber = 0;
			string callStack = string.Empty;
			string sourceFragment = string.Empty;

			var originalScriptException = originalException as OriginalScriptException;
			if (originalScriptException != null)
			{
				EdgeJsValue errorValue = originalScriptException.Error;

				if (errorValue.IsValid)
				{
					JsValueType errorValueType = errorValue.ValueType;

					if (errorValueType == JsValueType.Error
						|| errorValueType == JsValueType.Object)
					{
						EdgeJsValue messagePropertyValue = errorValue.GetProperty("message");
						description = messagePropertyValue.ConvertToString().ToString();

						EdgeJsValue namePropertyValue = errorValue.GetProperty("name");
						type = namePropertyValue.ValueType == JsValueType.String ?
							namePropertyValue.ConvertToString().ToString() : string.Empty;

						EdgeJsPropertyId stackPropertyId = EdgeJsPropertyId.FromString("stack");
						if (errorValue.HasProperty(stackPropertyId))
						{
							EdgeJsPropertyId descriptionPropertyId = EdgeJsPropertyId.FromString("description");
							if (errorValue.HasProperty(descriptionPropertyId))
							{
								EdgeJsValue descriptionPropertyValue = errorValue.GetProperty(descriptionPropertyId);
								if (descriptionPropertyValue.ValueType == JsValueType.String
									&& descriptionPropertyValue.StringLength > 0)
								{
									description = descriptionPropertyValue.ConvertToString().ToString();
								}
							}

							EdgeJsValue stackPropertyValue = errorValue.GetProperty(stackPropertyId);
							string messageWithTypeAndCallStack = stackPropertyValue.ValueType == JsValueType.String ?
								stackPropertyValue.ConvertToString().ToString() : string.Empty;
							string messageWithType = errorValue.ConvertToString().ToString();
							string rawCallStack = messageWithTypeAndCallStack
								.TrimStart(messageWithType)
								.TrimStart("Error")
								.TrimStart(new char[] { '\n', '\r' })
								;

							CallStackItem[] callStackItems = JsErrorHelpers.ParseCallStack(rawCallStack);
							if (callStackItems.Length > 0)
							{
								CallStackItem firstCallStackItem = callStackItems[0];
								if (firstCallStackItem.DocumentName.Length > 0)
								{
									documentName = firstCallStackItem.DocumentName;
								}
								lineNumber = firstCallStackItem.LineNumber;
								columnNumber = firstCallStackItem.ColumnNumber;
								callStack = JsErrorHelpers.StringifyCallStackItems(callStackItems);
							}

							message = JsErrorHelpers.GenerateScriptErrorMessage(type, description, callStack);
						}
						else
						{
							EdgeJsPropertyId urlPropertyId = EdgeJsPropertyId.FromString("url");
							if (errorValue.HasProperty(urlPropertyId))
							{
								EdgeJsValue urlPropertyValue = errorValue.GetProperty(urlPropertyId);
								documentName = urlPropertyValue.ConvertToString().ToString();
							}

							EdgeJsPropertyId linePropertyId = EdgeJsPropertyId.FromString("line");
							if (errorValue.HasProperty(linePropertyId))
							{
								EdgeJsValue linePropertyValue = errorValue.GetProperty(linePropertyId);
								lineNumber = linePropertyValue.ConvertToNumber().ToInt32() + 1;
							}

							EdgeJsPropertyId columnPropertyId = EdgeJsPropertyId.FromString("column");
							if (errorValue.HasProperty(columnPropertyId))
							{
								EdgeJsValue columnPropertyValue = errorValue.GetProperty(columnPropertyId);
								columnNumber = columnPropertyValue.ConvertToNumber().ToInt32() + 1;
							}

							string sourceLine = string.Empty;
							EdgeJsPropertyId sourcePropertyId = EdgeJsPropertyId.FromString("source");
							if (errorValue.HasProperty(sourcePropertyId))
							{
								EdgeJsValue sourcePropertyValue = errorValue.GetProperty(sourcePropertyId);
								sourceLine = sourcePropertyValue.ConvertToString().ToString();
							}

							sourceFragment = TextHelpers.GetTextFragmentFromLine(sourceLine, columnNumber);
							message = JsErrorHelpers.GenerateScriptErrorMessage(type, description, documentName,
								lineNumber, columnNumber, sourceFragment);
						}
					}
					else
					{
						message = errorValue.ConvertToString().ToString();
						description = message;
					}
				}

				WrapperScriptException wrapperScriptException;
				if (errorCode == JsErrorCode.ScriptCompile)
				{
					wrapperScriptException = new WrapperCompilationException(message, _engineModeName,
						originalScriptException);
				}
				else if (errorCode == JsErrorCode.ScriptTerminated)
				{
					wrapperScriptException = new WrapperInterruptedException(CommonStrings.Runtime_ScriptInterrupted,
						_engineModeName, originalScriptException);
				}
				else
				{
					wrapperScriptException = new WrapperRuntimeException(message, _engineModeName,
						originalScriptException)
					{
						CallStack = callStack
					};
				}
				wrapperScriptException.Type = type;
				wrapperScriptException.DocumentName = documentName;
				wrapperScriptException.LineNumber = lineNumber;
				wrapperScriptException.ColumnNumber = columnNumber;
				wrapperScriptException.SourceFragment = sourceFragment;

				wrapperException = wrapperScriptException;
			}
			else
			{
				if (originalException is OriginalUsageException)
				{
					wrapperException = new WrapperUsageException(message, _engineModeName, originalException);
				}
				else if (originalException is OriginalEngineException)
				{
					wrapperException = new WrapperEngineException(message, _engineModeName, originalException);
				}
				else if (originalException is OriginalFatalException)
				{
					wrapperException = new WrapperFatalException(message, _engineModeName, originalException);
				}
				else
				{
					wrapperException = new WrapperException(message, _engineModeName, originalException);
				}
			}

			wrapperException.Description = description;

			return wrapperException;
		}

		private WrapperEngineLoadException WrapDllNotFoundException(
			DllNotFoundException originalDllNotFoundException)
		{
			string originalMessage = originalDllNotFoundException.Message;
			string description;
			string message;

			if (originalMessage.ContainsQuotedValue(DllName.Chakra))
			{
				description = string.Format(CommonStrings.Engine_AssemblyNotRegistered, DllName.Chakra) +
					" " +
					string.Format(CommonStrings.Engine_EdgeInstallationRequired)
					;
				message = JsErrorHelpers.GenerateEngineLoadErrorMessage(description, _engineModeName);
			}
			else
			{
				description = originalMessage;
				message = JsErrorHelpers.GenerateEngineLoadErrorMessage(description, _engineModeName, true);
			}

			var wrapperEngineLoadException = new WrapperEngineLoadException(message, _engineModeName,
				originalDllNotFoundException)
			{
				Description = description
			};

			return wrapperEngineLoadException;
		}

		#endregion

		#region ChakraJsRtJsEngineBase overrides

		protected override void InnerStartDebugging()
		{
			EdgeJsContext.StartDebugging();
		}

		#region IInnerJsEngine implementation

		public override bool SupportsScriptPrecompilation
		{
			get { return true; }
		}


		public override PrecompiledScript Precompile(string code, string documentName)
		{
			PrecompiledScript precompiledScript = _dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						byte[] cachedBytes = EdgeJsContext.SerializeScript(code);

						return new PrecompiledScript(_engineModeName, code, cachedBytes, documentName);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e, documentName);
					}
				}
			});

			return precompiledScript;
		}

		public override object Evaluate(string expression, string documentName)
		{
			object result = _dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsValue resultValue = EdgeJsContext.RunScript(expression, _jsSourceContext++,
							documentName);

						return _typeMapper.MapToHostType(resultValue);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});

			return result;
		}

		public override void Execute(string code, string documentName)
		{
			_dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsContext.RunScript(code, _jsSourceContext++, documentName);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void Execute(PrecompiledScript precompiledScript)
		{
			_dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsContext.RunSerializedScript(precompiledScript.Code, precompiledScript.CachedBytes,
							_jsSourceContext++, precompiledScript.DocumentName);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
					finally
					{
						GC.KeepAlive(precompiledScript);
					}
				}
			});
		}

		public override object CallFunction(string functionName, params object[] args)
		{
			object result = _dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsValue globalObj = EdgeJsValue.GlobalObject;
						EdgeJsPropertyId functionId = EdgeJsPropertyId.FromString(functionName);

						bool functionExist = globalObj.HasProperty(functionId);
						if (!functionExist)
						{
							throw new WrapperRuntimeException(
								string.Format(CommonStrings.Runtime_FunctionNotExist, functionName),
								_engineModeName
							);
						}

						EdgeJsValue resultValue;
						EdgeJsValue functionValue = globalObj.GetProperty(functionId);

						if (args.Length > 0)
						{
							EdgeJsValue[] processedArgs = _typeMapper.MapToScriptType(args);

							foreach (EdgeJsValue processedArg in processedArgs)
							{
								AddReferenceToValue(processedArg);
							}

							EdgeJsValue[] allProcessedArgs = new[] { globalObj }
								.Concat(processedArgs)
								.ToArray()
								;

							try
							{
								resultValue = functionValue.CallFunction(allProcessedArgs);
							}
							finally
							{
								foreach (EdgeJsValue processedArg in processedArgs)
								{
									RemoveReferenceToValue(processedArg);
								}
							}
						}
						else
						{
							resultValue = functionValue.CallFunction(globalObj);
						}

						return _typeMapper.MapToHostType(resultValue);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});

			return result;
		}

		public override bool HasVariable(string variableName)
		{
			bool result = _dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsValue globalObj = EdgeJsValue.GlobalObject;
						EdgeJsPropertyId variableId = EdgeJsPropertyId.FromString(variableName);
						bool variableExist = globalObj.HasProperty(variableId);

						if (variableExist)
						{
							EdgeJsValue variableValue = globalObj.GetProperty(variableId);
							variableExist = variableValue.ValueType != JsValueType.Undefined;
						}

						return variableExist;
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});

			return result;
		}

		public override object GetVariableValue(string variableName)
		{
			object result = _dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsValue variableValue = EdgeJsValue.GlobalObject.GetProperty(variableName);

						return _typeMapper.MapToHostType(variableValue);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});

			return result;
		}

		public override void SetVariableValue(string variableName, object value)
		{
			_dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsValue inputValue = _typeMapper.MapToScriptType(value);
						AddReferenceToValue(inputValue);

						try
						{
							EdgeJsValue.GlobalObject.SetProperty(variableName, inputValue, true);
						}
						finally
						{
							RemoveReferenceToValue(inputValue);
						}
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void RemoveVariable(string variableName)
		{
			_dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsValue globalObj = EdgeJsValue.GlobalObject;
						EdgeJsPropertyId variableId = EdgeJsPropertyId.FromString(variableName);

						if (globalObj.HasProperty(variableId))
						{
							globalObj.SetProperty(variableId, EdgeJsValue.Undefined, true);
						}
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void EmbedHostObject(string itemName, object value)
		{
			_dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsValue processedValue = _typeMapper.GetOrCreateScriptObject(value);
						EdgeJsValue.GlobalObject.SetProperty(itemName, processedValue, true);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void EmbedHostType(string itemName, Type type)
		{
			_dispatcher.Invoke(() =>
			{
				using (CreateJsScope())
				{
					try
					{
						EdgeJsValue typeValue = _typeMapper.GetOrCreateScriptType(type);
						EdgeJsValue.GlobalObject.SetProperty(itemName, typeValue, true);
					}
					catch (OriginalException e)
					{
						throw WrapJsException(e);
					}
				}
			});
		}

		public override void Interrupt()
		{
			_jsRuntime.Disabled = true;
		}

		public override void CollectGarbage()
		{
			_jsRuntime.CollectGarbage();
		}

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public override void Dispose()
		{
			Dispose(true /* disposing */);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Destroys object
		/// </summary>
		/// <param name="disposing">Flag, allowing destruction of
		/// managed objects contained in fields of class</param>
		private void Dispose(bool disposing)
		{
			if (_disposedFlag.Set())
			{
				if (disposing)
				{
					if (_dispatcher != null)
					{
						_dispatcher.Invoke(DisposeUnmanagedResources);

						_dispatcher.Dispose();
						_dispatcher = null;
					}

					if (_typeMapper != null)
					{
						_typeMapper.Dispose();
						_typeMapper = null;
					}
				}
				else
				{
					DisposeUnmanagedResources();
				}
			}
		}

		private void DisposeUnmanagedResources()
		{
			if (_jsContext.IsValid)
			{
				_jsContext.Release();
			}
			_jsRuntime.Dispose();
		}

		#endregion

		#endregion
	}
}