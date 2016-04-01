using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace MVVMLightILWeaver
{
	class Program
	{
		static void Main(string[] args)
		{
			String pathToCLRProfiler = @"CLRProfiler.exe";
			if (args.Length > 0)
				pathToCLRProfiler = args[0];

			if (!File.Exists(pathToCLRProfiler))
			{
				Console.Error.WriteLine("Could not find path to CLRProfiler!");
				return;
			}

			String workingFileName = pathToCLRProfiler + ".wrk";

			File.Copy(pathToCLRProfiler, workingFileName, true);

			var assembly = AssemblyDefinition.ReadAssembly(workingFileName, new ReaderParameters() { ReadSymbols = true, SymbolStream = new FileStream("CLRProfiler.pdb", FileMode.Open) });

			// References to external functions and types.
			MethodReference setMethodReference = null;
			MethodReference raisePropertyChangedReference = null;
			MethodReference raiseCanExecuteChangedReference = null;
			MethodReference setObjectReference = null;
			TypeReference relayCommandReference = null;

			// prior to weaving anything, we need to resolve references to external types.
			foreach (var type in assembly.MainModule.GetTypeReferences())
			{
				if (type.Name == "ObservableObject")
				{
					TypeDefinition td = type.Resolve();					
					setMethodReference = td.Methods.Where(method => method.FullName == "System.Boolean GalaSoft.MvvmLight.ObservableObject::Set(T&,T,System.String)").FirstOrDefault();
					raisePropertyChangedReference = td.Methods.Where(method => method.FullName == "System.Void GalaSoft.MvvmLight.ObservableObject::RaisePropertyChanged(System.String)").FirstOrDefault();
				}
				else if (type.Name == "RelayCommand")
				{
					TypeDefinition td = type.Resolve();
					relayCommandReference = type;
					raiseCanExecuteChangedReference = td.Methods.Where(method => method.Name == "RaiseCanExecuteChanged").FirstOrDefault();					
				}
				
				if (setMethodReference != null && raisePropertyChangedReference != null && raiseCanExecuteChangedReference != null && relayCommandReference != null)
					break;
			}

			setObjectReference = assembly.MainModule.Types.Where(t => t.Name == "PropertyViewViewModel").First().Methods.Where(m => m.Name == "SetObject").FirstOrDefault();

			if (setMethodReference == null || raisePropertyChangedReference == null || raiseCanExecuteChangedReference == null || relayCommandReference == null)
			{
				Console.Error.WriteLine("Could not resolve needed MvvmLight references");
				return;
			}
			
			foreach (var type in assembly.MainModule.Types)
			{
				if (type.HasCustomAttributes)
				{
					foreach (var ca in type.CustomAttributes)
					{
						if (ca.AttributeType.Name == "WeaveRaisePropertyChanged")  // type needs some IL Weaving going on.
						{
							foreach (var prop in type.Properties.Where(prop => !prop.PropertyType.FullName.Contains("ICommand")))
							{
								if (prop.SetMethod.HasCustomAttributes)
								{
									foreach (var propa in prop.SetMethod.CustomAttributes)
									{
										if (propa.AttributeType.Name == "CompilerGeneratedAttribute") // this is compiler generated.
										{
											// find our backing field for this property.
											var backingField = type.Fields.Where(f => f.Name.Contains("<" + prop.Name + ">k__BackingField")).FirstOrDefault();

											if (backingField != null)
											{
												var proc = prop.SetMethod.Body.GetILProcessor();
												prop.SetMethod.Body.Instructions.Clear();
												proc.Append(proc.Create(OpCodes.Nop));
												proc.Append(proc.Create(OpCodes.Ldarg_0));
												proc.Append(proc.Create(OpCodes.Ldarg_0));
												proc.Append(proc.Create(OpCodes.Ldflda, backingField));
												proc.Append(proc.Create(OpCodes.Ldarg_1));
												proc.Append(proc.Create(OpCodes.Ldstr, prop.Name));

												var genInstance = new GenericInstanceMethod(setMethodReference);
												genInstance.GenericArguments.Add(prop.PropertyType);
												proc.Append(proc.Create(OpCodes.Call, type.Module.Import(genInstance)));
												proc.Append(proc.Create(OpCodes.Pop));

												if (prop.HasCustomAttributes)
												{
													foreach (var pca in prop.CustomAttributes)
													{
														if (pca.AttributeType.Name == "RaiseCanExecuteDependency")
														{
															var typeResolve = type.Resolve();
															MethodReference mr = null;
															foreach (var conArg in ((CustomAttributeArgument[])pca.ConstructorArguments.ToArray()[0].Value))
															{
																String commandToFind = conArg.Value.ToString();
																mr = (from t in typeResolve.Methods where t.Name.Contains("get_" + commandToFind) select t).FirstOrDefault();

																if (mr != null)
																{
																	proc.Append(proc.Create(OpCodes.Ldarg_0));
																	proc.Append(proc.Create(OpCodes.Call, mr));
																	proc.Append(proc.Create(OpCodes.Castclass, relayCommandReference));
																	proc.Append(proc.Create(OpCodes.Callvirt, type.Module.Import(raiseCanExecuteChangedReference)));
																	proc.Append(proc.Create(OpCodes.Nop));
																}
															}
														}
														/* 
														 * IL_0015: ldarg.0
															IL_0016: call instance class CLRProfiler.ViewModel.PropertyViewViewModel CLRProfiler.ViewModel.AppDomainListViewModel::get_SelectedAppDomain()
															IL_001b: ldarg.0
															IL_001c: ldfld class [Microsoft.Diagnostics.Runtime]Microsoft.Diagnostics.Runtime.ClrAppDomain CLRProfiler.ViewModel.AppDomainListViewModel::_SelectedItem
															IL_0021: callvirt instance void CLRProfiler.ViewModel.PropertyViewViewModel::SetObject(object)
															IL_0026: nop*/
														if (pca.AttributeType.Name == "AttachToPropertyView")
														{															
															var dependentProperty = type.Properties.Where(p => p.PropertyType.Name == "PropertyViewViewModel").FirstOrDefault();
															
															if (dependentProperty != null)
															{
																proc.Append(proc.Create(OpCodes.Ldarg_0));
																proc.Append(proc.Create(OpCodes.Call, dependentProperty.GetMethod));
																proc.Append(proc.Create(OpCodes.Ldarg_0));
																proc.Append(proc.Create(OpCodes.Ldfld, backingField));
																proc.Append(proc.Create(OpCodes.Callvirt, setObjectReference));
																proc.Append(proc.Create(OpCodes.Nop));
															}																														
														}
													}
												}

												proc.Append(proc.Create(OpCodes.Ret));
												Console.WriteLine("{0} - {1}", type.Name, prop.Name);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			assembly.Write(pathToCLRProfiler, new WriterParameters() { WriteSymbols = true });
			File.Delete(workingFileName);
		}
	}
}
