using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadPracticeWPF.Model;
using System.Threading;

namespace ThreadPracticeWPF.Service
{
    public class DataSaver
    {
        //private Thread UsdRubThread;
        //private Thread EurRubThread;
        //private Thread JpyRubThread;

        Task URWriteTask;
        Task ERWriteTask;
        Task JRWriteTask;

        List<Task> Tasks = new List<Task>();

        private CurrencyItem UsdRubCurrency;
        private CurrencyItem EurRubCurrency;
        private CurrencyItem JpyRubCurrency;



        public enum Currencies
        {
            UsdRu,
            EurRu,
            JpyRu
        }

        public DataSaver(List<CurrencyItem> currencies, params Currencies[] ps)
        {
            UsdRubCurrency = currencies.FirstOrDefault(c=>c.Currency==@"USD/RUB");
            EurRubCurrency = currencies.FirstOrDefault(c=>c.Currency==@"EUR/RUB");
            JpyRubCurrency = currencies.FirstOrDefault(c=>c.Currency==@"JPY/RUB");
            //UsdRubThread = new Thread(new ThreadStart(SaveUsdRub));
            //EurRubThread = new Thread(new ThreadStart(SaveEurRub));
            //JpyRubThread = new Thread(new ThreadStart(SaveJpyRub));
            ERWriteTask = new Task(SaveEurRub);
            URWriteTask = new Task(SaveUsdRub);
            JRWriteTask = new Task(SaveJpyRub);
            if(ps.Contains(Currencies.UsdRu))
                Tasks.Add(URWriteTask);
            if (ps.Contains(Currencies.EurRu))
                Tasks.Add(ERWriteTask);
            if(ps.Contains(Currencies.JpyRu))
                Tasks.Add(JRWriteTask);
        }


        public async Task<bool> SaveToCSV()
        {
            await Task.Delay(0);
            foreach(Task task in Tasks)
                task.Start();
            Task.WaitAll(ERWriteTask,URWriteTask,JRWriteTask);
            return true;
        }


        private async void SaveEurRub()
        {
            using(StreamWriter writer = new StreamWriter(Environment.CurrentDirectory+"CurrenciesEurRub.csv"))
            {
                await writer.WriteLineAsync(EurRubCurrency.ToCSV());
                //return true;
            }
            //return false;
        }

        private async void SaveUsdRub()
        {
            using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "CurrenciesUsdRub.csv"))
            {
                await writer.WriteLineAsync(UsdRubCurrency.ToCSV());
                //return true;
            }
            //return false;
        }

        private async void SaveJpyRub()
        {
            using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "CurrenciesJpyRub.csv"))
            {
                await writer.WriteLineAsync(JpyRubCurrency.ToCSV());
                //return true;
            }
            //return false;
        }

    }
}
