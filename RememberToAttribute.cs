using System;

namespace AnalyzerTools.RememberToDo
{
	[AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
	public sealed class RememberToAttribute : Attribute
	{
		public string Message { get; }
		public bool EmitOnDebug { get; }

		public RememberToAttribute (string message, bool emitOnDebug = false)
		{
			Message = message;
			EmitOnDebug = emitOnDebug;
		}
	}
}
