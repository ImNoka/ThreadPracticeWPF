using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThreadPracticeWPF.ViewModel;

namespace ThreadPracticeWPF.Service
{
    public class LoaderHTML
    {

        public LoaderHTML()
        {
            
        }

        public async Task<string> LoadHTML(string address)
        {
            string pageContents;
            using (HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5)})
            {
                var response = await client.GetAsync(address).ConfigureAwait(false);
                pageContents = await response.Content.ReadAsStringAsync();
            }
            return pageContents;
        }
    }

    
}
