using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bonus_optimizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class ComputeClassA
        {
            public string ID;
            public string SelecteRes;
            public double odds;
            public double Times;
        }
        double MaxMoneySet = 1000;
        int RecursionCount = 2;

        private void Recursion_Multibet(List<ComputeClassA> S_list, ref List<ComputeClassA> Res_List, int MaxRecursion, string Fstr = "", float Money = 1, int Seti = 0, int thisRecursion = 1)
        {
            for (int i = Seti; i < S_list.Count; i++)
            {
                float odds = (float)S_list[i].odds * Money;
                string thisStr = S_list[i].ID + "(" + S_list[i].SelecteRes + ")" + (Fstr == "" ? "" : " x " + Fstr);
                if (thisRecursion != MaxRecursion) Recursion_Multibet(S_list, ref Res_List, MaxRecursion, thisStr, odds, i + 1, thisRecursion + 1);
                else Res_List.Add(new ComputeClassA() { ID = thisStr, odds = odds });
            }
        }
        private void Recursion_ID(List<ComputeClassA> S_list, ref List<string[]> Res_List, int MaxRecursion, string Fstr = "", float Money = 1, int Seti = 0, int thisRecursion = 1)
        {
            for (int i = Seti; i < S_list.Count; i++)
            {
                string thisStr = S_list[i].ID + (Fstr == "" ? "" : "#" + Fstr);
                if (thisRecursion != MaxRecursion) Recursion_ID(S_list, ref Res_List, MaxRecursion, thisStr, 0, i + 1, thisRecursion + 1);
                else Res_List.Add(thisStr.Split('#').ToArray());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var WriteToTextBoxF1 = textBox3;
            string[] str = textBox1.Text.Replace("\r\n", "\r").Replace(" ", "#").Split('\r'); 

            if (checkBox1.Checked) RecursionCount = 2;
            if (checkBox2.Checked) RecursionCount = 3;
            if (checkBox3.Checked) RecursionCount = 4;
            if (checkBox4.Checked) RecursionCount = 5;
            if (checkBox5.Checked) RecursionCount = 6;
            if (checkBox6.Checked) RecursionCount = 7;
            MaxMoneySet = double.Parse(textBox2.Text);

            List<ComputeClassA> DataList = new List<ComputeClassA>();
            List<ComputeClassA> Res_List = new List<ComputeClassA>();
            foreach (var a in str)
            {
                string[] strTemp = a.Split('#');
                ComputeClassA Temp = new ComputeClassA();
                Temp.ID = strTemp.First();
                Temp.SelecteRes = strTemp[1];
                Temp.odds = double.Parse(strTemp.Last());
                DataList.Add(Temp);
            }
            Recursion_Multibet(DataList, ref Res_List, RecursionCount);

            double Sum = Res_List.Sum(s => 1d / s.odds);
            for (int i = 0; i < Res_List.Count; i++) Res_List[i].Times = 1d / Res_List[i].odds / Sum;

            for (int i = 0; i < Res_List.Count; i++)
            {
                Res_List[i].Times = Math.Round(MaxMoneySet * Res_List[i].Times / 2d);
            }

            WriteToTextBoxF1.Text = "Multiple:" + Res_List.Sum(s => s.Times) + System.Environment.NewLine;
            WriteToTextBoxF1.Text += "Total Cost:" + Res_List.Sum(s => s.Times * 2d) + System.Environment.NewLine;

            Res_List.Sort((y, x) => x.Times.CompareTo(y.Times));
            for (int i = 0; i < Res_List.Count; i++)
            {
                WriteToTextBoxF1.Text += (i + ":" + Res_List[i].ID + "," + RecursionCount + "串1," + Res_List[i].Times + "倍" + "," + Res_List[i].Times * 2 + "元") + System.Environment.NewLine;
                /*string[] strtemps = Res_List[i].ID.Replace(" x ", "#").Split('#');
                StringBuilder result1 = new StringBuilder();
                result1.AppendFormat("{0}\t{1}\t{2}\t{3}\t {4} \t{5}\t {6}", i + ":", strtemps[0]," x " ,strtemps[1], RecursionCount + "串1", Res_List[i].Times + "倍", Res_List[i].Times * 2d + "元"); 
                WriteToTextBoxF1.Text += result1.ToString()+ System.Environment.NewLine; 
                result1.Clear(); */
            }

            WriteToTextBoxF1.Text += System.Environment.NewLine + "Cost List:" + System.Environment.NewLine;

            foreach (var a in Res_List)
            {
                WriteToTextBoxF1.Text += (a.ID + " Bonus:" + (a.odds * a.Times * 2d).ToString("f2")) + System.Environment.NewLine;
            }

            WriteToTextBoxF1.Text += System.Environment.NewLine;
           
            for (int HitsCount = 0; HitsCount <= DataList.Count; HitsCount++)
            {
                /////////Compute Bonus
                double AVGBONUS = 0, COUNTBONUS = 0;
                List<string[]> AllNameTemp = new List<string[]>();
                Recursion_ID(DataList, ref AllNameTemp, HitsCount);


                foreach (var ANT_Item in AllNameTemp)
                {
                    List<string> IDS = new List<string>();
                    IDS.AddRange(ANT_Item.ToArray());
                    double Sums = 0;
                    foreach (var a in Res_List)
                    {
                        int FindStr = 0;
                        for (int j = 0; j < IDS.Count; j++) if (a.ID.IndexOf(IDS[j]) >= 0) FindStr++;
                        if (FindStr == RecursionCount) Sums += a.odds * a.Times * 2d;
                    }
                    AVGBONUS += Sums;
                    COUNTBONUS++;
                }
                AVGBONUS /= (COUNTBONUS == 0 ? 1d : COUNTBONUS);

                WriteToTextBoxF1.Text += DataList.Count + " hits " + HitsCount + " Bonus:" + AVGBONUS.ToString("f2") + System.Environment.NewLine;
            }
             
        }
    }
}
