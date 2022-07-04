using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadPracticeWPF.Model
{
    public class CounterItem
    {
        public int Value { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string CounterName { get; set; }

    }
}
