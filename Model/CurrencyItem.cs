using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadPracticeWPF.Model
{
    public class CurrencyItem
    {
        public string Currency { get; set; }
        public decimal Offer { get; set; }
        public decimal Demand { get; set; }
        public decimal Last { get; set; }
        public string PercentUp { get; set; }
        public decimal ValueUp { get; set; }
        public DateTime DateTime { get; set; }


        public string ToCSV()
        {
            return $"{Currency}," +
                $"{Offer}," +
                $"{Demand}," +
                $"{Last}," +
                $"{Last}," +
                $"{PercentUp}," +
                $"{ValueUp}," +
                $"{DateTime}";
        }
    }
}
