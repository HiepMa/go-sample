using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AudioUWP.ViewModel
{
    [Bindable]
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigation = new NavigationService();
            //navigation.Configure(SecondPageKey, typeof(SecondPage));
            SimpleIoc.Default.Register<INavigationService>(() => navigation);

            SimpleIoc.Default.Register<IDialogService, DialogService>();

            if (ViewModelBase.IsInDesignModeStatic)
            {

            }
            else
            {

            }
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<DemoViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public DemoViewModel demo => ServiceLocator.Current.GetInstance<DemoViewModel>();
    }
}
