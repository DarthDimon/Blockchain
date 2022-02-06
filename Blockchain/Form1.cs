using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.IO;

namespace Blockchain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int maxDeep = (int)numericUpDown1.Value;
            Rate rateBitcoin = new Rate(CoinСurrency.Bitcoin, CoinСurrency.Russian_Ruble);
            List<string> wallStr = new List<string>();
            using (StreamReader sr = new StreamReader(textBox1.Text))
            {
                string str;
                while((str = sr.ReadLine()) != null)
                {
                    wallStr.Add(str);
                }
            }
            Informations.GetInformations(wallStr, maxDeep, rateBitcoin);
            //if (textBox1.Text == "Путь к списку кошельков") { MessageBox.Show("I need path to wallet list"); return; }
            //if (textBox2.Text == "Путь для сохронения") { MessageBox.Show("I need path to folder for save"); return; }
            //for(int i = 0; i < wallStr.Count; i++)
            //{
            //    GetTransactionsByWallet(wallStr[i], false, maxDeep, rateBitcoin);
            //}
            MessageBox.Show("ok");
        }
        private void GetTransactionsByWallet(string wall, bool append, int maxDeep, Rate rateBitcoin=null, int deep = 0)
        {
            Wallets wallets = new Wallets(wall);
            wallets.Write(append: append, deep: deep);
            wallets.CryptoWallets = wallets.CryptoWallets.Where(r => r.network == "BTC").ToList();
            List<Transactions> transactions = wallets.GetListTransactions();
            transactions.Write(null, append, rateBitcoin, deep);
            append = true;
            if (++deep < maxDeep)
            {
                List<string> walls = new List<string>();
                foreach (Transactions transaction in transactions)
                {
                    foreach(Transaction transaction1 in transaction.TransactionsList)
                    {
                        foreach(Input input in transaction1.inputs)
                        {
                            walls.Add(input.address);
                        }
                        foreach (Output output in transaction1.outputs)
                        {
                            walls.Add(output.address);
                        }

                    }
                }
                walls=walls.Where(r=>r!=wall).Distinct().ToList();
                GetTransactionsByWallet(walls, append, maxDeep, rateBitcoin, deep);
            }
        }
        private void GetTransactionsByWallet(List<string> walls, bool append, int maxDeep, Rate rateBitcoin, int deep)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                GetTransactionsByWallet(walls[i], append, maxDeep, rateBitcoin, deep);
                append = true;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.txt|*.txt";
            openFileDialog.ShowDialog();
            textBox1.Text = openFileDialog.FileName;
        }
    }
}
