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
        /*
        25日に対して
        傾きが上がっている直近1年分から日数のカウント
        latest傾き度
        max傾き度
        回帰直線
        */

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

        struct Score
        {
            public short code;
            public short days;
            public double latest;
            public double max;
            public double min;
            public double sum;
            public double trend;
        }

        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(".");
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

                    Console.WriteLine(s);

                    Console.WriteLine(today / double.Parse(records[records.Count - 1].終値));

                    //Console.WriteLine(yesterday);
                    //Console.WriteLine(today);
                    //Console.WriteLine(records.GetRange(1, records.Count - 1).Count);
                    //Console.WriteLine(records.GetRange(0, records.Count - 1).Count);

                    //if (today > 0 && yesterday > 0 && today > yesterday)
                    if (today > 0 && yesterday > 0)
                    {
                        //Console.Write("today / yesterday : ");
                        Console.WriteLine(today / yesterday);
                    }
                }
            }

            Console.Read();
        }

        private static double GetRate(List<Record> records, int index)
        {
            return double.Parse(records[index]._25DMA) / double.Parse(records[index - 1]._25DMA);
        }

        private static double GetMax(List<Record> records)
        {
            double ret = 0.0;

            for (int i = 1; i < records.Count; i++)
            {
                double buf = GetRate(records, i);

                if (buf > ret)
                {
                    ret = buf;
                }
            }

            return ret;
        }

        private static double GetMin(List<Record> records)
        {
            double ret = double.MaxValue;

            for (int i = 1; i < records.Count; i++)
            {
                double buf = GetRate(records, i);

                if (buf < ret)
                {
                    ret = buf;
                }
            }

            return ret;
        }

        private static double GetSum(List<Record> records)
        {
            double ret = 0.0;

            for (int i = 1; i < records.Count; i++)
            {
                ret += GetRate(records, i);
            }

            return ret;
        }

        private static double GetLatest(List<Record> records)
        {
            return GetRate(records, records.Count - 1);
        }

        private static short GetDays(List<Record> records)
        {
            short ret = 0;

            for (int i = 1; i < records.Count; i++)
            {
                if (GetRate(records, i) > 1.0)
                {
                    ret++;
                }
            }

            return ret;
        }

        private static double GetTrend(List<Record> records)
        {
            double[] y = new double[records.Count];

            for (int i = 0; i < records.Count; i++)
            {
                //y[i] = double.Parse(records[i]._25DMA);
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
