using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MVVMBase.MVVM.View;
using MVVMBase.MVVM.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using NModbus.Device;
using System.IO.Ports;


namespace MVVMBase.MVVM.ViewModel
{
    public class MainViewModel
    {
        public MainViewModel()
        {

            #region LOG
            OutputItems = new ObservableCollection<LogItem>(OutputLog.GetInstance().LogItems);
            ((INotifyCollectionChanged)OutputLog.GetInstance().LogItems).CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (LogItem item in a.NewItems)
                            OutputItems.Add(item);
                    }));
                if (a.OldItems?.Count >= 1)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (LogItem item in a.OldItems)
                            OutputItems.Remove(item);
                    }));
            };
            #endregion  

            reader = new();
            Button1 = new RelayCommand(obj => StartReading());
            Button2 = new RelayCommand(obj => StopReading());
        }
        public ObservableCollection<LogItem> OutputItems { get; set; }
        public RelayCommand Button1 { get; set; }
        public RelayCommand Button2 { get; set; }
        void StartReading() 
        {
            reader.StartReading();
        }
        void StopReading()
        {
            reader.StopReading();
        }

        DataReader reader;
        public string PollingCycle { get; set; }
    }
}
