﻿using System;
using System.Linq;

namespace MsieJavaScriptEngine.Utilities
{
	/// <summary>
	/// Extensions for String
	/// </summary>
	internal static class StringExtensions
	{
		/// <summary>
		/// Removes leading occurrence of the specified string from the current <see cref="String"/> object
		/// </summary>
		/// <param name="source">Instance of <see cref="String"/></param>
		/// <param name="trimString">An string to remove</param>
		/// <returns>The string that remains after removing of the specified string from the start of
		/// the current string</returns>
		public static string TrimStart(this string source, string trimString)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (trimString == null)
			{
				throw new ArgumentNullException(nameof(trimString));
			}

			if (source.Length == 0 || trimString.Length == 0)
			{
				return source;
			}

			string result = source;
			if (source.StartsWith(trimString, StringComparison.Ordinal))
			{
				result = source.Substring(trimString.Length);
			}

			return result;
		}

		/// <summary>
		/// Converts a first letter of string to capital
		/// </summary>
		/// <param name="source">Instance of <see cref="String"/></param>
		/// <returns>The string starting with a capital letter</returns>
		public static string CapitalizeFirstLetter(this string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			int length = source.Length;
			if (length == 0)
			{
				return source;
			}

			string result;
			char firstCharacter = source.First();

			if (char.IsLower(firstCharacter))
			{
				result = char.ToUpperInvariant(firstCharacter).ToString();
				if (length > 1)
				{
					result += source.Substring(1);
				}
			}
			else
			{
				result = source;
			}

			return result;
		}

		/// <summary>
		/// Splits a string into lines
		/// </summary>
		/// <param name="source">Instance of <see cref="String"/></param>
		/// <returns>An array of lines</returns>
		public static string[] SplitToLines(this string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			string[] result = source.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			return result;
		}
	}
}