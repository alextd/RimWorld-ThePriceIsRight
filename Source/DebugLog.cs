using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Price_Is_Right
{
	static class Log
	{
		[System.Diagnostics.Conditional("DEBUG")]
		public static void Message(string x)
		{
			Verse.Log.Message(x);
		}
	}
}
