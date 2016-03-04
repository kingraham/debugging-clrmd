using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace CLRProfiler.ViewModel
{   
    public class ViewModelLocator
    {      
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);          
            SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<ProcessViewViewModel>();
			SimpleIoc.Default.Register<ProcessListViewModel>();
			SimpleIoc.Default.Register<ProcessInformationViewModel>();
			SimpleIoc.Default.Register<ViewServices.DialogService>();
        }

		public ProcessListViewModel ProcessList
		{
			get
			{
				return ServiceLocator.Current.GetInstance<ProcessListViewModel>();
			}
		}

		public ProcessViewViewModel ProcessView
		{
			get
			{
				return ServiceLocator.Current.GetInstance<ProcessViewViewModel>();
			}
		}

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            
        }
    }
}