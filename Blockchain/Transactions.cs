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
    public class Transactions
    {
        public string Network { get; set; }
        public string Wallet { get; set; }
        public List<Transaction> TransactionsList { get; set; }
        public Transactions(string network, string wallet, int trCount)
        {
            Network = network;
            Wallet = wallet;
            TransactionsList = new List<Transaction>();
            for (int offset = 0, limit = 50; (TransactionsList.Count == 0 || TransactionsList.Count < trCount) && offset<=50000; offset += limit)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format(
                    "https://api.blockchain.info/haskoin-store/{0}/address/{1}/transactions/full?limit={2}&offset={3}",
                    Network.ToLower(), wallet, limit, offset));
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:96.0) Gecko/20100101 Firefox/96.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8";
                request.KeepAlive = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(response.GetResponseStream(), Encoding.UTF8);
                JObject jObject = JObject.Parse("{\"data\":" + doc.Text + "}");
                //if (offset == 0) { trCount = jObject["data"][0]["txCount"].ToObject<int>(); }
                TransactionsList = TransactionsList.Concat(jObject["data"].ToObject<List<Transaction>>()).ToList();
            }
            //if(trCount< TransactionsList.Count) TransactionsList.RemoveRange(trCount, TransactionsList.Count-trCount);
            //TransactionsList=TransactionsList.Where(
            //    r=>r.outputs.FirstOrDefault(t=>t.address==wallet)!=null
            //    || r.inputs.FirstOrDefault(t => t.address == wallet) != null
            //    ).ToList(); // не понятно почему возвращает больше транзакций
            //TransactionsList = TransactionsList.Distinct().ToList();
        }
        public Transactions(CryptoWallet cryptoWallet) : this(cryptoWallet.network, cryptoWallet.hash, cryptoWallet.txCount) { }
        public void Write(string path, bool append, char separator = '\t', char separator2 = ';', Rate rate = null, int? deep = null)
        {
            using (StreamWriter sw = new StreamWriter(path, append, Encoding.UTF8))
            {
                if (!append)
                {
                    string strT = "Кошелек" + separator + Transaction.GetTitle(separator, rate);
                    if (deep != null) { strT += separator + "глубина"; }
                    sw.WriteLine(strT);
                }
                foreach (Transaction transaction in TransactionsList)
                {
                    string str = Wallet + separator + transaction.GetStr(separator, separator2, Wallet, rate);
                    if (deep != null) { str += separator + deep.ToString(); }
                    sw.WriteLine(str);
                }
            }
        }
    }
    //public class Transaction
    //{
    //    public string type { get; set; } //тип кошелек или транзакция
    //    public string network { get; set; } //валюта
    //    public decimal block_no { get; set; } //Блок
    //    public decimal height { get; set; } //Блок
    //    public decimal index { get; set; } //
    //    public decimal time { get; set; }// пример 1643112011
    //    public string txid { get; set; } //номер транзакции
    //    public string fee { get; set; } //Комиссия
    //    public decimal confirmations { get; set; }//Кол-во подтверждений
    //    public List<Input> inputs { get; set; }
    //    public List<Output> outputs { get; set; }
    //    public decimal inputCnt { get; set; } //количество отправителей
    //    public decimal outputCnt { get; set; } //количество получателей
    //    public static string GetTitle(char separator)
    //    {
    //        return string.Format("type{0}" +
    //            "type{0}" +
    //            "валюта{0}" +
    //            "block_no{0}" +
    //            "height{0}" +
    //            "index{0}" +
    //            "time" +
    //            "номер транзакции{0}" +
    //            "Комиссия в блоке{0}" +
    //            "Кол-во подтверждений{0}" +
    //            "{1}{0}" +
    //            "{2}{0}" +
    //            "количество отправителей{0}" +
    //            "количество получателей",
    //            separator,
    //            Input.GetTitle(separator),
    //            Output.GetTitle(separator));
    //    }
    //    public string GetStr(char separator, char separator2, string wallet)
    //    {
    //        return string.Format("{1}{0}" +
    //            "{2}{0}" +
    //            "{3}{0}" +
    //            "{4}{0}" +
    //            "{5}{0}" +
    //            "{6}{0}" +
    //            "{7}{0}" +
    //            "{8}{0}" +
    //            "{9}{0}" +
    //            "{10}{0}" +
    //            "{11}{0}" +
    //            "{12}{0}" +
    //            "{13}",
    //            separator,
    //            type,
    //            network,
    //            block_no,
    //            height,
    //            index,
    //            time,
    //            txid,
    //            fee,
    //            confirmations,
    //            inputs.GetStr(separator, separator2, wallet),
    //            outputs.GetStr(separator, separator2, wallet),
    //            inputCnt,
    //            outputCnt);
    //    }
    //}
    //public class ReceivedFrom
    //{
    //    public decimal output_no { get; set; }
    //    public string txid { get; set; }
    //    public static string GetTitle(char separator)
    //    {
    //        return string.Format("output_no{0}" +
    //            "txid{0}", separator);
    //    }
    //    public string GetStr(char separator)
    //    {
    //        return string.Format("{1}{0}" +
    //            "{2}",
    //            separator,
    //            output_no,
    //            txid);
    //    }
    //}
    //public class Input
    //{
    //    public decimal input_no { get; set; }
    //    public string address { get; set; }
    //    public string value { get; set; }
    //    public string addressAlias { get; set; }
    //    public ReceivedFrom received_from { get; set; }
    //    public static string GetTitle(char separator)
    //    {
    //        return string.Format("input_no{0}" +
    //            "address{0}" +
    //            "value{0}" +
    //            "addressAlias{0}" +
    //            "{1}", separator, ReceivedFrom.GetTitle(separator));
    //    }
    //    public string GetStr(char separator)
    //    {
    //        return string.Format("{1}{0}" +
    //            "{2}{0}" +
    //            "{3}{0}" +
    //            "{4}{0}" +
    //            "{5}",
    //            separator,
    //            input_no,
    //            address,
    //            value,
    //            addressAlias,
    //            received_from.GetStr(separator));
    //    }
    //}
    //public class Output
    //{
    //    public decimal output_no { get; set; }
    //    public string address { get; set; }
    //    public string value { get; set; }
    //    public string addressAlias { get; set; }
    //    public static string GetTitle(char separator)
    //    {
    //        return string.Format("output_no{0}" +
    //            "address{0}" +
    //            "value{0}" +
    //            "addressAlias", separator);
    //    }
    //    public string GetStr(char separator)
    //    {
    //        return string.Format("{1}{0}" +
    //            "{2}{0}" +
    //            "{3}{0}" +
    //            "{4}",
    //            separator,
    //            address,
    //            value,
    //            addressAlias);
    //    }
    //}
    public static class TransactionMetods
    {
        public static List<Transactions> GetListTransactions(this List<CryptoWallet> cryptoWallets)
        {
            List<Transactions> result = new List<Transactions>();
            foreach (var cryptoWallet in cryptoWallets)
            {
                result.Add(new Transactions(cryptoWallet));
            }
            return result;
        }
        public static List<Transactions> GetListTransactions(this Wallets wallets)
        {
            return wallets.CryptoWallets.GetListTransactions();
        }
        public static List<Transactions> GetListTransactions(this List<Wallets> walletsList)
        {
            List<Transactions> result = new List<Transactions>();
            foreach (Wallets wallet in walletsList)
            {
                result = result.Concat(wallet.GetListTransactions()).ToList();
            }
            return result;
        }
        public static string GetStr(this List<Input> inputs, char separator, char separator2, string wallet)
        {

            if (wallet != null && inputs.FirstOrDefault(r => r.address == wallet) != null)
            {
                inputs = inputs.Where(r => r.address == wallet).ToList();
            }
            return inputs.GetStr(separator, separator2);
        }
        public static string GetStr(this List<Input> inputs, char separator, char separator2)
        {
            string address = "", pkscript = "",
                value = "", coinbase = "",
                txid = "", output = "",
                sigscript = "", sequence = ""
                //,witness = ""
                ;
            for (int i = 0; i < inputs.Count; i++)
            {
                if (i > 0)
                {
                    address += separator2 + " ";
                    pkscript += separator2 + " ";
                    value += separator2 + " ";
                    coinbase += separator2 + " ";
                    txid += separator2 + " ";
                    output += separator2 + " ";
                    sigscript += separator2 + " ";
                    sequence += separator2 + " ";
                }
                address += inputs[i].address;
                pkscript += inputs[i].pkscript;
                value += (inputs[i].value / 100000000);
                coinbase += inputs[i].coinbase;
                txid += inputs[i].txid;
                output += inputs[i].output;
                sigscript += inputs[i].sigscript;
                sequence += inputs[i].sequence;
            }
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}",
                separator,
                address,
                pkscript,
                value,
                coinbase,
                txid,
                output,
                sigscript,
                sequence
                //witness
                );
        }
        public static string GetStr(this List<Output> outputs, char separator, char separator2, string wallet)
        {

            if (wallet != null && outputs.FirstOrDefault(r => r.address == wallet) != null)
            {
                outputs = outputs.Where(r => r.address == wallet).ToList();
            }
            return outputs.GetStr(separator, separator2);
        }
        public static string GetStr(this List<Output> outputs, char separator, char separator2)
        {
            string address = "", pkscript = "", value = "", spent = "", spender = "", spTxid = "", spInput = "";
            for (int i = 0; i < outputs.Count; i++)
            {
                if (i > 0)
                {
                    address += separator2 + " ";
                    pkscript += separator2 + " ";
                    value += separator2 + " ";
                    spent += separator2 + " ";
                    if (outputs[i].spender != null)
                    {
                        spTxid += separator2 + " ";
                        spInput += separator2 + " ";
                    }
                }
                address += outputs[i].address;
                pkscript += outputs[i].pkscript;
                value += outputs[i].value / 100000000;
                spent += outputs[i].spent.ToString();
                if (outputs[i].spender != null)
                {
                    spTxid += outputs[i].spender.txid;
                    spInput += outputs[i].spender.input;
                }
            }
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}",
                separator,
                address,
                pkscript,
                value,
                spent,
                spTxid,
                spInput);
        }
        public static string GetStr(this List<string> list, char separator)
        {
            string str = "";
            for (int i = 0; i < list.Count; i++)
            {
                str += list[0];
                if (i > 0)
                {
                    str += separator + " ";
                }
            }
            return str;
        }
        public static void Write(this List<Transactions> transactions, string path, bool append, Rate rate = null, int deep = 0)
        {
            if (path == null || !path.Substring(path.Length - 6).Contains('.'))
            {
                path += path != null ? "/Transactions.txt" : "Transactions.txt";
            }
            for (int i = 0; i < transactions.Count; i++)
            {
                transactions[i].Write(path, append, '\t', ';', rate, deep);
            }
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Input
    {
        public string address { get; set; }
        public string pkscript { get; set; }
        public decimal value { get; set; }
        public bool coinbase { get; set; }
        public string txid { get; set; }
        public decimal output { get; set; }
        public string sigscript { get; set; }
        public string sequence { get; set; }
        public List<string> witness { get; set; }
        public static string GetTitle(char separator, string prefix)
        {
            prefix += "Input.";
            return string.Format("{1}address{0}" +
                "{1}pkscript{0}" +
                "{1}value{0}" +
                "{1}coinbase{0}" +
                "{1}txid{0}" +
                "{1}output{0}" +
                "{1}sigscript{0}" +
                "{1}sequence", separator, prefix);
        }
        public string GetStr(char separator)
        {
            return string.Format("{1}{0}" +
                "{2}{0}" +
                "{3}{0}" +
                "{4}{0}" +
                "{5}{0}" +
                "{6}{0}" +
                "{7}{0}" +
                "{8}{0}" +
                "{9}",
                separator,
                address.ToString(),
                pkscript,
                value / 100000000,
                coinbase,
                txid,
                output,
                sigscript,
                sequence,
                witness.GetStr(';')
                );
        }
    }
    public class Spender
    {
        public string txid { get; set; }
        public decimal input { get; set; }
        public static string GetTitle(char separator, string prefix)
        {
            prefix += "Spender.";
            return string.Format("{1}txid{0}" +
                "{1}input", separator, prefix);
        }
        public string GetStr(char separator)
        {
            return string.Format("{1}{0}" +
                "{2}",
                separator,
                txid,
                input);
        }
        public static string GetNullStr(char separator) { return separator.ToString(); }
    }
    public class Output
    {
        public string address { get; set; }
        public string pkscript { get; set; }
        public decimal value { get; set; }
        public bool spent { get; set; }
        public Spender spender { get; set; }
        public static string GetTitle(char separator, string prefix)
        {
            prefix += "Output.";
            return string.Format("{1}address{0}" +
                "{1}pkscript{0}" +
                "{1}value{0}" +
                "{1}spent{0}" +
                "{1}{2}", separator, prefix, Spender.GetTitle(separator, prefix));
        }
        public string GetStr(char separator)
        {
            return string.Format("{1}{0}" +
                "{2}{0}" +
                "{3}{0}" +
                "{4}{0}" +
                "{5}",
                separator,
                address,
                pkscript,
                value / 100000000,
                spent,
                spender.GetStr(separator));
        }
    }
    public class Block
    {
        public decimal height { get; set; }
        public decimal position { get; set; }
        public static string GetTitle(char separator, string prefix)
        {
            prefix += "Block.";
            return string.Format("{1}height{0}" +
                "{1}position", separator, prefix);
        }
        public string GetStr(char separator)
        {
            return string.Format("{1}{0}" +
                "{2}",
                separator,
                height,
                position);
        }
    }
    public class Transaction
    {
        public string txid { get; set; }
        public decimal size { get; set; }
        public decimal version { get; set; }
        public decimal locktime { get; set; }
        public decimal fee { get; set; } // комиссия /1000000000
        public List<Input> inputs { get; set; }
        public List<Output> outputs { get; set; }
        public Block block { get; set; }
        public bool deleted { get; set; }
        public decimal time { get; set; }
        public bool rbf { get; set; }
        public decimal weight { get; set; }
        public static string GetTitle(char separator)
        {
            string prefix = "Transaction.";
            string res = string.Format("{1}txid{0}" +
                "{1}size{0}" +
                "{1}version{0}" +
                "{1}locktime{0}" +
                "{1}комиссия блока{0}" +
                "{1}{2}{0}" +
                "{1}{3}{0}" +
                "{1}{4}{0}" +
                "{1}deleted{0}" +
                "{1}time{0}" +
                "{1}rbf{0}" +
                "{1}weight",
                separator,
                prefix,
                Input.GetTitle(separator, prefix),
                Output.GetTitle(separator, prefix),
                Block.GetTitle(separator, prefix));
            return res;
        }
        public static string GetTitle(char separator, Rate rate)
        {
            string str = string.Format("{0}Min{0}Max", separator);
            return GetTitle(separator) + str;
        }
        public string GetStr(char separator, char separator2, string wallet)
        {
            return string.Format("{1}{0}" +
                "{2}{0}" +
                "{3}{0}" +
                "{4}{0}" +
                "{5}{0}" +
                "{6}{0}" +
                "{7}{0}" +
                "{8}{0}" +
                "{9}{0}" +
                "{10}{0}" +
                "{11}{0}" +
                "{12}",
                separator,
                txid,
                size,
                version,
                locktime,
                fee / 100000000,
                inputs.GetStr(separator, separator2, wallet),
                outputs.GetStr(separator, separator2, wallet),
                block.GetStr(separator),
                deleted,
                new DateTime(1970, 1, 1).AddHours(3).AddSeconds((double)time).ToString(),
                rbf,
                weight
                );
        }
        public string GetStr(char separator, char separator2, string wallet, Rate rate)
        {
            DateTime dateTime = new DateTime(1970, 1, 1).AddHours(3).AddSeconds((double)time);
            var minMax = rate.GetMinMax(fee, dateTime);
            string str = string.Format(
                "{0}{1}" +
                "{0}{2}"
                , separator
                , minMax.min
                , minMax.max
                );
            return GetStr(separator, separator2, wallet) + str;
        }
    }

}
//https://api.coinmarketcap.com/data-api/v3/cryptocurrency/historical?id=1&convertId=2806&timeStart=1632700800&timeEnd=1643241600 курс
