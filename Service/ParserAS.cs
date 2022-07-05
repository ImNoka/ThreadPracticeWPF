using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using ThreadPracticeWPF.Model;
using System.Globalization;

namespace ThreadPracticeWPF.Service
{
    public class ParserAS
    {
        private IConfiguration _config;
        private IBrowsingContext _context;
        public ParserAS()
        {
            _config = Configuration.Default;
            _context = BrowsingContext.New(_config);
            
        }

        public async Task<string> ParseHTML(string source)
        {
            using var context = BrowsingContext.New(_config);
            using var doc = await context.OpenAsync(req => req.Content(@source));
            var node = doc.GetElementsByClassName(@"css-vlibs4");

            StringBuilder sb = new StringBuilder();
            sb.Append("Status code: "+doc.StatusCode);
            sb.Append("\nHead: "+doc.Head);
            sb.Append("\nBody: "+doc.Body.ToString());
            foreach(var item in node)
            {
                sb.Append("\n"+item.GetElementsByClassName("css-1x8dg53")[0].TextContent+
                    " Price: "+item.GetElementsByClassName("css-ovtrou")[0].TextContent);
            }
            return sb.ToString();
        }

        public async Task<List<CurrencyItem>> GetCurrencies(string source)
        {
            List<CurrencyItem> currencies = new List<CurrencyItem>();
            using var context = BrowsingContext.New(_config);
            using var doc = await context.OpenAsync(req => req.Content(@source));

            var node = doc.GetElementsByClassName(@"quote_list")[0];
            foreach(var tr in node.GetElementsByTagName("tbody")[0].GetElementsByTagName("tr"))
            {
                if (tr.GetElementsByTagName("th").Any())
                    continue;
                var tdList = tr.GetElementsByTagName("td");
                CurrencyItem currencyItem = new CurrencyItem()
                {

                    Currency = tdList[1].TextContent,
                    Offer = ParseNode(tdList[2]),
                    Demand = ParseNode(tdList[3]),
                    Last = ParseNode(tdList[4]),
                    PercentUp = tdList[5].TextContent,
                    ValueUp = ParseNode(tdList[6]),
                    DateTime = DateTime.ParseExact(tdList[7].TextContent, "HH:mm:ss",
                                                    CultureInfo.InvariantCulture)
                };
                currencies.Add(currencyItem);

            }
            return currencies;
        }

        private decimal ParseNode(IElement node)
        {

            try
            {
                return decimal.Parse(node.TextContent.Trim(), CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                try
                {
                    var spanNode = node.GetElementsByTagName("div")[0];
                    return decimal.Parse(spanNode.TextContent.Trim(), CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                {
                    try
                    {
                        var spanNode = node.GetElementsByTagName("div")[0].GetElementsByTagName("span")[0];
                        return decimal.Parse(spanNode.TextContent.Trim(), CultureInfo.CurrentCulture);
                    }
                    catch
                    {
                        return 0m;
                    }
                    
                }
            }
        }


    }
}
