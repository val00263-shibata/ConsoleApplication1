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
            public double 注文買値1;
            public double 注文買値2;
            public double 注文買値3;
            public double minus_min;
            public double minus_ave;
            public double minus_max;
            public double plus_min;
            public double plus_ave;
            public double plus_max;
            public double current_wave;

            public int CompareTo(object obj)
            {
                if (((Score)obj).直近傾きの上昇率 == 直近傾きの上昇率)
                {
                    return 0;
                }
                return ((Score)obj).直近傾きの上昇率 > 直近傾きの上昇率 ? 1 : -1;
            }

            public override string ToString()
            {
                return コード + "," + 直近日当たりの上昇率 + "," + 直近傾きの上昇率 + "," + 注文買値1 + "," + 注文買値2 + "," + 注文買値3 + "," + minus_min + "," + minus_ave + "," + minus_max + "," + plus_min + "," + plus_ave + "," + plus_max + "," + current_wave; 
            }
        }

        private static bool CheckIsStockCSV(string s)
        {
            return s.EndsWith(".csv") == true && s.Length <= 10;
        }

        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(".");
            CheckDuplicateFileSize(files);
            List<Score> scores = new List<Score>();

            foreach (string s in files)
            {
                if (CheckIsStockCSV(s) == true)
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

                    double block1 = GetTrend(records.GetRange(00, records.Count - 40));
                    double block2 = GetTrend(records.GetRange(20, records.Count - 40));
                    double block3 = GetTrend(records.GetRange(40, records.Count - 40));

                    if (block1 < block2 && block2 < block3)
                    {
                        Score score = new Score();

                        score.コード = s.Substring(2, 4);
                        score.直近日当たりの上昇率 = (double.Parse(records[records.Count - 1].終値) + today) / double.Parse(records[records.Count - 1].終値); // 株価の伸び率＝（今日の株価＋傾き）÷今日の株価
                        score.直近傾きの上昇率 = today / yesterday;
                        score.注文買値1 = GetPrice(records, 1);
                        score.注文買値2 = GetPrice(records, 2);
                        score.注文買値3 = GetPrice(records, 3);

                        score = Get_minus_min(records, score);

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

        private static void CheckDuplicateFileSize(string[] files)
        {
            long[] filesizes = new long[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                if (CheckIsStockCSV(files[i]) == false)
                {
                    continue;
                }

                FileInfo fi = new FileInfo(files[i]);

                for (int j = 0; j < filesizes.Length; j++)
                {
                    if (filesizes[j] == fi.Length)
                    {
                        throw new ApplicationException(files[i]);
                    }
                }

                filesizes[i] = fi.Length;
            }
        }

        private static Score Get_minus_min(List<Record> records, Score score)
        {
            List<double[]> waves = new List<double[]>();
            List<double> group = new List<double>();
            List<double> group2 = new List<double>();

            for (int i = 0; i < records.Count; i++)
            {
                if (double.Parse(records[i].前日比) < 0)
                {
                    group.Add(double.Parse(records[i].前日比));

                    waves.Add(group2.ToArray());
                    group2.Clear();
                }
                else
                {
                    group2.Add(double.Parse(records[i].前日比));

                    waves.Add(group.ToArray());
                    group.Clear();
                }
            }

            List<double[]> m_waves = new List<double[]>();
            List<double[]> p_waves = new List<double[]>();
            List<double[]> c_waves = new List<double[]>();

            if (group.Count > 0)
            {
                c_waves.Add(group.ToArray());
            }
            else
            {
                c_waves.Add(group2.ToArray());
            }
            
            for (int i = 0; i < waves.Count; i++)
            {
                if (waves[i].Length == 0)
                {
                    continue;
                }

                if (waves[i][0] < 0)
                {
                    m_waves.Add(waves[i]);
                }
                else
                {
                    p_waves.Add(waves[i]);
                }
            }

            List<double> m_waves_sum = new List<double>();
            List<double> p_waves_sum = new List<double>();
            List<double> c_waves_sum = new List<double>();

            for (int i = 0; i < m_waves.Count; i++)
            {
                m_waves_sum.Add(m_waves[i].Sum());
            }
            for (int i = 0; i < p_waves.Count; i++)
            {
                p_waves_sum.Add(p_waves[i].Sum());
            }
            for (int i = 0; i < c_waves.Count; i++)
            {
                c_waves_sum.Add(c_waves[i].Sum());
            }

            score.minus_min = m_waves_sum.Min();
            score.minus_ave = m_waves_sum.Average();
            score.minus_max = m_waves_sum.Max();

            score.plus_min = p_waves_sum.Min();
            score.plus_ave = p_waves_sum.Average();
            score.plus_max = p_waves_sum.Max();

            score.current_wave = c_waves_sum[0];

            return score;
        }

        private static double GetPrice(List<Record> records, byte rate)
        {
            List<double> differ = new List<double>();

            for (int i = 0; i < records.Count - 1; i++)
            {
                if (double.Parse(records[i].終値) > double.Parse(records[i + 1].安値))
                {
                    differ.Add(double.Parse(records[i].終値) - double.Parse(records[i + 1].安値));
                }
            }

            return double.Parse(records[records.Count - 1].終値) - (differ.Average() * rate);
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
