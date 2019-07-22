using demoTail.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace demoTail.ViewModels
{
    class SubmitViewModel : ViewModelBase
    {
        private string _tblock;
        private string _Text;
        public ICommand Click_Submit
        {
            get;
            private set;
        }

        public string TextBlock
        {
            get
            {
                return _tblock;
            }
            set
            {
                _tblock = Text;
                RaisePropertyChanged("TextBlock");
            }
        }

        public SubmitViewModel()
        {
            Click_Submit = new RelayCommand(SetTextAsync,CanExecuteMyMethod);
        }

        private void SetTextAsync()
        {
            TextBlock = Text;
            
        }

        public string Text
        {
            set
            {
                _Text = value;
                RaisePropertyChanged("Text");
            }
            get
            {
                return _Text;
            }

        } 

        private bool CanExecuteMyMethod()
        {
            return true;
        }

    }

}
