using GalaSoft.MvvmLight;

namespace CLRProfiler.Model
{
	public class ProcessItem 
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public ProcessItem(System.Diagnostics.Process p)
		{
			this.ID = p.Id;
			this.Name = p.ProcessName;
		}
	}
}