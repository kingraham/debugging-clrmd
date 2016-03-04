using Microsoft.Diagnostics.Runtime;
using System;

namespace CLRProfiler.Model
{
	public class StackObject
	{
		public ulong Pointer { get; set; }
		public ulong Object { get; set; }
		public string Name { get; set; }
		public ClrType Type { get; set; }

		public StackObject(ulong pointer, ulong obj, string name, ClrType type)
		{
			this.Pointer = pointer;
			this.Object = obj;
			this.Name = name;
			this.Type = type;
		}
	}
}