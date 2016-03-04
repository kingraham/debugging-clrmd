using Microsoft.Diagnostics.Runtime;
using System;
using System.Diagnostics;

namespace ClrProfilerConsole
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			String procToFind = "NCalcExample.vshost";
			int id = -1;

			foreach (var p in System.Diagnostics.Process.GetProcesses())
			{
				if (p.ProcessName.Equals(procToFind, StringComparison.CurrentCultureIgnoreCase))
					id = p.Id;
			}

			Console.WriteLine("Pid is {0}", id);

			if (id >= 0)
			{
				using (DataTarget dataTarget = DataTarget.AttachToProcess(id, 3000, AttachFlag.Passive))
				{
					var runtime = CreateRuntime(dataTarget);

					// dump domains
					foreach (var appDomain in runtime.AppDomains)
					{
						Console.WriteLine("App Domain: {0} ", appDomain.Id);
						Console.WriteLine("Name: {0}", appDomain.Name);
						Console.WriteLine("Address: {0}", appDomain.Address);
						Console.WriteLine("Application Base: {0}", appDomain.ApplicationBase);
						Console.WriteLine("Configuration File: {0}", appDomain.ConfigurationFile);						
						Console.WriteLine("Modules: ");
						Console.WriteLine(new String('-', 24));
						foreach (var module in appDomain.Modules)
						{
							Console.WriteLine("Id: {0}", module.AssemblyId);
							Console.WriteLine("Name: {0}", module.AssemblyName);
							if (module.IsDynamic)
							{
								Console.WriteLine("Is Dynamic");
							}
							else
							{
								Console.WriteLine("Filename: {0}", module.FileName);
								Console.WriteLine("Size: {0}", module.Size);
							}

							if (module.Pdb != null)
							{
								Console.WriteLine();
								Console.WriteLine("PDB Info :");
								Console.WriteLine("ID: {0}", module.Pdb.Guid);
								Console.WriteLine("Filename: {0}", module.Pdb.FileName);
								Console.WriteLine("Revision: {0}", module.Pdb.Revision);
							}
							Console.WriteLine(new String('-', 24));
						}
					}

					Console.WriteLine();
					foreach (ClrThread thread in runtime.Threads)
					{
						// The ClrRuntime.Threads will also report threads which have recently died, but their
						// underlying datastructures have not yet been cleaned up.  This can potentially be
						// useful in debugging (!threads displays this information with XXX displayed for their
						// OS thread id).  You cannot walk the stack of these threads though, so we skip them
						// here.
						if (!thread.IsAlive)
							continue;


						Console.WriteLine("Thread {0:X}:", thread.OSThreadId);
						Console.WriteLine("Stack: {0:X} - {1:X}", thread.StackBase, thread.StackLimit);
						Console.ReadLine();

						// Each thread tracks a "last thrown exception".  This is the exception object which
						// !threads prints.  If that exception object is present, we will display some basic
						// exception data here.  Note that you can get the stack trace of the exception with
						// ClrHeapException.StackTrace (we don't do that here).
						ClrException exception = thread.CurrentException;
						if (exception != null)
							Console.WriteLine("Exception: {0:X} ({1}), HRESULT={2:X}", exception.Address, exception.Type.Name, exception.HResult);

						// Walk the stack of the thread and print output similar to !ClrStack.
						Console.WriteLine();
						Console.WriteLine("Managed Callstack:");
						foreach (ClrStackFrame frame in thread.StackTrace)
						{
							// Note that CLRStackFrame currently only has three pieces of data: stack pointer,
							// instruction pointer, and frame name (which comes from ToString).  Future
							// versions of this API will allow you to get the type/function/module of the
							// method (instead of just the name).  This is not yet implemented.
							Console.WriteLine("{0,16:X} {1,16:X} {2}", frame.StackPointer, frame.InstructionPointer, frame.DisplayString);
						}

						// Print a !DumpStackObjects equivalent.

						// We'll need heap data to find objects on the stack.
						ClrHeap heap = runtime.GetHeap();

						// Walk each pointer aligned address on the stack.  Note that StackBase/StackLimit
						// is exactly what they are in the TEB.  This means StackBase > StackLimit on AMD64.
						ulong start = thread.StackBase;
						ulong stop = thread.StackLimit;

						// We'll walk these in pointer order.
						if (start > stop)
						{
							ulong tmp = start;
							start = stop;
							stop = tmp;
						}

						Console.WriteLine();
						Console.WriteLine("Stack objects:");

						// Walk each pointer aligned address.  Ptr is a stack address.
						for (ulong ptr = start; ptr <= stop; ptr += (ulong)runtime.PointerSize)
						{
							// Read the value of this pointer.  If we fail to read the memory, break.  The
							// stack region should be in the crash dump.
							ulong obj;
							if (!runtime.ReadPointer(ptr, out obj))
								break;

							// 003DF2A4 
							// We check to see if this address is a valid object by simply calling
							// GetObjectType.  If that returns null, it's not an object.
							try
							{
								ClrType type = heap.GetObjectType(obj);
								if (type == null)
									continue;

								// Don't print out free objects as there tends to be a lot of them on
								// the stack.
								if (!type.IsFree)
								{
									Console.WriteLine("{0,16:X} {1,16:X} {2}", ptr, obj, type.Name);

									if (type.Fields.Count > 0)
									{
										foreach (var field in type.Fields)
										{
											Console.WriteLine("{0} - {1}", field.Name, field.GetValue(obj));
										}
									}
								}
							}
							catch { }
						}

					}
				}
			}	
		}

		// https://github.com/Microsoft/dotnetsamples/blob/master/Microsoft.Diagnostics.Runtime/CLRMD/ClrStack/Program.cs
		private static ClrRuntime CreateRuntime(DataTarget dataTarget)
		{			
			// Now check bitness of our program/target:
			bool isTarget64Bit = dataTarget.PointerSize == 8;
			if (Environment.Is64BitProcess != isTarget64Bit)
				throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));

			// does not handle SxS scenarios.
			ClrInfo version = dataTarget.ClrVersions[0];
			return version.CreateRuntime();
		}
	}
}