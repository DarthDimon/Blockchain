using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Blockchain
{
    public class Rate
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public List<Quote> quotes { get; set; } = new List<Quote>();
        public Rate(CoinСurrency coinCurrency, CoinСurrency fiatCurrency)
        {
            string currentName = "";
            if (coinCurrency == CoinСurrency.Bitcoin) { currentName += "BTC-"; }
            if (coinCurrency == CoinСurrency.Bitcoin) { currentName += "RUB.txt"; }
            string currentDirectory = Environment.CurrentDirectory;
            if (!Directory.Exists(currentDirectory))
            {
                Directory.CreateDirectory(currentDirectory);
            }
            if (!File.Exists(currentDirectory + "\\" + currentName))
            {
                AddFromSite(coinCurrency, fiatCurrency, new DateTime(2010, 1, 1), DateTime.Now);
            }
            else
            {
                GetFromTxt(Environment.CurrentDirectory);
            }
        }
        private Rate() { }
        private void AddFromSite(CoinСurrency coin, CoinСurrency fiat, DateTime startDate, DateTime finishDate)
        {
            DateTime dateTime = new DateTime(1970, 1, 1).AddSeconds(1632700800);
            double start = (startDate.Date - new DateTime(1970, 1, 1)).TotalSeconds;
            double finish = (finishDate.Date - new DateTime(1970, 1, 1)).TotalSeconds;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                "https://api.coinmarketcap.com/data-api/v3/cryptocurrency/historical?id=" +
                (int)coin +
                "&convertId=" +
                (int)fiat +
                "&timeStart=" +
                start +
                "&timeEnd=" +
                finish
                );
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:96.0) Gecko/20100101 Firefox/96.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8";
            request.KeepAlive = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(response.GetResponseStream(), Encoding.UTF8);
            JObject jObject = JObject.Parse(doc.Text);
            if (name == null)
            {
                id = jObject["data"]["id"].ToObject<int>();
                name = jObject["data"]["name"].ToObject<string>();
                symbol = jObject["data"]["symbol"].ToObject<string>();
            }
            quotes = quotes.Concat(jObject["data"]["quotes"].ToObject<List<Quote>>()).ToList();
        }
        private void GetFromTxt(string path)
        {

        }
        private void Write(string path)
        {

        }
        public Tuple<decimal, decimal> GetSumOnDate(decimal sum, DateTime dateTime)
        {
            dateTime = dateTime.Date;
            Quote quote = quotes.FirstOrDefault(r => r.timeOpen == dateTime);
            if (quote == null) { return new Tuple<decimal, decimal>(0, 0); }
            decimal min = sum;
            decimal max = sum;
            return new Tuple<decimal, decimal>(min, max);
        }
        public (decimal? min, decimal? max) GetMinMax(decimal sum, DateTime dateTime)
        {
            dateTime = dateTime.Date;
            Quote quote = quotes.FirstOrDefault(r => r.timeOpen == dateTime);
            if (quote == null) { return (null, null); }
            return ((decimal)quote.quote.low, (decimal)quote.quote.high);
        }
    }
    public class Quote2
    {
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
        public double marketCap { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class Quote
    {
        public DateTime timeOpen { get; set; }
        public DateTime timeClose { get; set; }
        public DateTime timeHigh { get; set; }
        public DateTime timeLow { get; set; }
        public Quote2 quote { get; set; }
    }
    public enum CoinСurrency
    {
        Bitcoin = 1,
        Russian_Ruble = 2806,
        Euro = 2790,
        United_States_Dollar = 2781
        ,Ethereum=1027
    }


}
