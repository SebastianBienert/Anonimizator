/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Anonimizator"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System.IO;
using Anonimizator.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
namespace Anonimizator.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<AnonimizatorViewModel>();
            SimpleIoc.Default.Register<KAnonimizationViewModel>();
            SimpleIoc.Default.Register<AnonimizationViewModel>();
            SimpleIoc.Default.Register<FileService>();
        }

        public AnonimizatorViewModel AnonimizatorViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AnonimizatorViewModel>();
            }
        }

        public KAnonimizationViewModel KAnonimizationViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<KAnonimizationViewModel>();
            }
        }

        public AnonimizationViewModel AnonimizationViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AnonimizationViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}