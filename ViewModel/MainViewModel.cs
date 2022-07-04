using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadPracticeWPF.Model;
using ThreadPracticeWPF.Service;

namespace ThreadPracticeWPF.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {

        int time = 2000;

        private bool isSynhronized = true;
        public bool IsSynhronized
        {
            get { return isSynhronized; }
            set
            {
                isSynhronized = value;
                OnPropertyChanged();
            }
        }

        AutoResetEvent waitHandler = new AutoResetEvent(true);
        public Thread Counter1 { get; set; }
        public Thread Counter2 { get; set; }

        private ObservableCollection<CounterItem> counterItems;

        public ObservableCollection<CounterItem> CounterItems
        {
            get { return counterItems; }
            set { counterItems = value; 
                    OnPropertyChanged(); }
        }

        private ObservableCollection<CounterItem> counterItems1;

        public ObservableCollection<CounterItem> CounterItems1
        {
            get { return counterItems1; }
            set
            {
                counterItems1 = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CounterItem> counterItems2;

        public ObservableCollection<CounterItem> CounterItems2
        {
            get { return counterItems2; }
            set
            {
                counterItems2 = value;
                OnPropertyChanged();
            }
        }


        private RelayCommand counterCommand;

        public RelayCommand CounterCommand
        {
            get
            {
                return counterCommand ??
                    (counterCommand = new RelayCommand((counter) =>
                    {
                        Thread ctr = counter as Thread;
                        /*if (ctr.ThreadState == ThreadState.Unstarted)
                        {
                            switch (ctr.Name)
                            {
                                case ("Counter1"):
                                    ctr.Start(CounterItems1);
                                    break;
                                case ("Counter2"):
                                    ctr.Start(CounterItems2);
                                    break;
                                default:
                                    return;
                            }
                        }*/
                        Counter1.Start(CounterItems1);
                        Counter2.Start(CounterItems2);
                    }));
            }
        }



        public MainViewModel()
        {
            CounterItems1 = new ObservableCollection<CounterItem>();
            CounterItems2 = new ObservableCollection<CounterItem>();
            CounterItems = new ObservableCollection<CounterItem>() { new CounterItem() { Value=0} };
            Counter1 = new Thread(new ParameterizedThreadStart(RunCounter1));
            Counter1.Name = "Counter1";
            Counter2 = new Thread(new ParameterizedThreadStart(RunCounter2));
            Counter2.Name = "Counter2";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public void RunCounter1(object? items)
        {
            ObservableCollection<CounterItem> counterItems = items as ObservableCollection<CounterItem>;
            if (counterItems == null)
                return;
            while(CounterItems.Count < 1000)
            {
                if(isSynhronized)
                    try
                    {
                        waitHandler.WaitOne();
                    }
                    catch (ObjectDisposedException ex)
                    {
                        waitHandler = new AutoResetEvent(false);
                        waitHandler.WaitOne();
                    }
                /*if (CounterItems.Count==0)
                    App.Current.Dispatcher.Invoke((Action)delegate {
                    CounterItems.Add(new CounterItem() { Value = 0 });
                });*/
                //for (int i = 0; i < 3; i++)
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var counterItem = new CounterItem()
                    {
                        Value = CounterItems.LastOrDefault().Value + 1,
                        CounterName = "Counter1"
                    };
                    counterItems.Add(counterItem);
                    CounterItems.Add(counterItem);
                    
                });
                Thread.Sleep(time);
                if (isSynhronized)
                    try
                    {
                        waitHandler.Set();
                    }
                    catch (ObjectDisposedException ex) { }
                else
                {
                    try
                    {
                        waitHandler.Set();
                    }
                    catch (ObjectDisposedException ex) { }
                    waitHandler.Dispose();
                }
            }
        }
        public void RunCounter2(object? items)
        {
            ObservableCollection<CounterItem> counterItems = items as ObservableCollection<CounterItem>;
            if (counterItems == null)
                return;
            while (CounterItems.Count<1000)
            {
                if (isSynhronized)
                    try
                    {
                        waitHandler.WaitOne();
                    }
                    catch (ObjectDisposedException ex) 
                    {
                        waitHandler = new AutoResetEvent(false);
                        waitHandler.WaitOne();
                    }

                /*if (CounterItems.Count==0)
                    App.Current.Dispatcher.Invoke((Action)delegate {
                    CounterItems.Add(new CounterItem() { Value = 0 });
                });*/
                //for(int i = 0; i<3;i++)
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var counterItem = new CounterItem()
                    {
                        Value = CounterItems.LastOrDefault().Value + 1,
                        CounterName = "Counter2"
                    };
                    counterItems.Add(counterItem);
                    CounterItems.Add(counterItem);
                    
                });
                Thread.Sleep(time / 2);

                if (isSynhronized)
                    try
                    {
                        waitHandler.Set();
                    }
                    catch (ObjectDisposedException ex) { }
                else
                {
                    try
                    {
                        waitHandler.Set();
                    }
                    catch(ObjectDisposedException ex) { }
                    waitHandler.Dispose();
                }
                    

            }
        }
    }
}
