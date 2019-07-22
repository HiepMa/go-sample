using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioUWP.ViewModel
{
    public class DemoViewModel :  ViewModelBase
    {
        private RelayCommand _click_start;
        private RelayCommand _click_stop;

        public RelayCommand Click_start
        {
            get
            {
                return _click_start
                    ?? (_click_start = new RelayCommand(Start));
            }
        }

        public RelayCommand Click_Stop
        {
            get
            {
                return _click_stop
                    ?? (_click_stop = new RelayCommand(Stop));
            }
        }

        private void Stop()
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            
        }
    }
}
