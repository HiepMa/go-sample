using demoTail.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace demoTail.ViewModels
{
    class ExampleViewModel : ViewModelBase
    {
        public ICommand Start_Server { get; private set; }
        public ICommand Start_Client { get; private set; }
        public ExampleViewModel()
        {
            Start_Server = new RelayCommand(
                Startserver,
                CanExecuteMyMethod());
            Start_Client = new RelayCommand(Client, CanExecuteMyMethod);
        }
        public void Client()
        {
            Service.Service service = new Service.Service();
            service.SaveAudioToFile();
        }
        public void Startserver()
        {
            Service.Service sv = new Service.Service();
            Thread server = new Thread(sv.ServerStartAsync);
            server.Start();
        }
        private bool CanExecuteMyMethod()
        {
            return true;
        }
    }
}
