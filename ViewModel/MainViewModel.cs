using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ThreadPracticeWPF.Model;
using ThreadPracticeWPF.Service;
using ThreadPracticeWPF.View;

namespace ThreadPracticeWPF.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        int x;
        int time = 2000;
        public int Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }


        private bool isSynhronized = true;
        public bool IsRunning = true;

        int lastNumber = 0;
        public int LastNumber
        {
            get { return lastNumber; }
            set
            {
                lastNumber = value;
                OnPropertyChanged();
            }
        }

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
        ManualResetEvent resetEvent = new ManualResetEvent(false);

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


        private RelayCommand runCounters;
        private RelayCommand stopCounters;

        public RelayCommand RunCounters
        {
            get
            {
                return runCounters ??
                    (runCounters = new RelayCommand((counter) =>
                    {
                        IsRunning = true;
                        if (Counter1.ThreadState == ThreadState.WaitSleepJoin && Counter2.ThreadState == ThreadState.WaitSleepJoin)
                        {
                            resetEvent.Set();
                            return;
                        }

                        Counter1.Start(CounterItems1);
                        Counter2.Start(CounterItems2);
                    }));
            }
        }
        public RelayCommand StopCounters
        {
            get
            {
                return stopCounters ??
                    (stopCounters = new RelayCommand((counter) =>
                    {
                        IsRunning = false;
                        resetEvent.Reset();
                    }));
            }
        }



        RelayCommand newChildWindow;
        public RelayCommand NewChildWindow
        {
            get
            {
                return newChildWindow ??
                    (new RelayCommand((newChild) =>
                    {
                        CurrencyWindow childWindow = new CurrencyWindow();
                        childWindow.Show();
                    }));
            }
        }


        public MainViewModel()
        {
            CounterItems1 = new ObservableCollection<CounterItem>();
            CounterItems2 = new ObservableCollection<CounterItem>();
            CounterItems = new ObservableCollection<CounterItem>() { new CounterItem() { Value = 0 } };
            Counter1 = new Thread(new ParameterizedThreadStart(RunCounter1Test));
            Counter1.Name = "Counter1";
            Counter2 = new Thread(new ParameterizedThreadStart(RunCounter2Test));
            Counter2.Name = "Counter2";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public void RunCounter1Test(object? items)
        {
            ObservableCollection<CounterItem> counterItems = items as ObservableCollection<CounterItem>;
            if (counterItems == null)
                return;
            while (CounterItems.Count < 1000)
            {
                
                if (!IsRunning)
                    resetEvent.WaitOne();
                if (IsSynhronized)
                    waitHandler.WaitOne();
                else
                    waitHandler.Set();

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var counterItem = new CounterItem()
                    {
                        Value = CounterItems.LastOrDefault().Value + 1,
                        CounterName = "Counter1"
                    };
                    counterItems.Add(counterItem);
                    CounterItems.Add(counterItem);
                    LastNumber = counterItem.Value;

                });

                if (isSynhronized)
                {
                    Thread.Sleep(Time);
                    waitHandler.Set();
                }
                else
                {
                    Thread.Sleep(Time);
                    continue;
                }
                

            }


        }

        public void RunCounter2Test(object? items)
        {
            ObservableCollection<CounterItem> counterItems = items as ObservableCollection<CounterItem>;
            if (counterItems == null)
                return;
            while (CounterItems.Count < 1000)
            {
                
                if (!IsRunning)
                    resetEvent.WaitOne();
                if (IsSynhronized)
                    waitHandler.WaitOne();
                else
                    waitHandler.Set();
                

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var counterItem = new CounterItem()
                    {
                        Value = CounterItems.LastOrDefault().Value + 1,
                        CounterName = "Counter2"
                    };
                    counterItems.Add(counterItem);
                    CounterItems.Add(counterItem);
                    LastNumber = counterItem.Value;

                });

                if (IsSynhronized)
                {
                    Thread.Sleep(Time / 3);
                    waitHandler.Set();
                }
                else
                {
                    Thread.Sleep(Time / 3);
                    continue;
                }
                
            }


        }
    }

}
