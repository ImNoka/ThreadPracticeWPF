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
    public class CurrencyWindowViewModel : INotifyPropertyChanged
    {
        ManualResetEvent resetEvent = new ManualResetEvent(false);
        public Thread ParsingThread { get; set; }
        public Thread WritingThread { get; set; }

        private ObservableCollection<CurrencyItem> currencies;

        public ObservableCollection<CurrencyItem> Currencies
        {
            get { return currencies; }
            set
            {
                currencies = value;
                OnPropertyChanged();
            }
        }

        private bool isRunning = false;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                OnPropertyChanged();
            }
        }

        private bool isWriting = false;
        public bool IsWriting
        {
            get { return isWriting; }
            set
            {
                isWriting = value;
                OnPropertyChanged();
            }
        }

        private DateTime timeUpdated = DateTime.Now;
        public DateTime TimeUpdated
        {
            get { return timeUpdated; }
            set
            {
                timeUpdated = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand startParsing;
        public RelayCommand StartParsing
        {
            get
            {
                return startParsing ??
                    (startParsing = new RelayCommand((obj) =>
                    {
                        if (IsRunning)
                            return;
                        IsRunning = true;
                        if (ParsingThread.ThreadState == ThreadState.WaitSleepJoin)
                        {
                            resetEvent.Set();
                            return;
                        }


                        ParsingThread.Start();
                    }));
            }
        }

        private RelayCommand stopParsing;
        public RelayCommand StopParsing
        {
            get
            {
                return stopParsing ??
                    (stopParsing = new RelayCommand((obj) =>
                    {
                        if (!IsRunning)
                            return;
                        IsRunning= false;
                        resetEvent.Reset();
                    }));
            }
        }

        private RelayCommand startWriting;
        public RelayCommand StartWriting
        {
            get
            {
                return startWriting ??
                    (startWriting = new RelayCommand((obj) =>
                    {
                        if (isWriting)
                            return;
                        isWriting = true;

                    }));
            }
        }


        public CurrencyWindowViewModel()
        {
            Currencies = new ObservableCollection<CurrencyItem>();
            ParsingThread = new Thread(new ThreadStart(LoadAndParse));
            //LoadAndParse();
        }

        public void LoadAndParse()
        {
            while (true)
            {
                if (!IsRunning)
                    resetEvent.WaitOne();
                LoaderHTML loader = new LoaderHTML();
                string htmlText = loader.LoadHTML(@"https://www.finanz.ru/valyuty/v-realnom-vremeni").Result;
                ParserAS parser = new ParserAS();
                List<CurrencyItem> items = parser.GetCurrencies(htmlText).Result;
                Currencies = new ObservableCollection<CurrencyItem>(items);
                TimeUpdated = DateTime.Now;
                Thread.Sleep(1000);
            }
            
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }



    }
}
