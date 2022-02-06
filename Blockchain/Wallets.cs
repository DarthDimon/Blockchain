using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Blockchain
{
    public class Wallets
    {
        public string Wallet { get; set; }
        public List<CryptoWallet> CryptoWallets { get; set; }
        public Wallets(string wallet)
        {
            Wallet = wallet;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://tokenview.com/api/search/" + wallet);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:96.0) Gecko/20100101 Firefox/96.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8";
            request.KeepAlive = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(response.GetResponseStream(), Encoding.UTF8);
            JObject jObject = JObject.Parse(doc.Text);
            CryptoWallets = jObject["data"].ToObject<List<CryptoWallet>>();
        }
        public void Write(string path = "CryptoWallets.txt", bool append = true, char separator = '\t', int? deep = null)
        {
            if (CryptoWallets == null || CryptoWallets.Count == 0) { return; }
            using (StreamWriter sw = new StreamWriter(path, append, Encoding.UTF8))
            {
                if (!append)
                {
                    if (deep != null) { sw.WriteLine(CryptoWallet.GetTitle(separator) + separator + "deep"); }
                    else { sw.WriteLine(CryptoWallet.GetTitle(separator)); }
                }
                foreach (CryptoWallet wallet in CryptoWallets)
                {
                    if (deep != null) { sw.WriteLine(wallet.GetStr(separator) + separator + deep.ToString()); }
                    else { sw.WriteLine(wallet.GetStr(separator)); }
                }
            }
        }
    }
    public class CryptoWallet
    {
        public string type { get; set; }
        public string hash { get; set; }
        public string network { get; set; } //валюта
        public int txCount { get; set; }//Количество транзакций
        public decimal spend { get; set; }//Общая сумма перевода
        public decimal receive { get; set; }//Общий доход
        public void Write(string path, bool append, char separator = '\t')
        {
            using (StreamWriter sw = new StreamWriter(path, append, Encoding.UTF8))
            {
                if (!append) { sw.WriteLine(GetTitle(separator)); }
                sw.WriteLine(GetStr(separator));
            }
        }
        public static string GetTitle(char separator)
        {
            return string.Format("кошелек{0}" +
                "type{0}" +
                "валюта{0}" +
                "Количество транзакций{0}" +
                "Общая сумма перевода{0}" +
                "Общий доход{0}" +
                "остаток", separator);
        }
        public string GetStr(char separator)
        {
            return string.Format("{1}{0}" +
                "{2}{0}" +
                "{3}{0}" +
                "{4}{0}" +
                "{5}{0}" +
                "{6}{0}" +
                "{7}",
                separator,
                hash,
                type,
                network,
                txCount,
                spend,
                receive,
                (receive + spend).ToString());
        }
    }
}
