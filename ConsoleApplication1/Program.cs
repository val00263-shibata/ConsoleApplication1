using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace ConsoleApplication1
{
    class Program
    {
        struct Record
        {
            public string 日付;
            public string 始値;
            public string 高値;
            public string 安値;
            public string 終値;
            public string 前日比;
            public string 出来高;
            public string 貸株残高;
            public string 融資残高;
            public string 貸借倍率;
            public string 逆日歩;
            public string _5DMA;
            public string _25DMA;
            public string _5DVMA;
            public string _25DVMA;
        }

        struct Score : IComparable
        {
            public string コード;
            public double 直近日当たりの上昇率;
            public double 直近傾きの上昇率;
            public double 注文買値;

            public int CompareTo(object obj)
            {
                return ((Score)obj).直近傾きの上昇率 > 直近傾きの上昇率 ? 1 : -1;
            }

            public override string ToString()
            {
                return コード + "," + 直近日当たりの上昇率 + "," + 直近傾きの上昇率 + "," + 注文買値; 
            }
        }

        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(".");
            List<Score> scores = new List<Score>();

            foreach (string s in files)
            {
                if (s.EndsWith(".csv") == true)
                {
                    TextFieldParser parser = new TextFieldParser(s);
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    List<Record> records = new List<Record>();

                    while (parser.EndOfData == false)
                    {
                        Record record = new Record();

                        string[] column = parser.ReadFields();

                        record.日付 = column[0];
                        record.始値 = column[1];
                        record.高値 = column[2];
                        record.安値 = column[3];
                        record.終値 = column[4];
                        record.前日比 = column[5];
                        record.出来高 = column[6];
                        record.貸株残高 = column[7];
                        record.融資残高 = column[8];
                        record.貸借倍率 = column[9];
                        record.逆日歩 = column[10];
                        record._5DMA = column[11];
                        record._25DMA = column[12];
                        record._5DVMA = column[13];
                        record._25DVMA = column[14];

                        records.Add(record);
                    }

                    records.RemoveAt(0);

                    double today = GetTrend(records.GetRange(1, records.Count - 1));
                    double yesterday = GetTrend(records.GetRange(0, records.Count - 1));

                    if (today > 0 && yesterday > 0)
                    {
                        Score score = new Score();

                        score.コード = s.Substring(2, 4);
                        score.直近日当たりの上昇率 = today / double.Parse(records[records.Count - 1].終値);
                        score.直近傾きの上昇率 = today / yesterday;
                        score.注文買値 = GetPrice(records);

                        scores.Add(score);
                    }
                }
            }

            scores.Sort();

            TextWriter tw = new StreamWriter(DateTime.Now.Ticks + ".csv");
            foreach (Score score in scores)
            {
                tw.WriteLine(score.ToString());
            }
            tw.Flush();
        }

        private static double GetPrice(List<Record> records)
        {
            List<double> differ = new List<double>();

            for (int i = 0; i < records.Count - 1; i++)
            {
                if (double.Parse(records[i].終値) > double.Parse(records[i + 1].安値))
                {
                    differ.Add(double.Parse(records[i].終値) - double.Parse(records[i + 1].安値));
                }
            }

            return double.Parse(records[records.Count - 1].終値) - differ.Average() * 3;
        }

        private static double GetTrend(List<Record> records)
        {
            double[] y = new double[records.Count];

            for (int i = 0; i < records.Count; i++)
            {
                y[i] = (double.Parse(records[i].高値) + double.Parse(records[i].安値) + double.Parse(records[i].始値) + double.Parse(records[i].終値)) / 4;
            }

            return CalculateLinearRegression(y);
        }

        static double CalculateLinearRegression(double[] y)
        {
            double[] x = new double[y.Length];

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = i + 1;
            }

            return CalculateLinearRegression(x, y);
        }

        static double CalculateLinearRegression(double[] x, double[] y)
        {
            Tuple<double, double> trend = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(x, y);
            return trend.Item2;
        }
    }
}
