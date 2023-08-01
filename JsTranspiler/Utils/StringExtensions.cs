using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsTranspiler.Utils
{
	public static class StringExtensions
	{
		public static bool IsAlphaNumeric(this string @this)
		{
			return !Regex.IsMatch(@this, "[^a-zA-Z0-9_$]");
		}

		public static bool IsAlphaNumeric(this char @this)
		{
			return !Regex.IsMatch(@this.ToString(), "[^a-zA-Z0-9_$]");
		}
	}
}
