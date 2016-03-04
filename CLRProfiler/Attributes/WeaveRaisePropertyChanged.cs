using System;

namespace CLRProfiler.Attributes
{
	/// <summary>
	/// Any classes decorated with this attribute will have all public properties replaced with MVVM Light set calls.
	/// </summary>
	public class WeaveRaisePropertyChanged : Attribute
	{
	}
}