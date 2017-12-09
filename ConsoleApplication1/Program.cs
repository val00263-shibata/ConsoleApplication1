using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq.Expressions;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

/*/

get header
latest price
加重平均
_25DMA上昇回数

/*/

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
            public string 特別空売り料;
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
            public double 最低株価;
            public double 株価;
            public double 最高株価;
            public double 加重平均株価;
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
            public byte _25DMA上昇回数;
            public ulong 取引量;
            public bool is25up;
            public double _25DMA乖離率;
            public double 加重平均乖離率;
            public bool is25RateUP;
            public bool convex;
            public ushort one_per_win;
            public string price_line;
            public double mapping_candle;
            public string price_line2;
            public double price_line3;
            public string 貸借倍率;

            public override string ToString()
            {
                return
                    ""  + コード +
                    "," + 直近日当たりの上昇率 +
                    "," + 直近傾きの上昇率 +
                    "," + 最低株価 +
                    "," + 株価 +
                    "," + 最高株価 +
                    "," + 加重平均株価 +
                    "," + 注文買値1 +
                    "," + 注文買値2 +
                    "," + 注文買値3 +
                    "," + minus_min +
                    "," + minus_ave +
                    "," + minus_max +
                    "," + plus_min +
                    "," + plus_ave +
                    "," + plus_max +
                    "," + current_wave +
                    "," + _25DMA上昇回数 +
                    "," + 取引量 +
                    "," + is25up +
                    "," + _25DMA乖離率 +
                    "," + 加重平均乖離率 +
                    "," + is25RateUP +
                    "," + convex +
                    "," + one_per_win +
                    "," + price_line +
                    "," + mapping_candle +
                    "," + price_line2 +
                    "," + price_line3 +
                    "," + 貸借倍率 +
                    "";
            }

            internal static string GetHeader()
            {
                Score score = new Score();

                return
                    ""  + GetName(() => score.コード) +
                    "," + GetName(() => score.直近日当たりの上昇率) +
                    "," + GetName(() => score.直近傾きの上昇率) +
                    "," + GetName(() => score.最低株価) +
                    "," + GetName(() => score.株価) +
                    "," + GetName(() => score.最高株価) +
                    "," + GetName(() => score.加重平均株価) +
                    "," + GetName(() => score.注文買値1) +
                    "," + GetName(() => score.注文買値2) +
                    "," + GetName(() => score.注文買値3) +
                    "," + GetName(() => score.minus_min) +
                    "," + GetName(() => score.minus_ave) +
                    "," + GetName(() => score.minus_max) +
                    "," + GetName(() => score.plus_min) +
                    "," + GetName(() => score.plus_ave) +
                    "," + GetName(() => score.plus_max) +
                    "," + GetName(() => score.current_wave) +
                    "," + GetName(() => score._25DMA上昇回数) +
                    "," + GetName(() => score.取引量) +
                    "," + GetName(() => score.is25up) +
                    "," + GetName(() => score._25DMA乖離率) +
                    "," + GetName(() => score.加重平均乖離率) +
                    "," + GetName(() => score.is25RateUP) +
                    "," + GetName(() => score.convex) +
                    "," + GetName(() => score.one_per_win) +
                    "," + GetName(() => score.price_line) +
                    "," + GetName(() => score.mapping_candle) +
                    "," + GetName(() => score.price_line2) +
                    "," + GetName(() => score.price_line3) +
                    "," + GetName(() => score.貸借倍率) +
                    "";
            }

            private static string GetName<T>(Expression<Func<T>> e)
            {
                var member = (MemberExpression)e.Body;
                return member.Member.Name;
            }

            public int CompareTo(object obj)
            {
                if (((Score)obj).直近日当たりの上昇率 == 直近日当たりの上昇率)
                {
                    return 0;
                }
                return ((Score)obj).直近日当たりの上昇率 > 直近日当たりの上昇率 ? 1 : -1;
            }
        }

        private static bool CheckIsStockCSV(string s)
        {
            return s.EndsWith(".csv") == true && s.Length <= 10;
        }

        private static void TestIncludeMapping()
        {
            Record record100200 = new Record();
            record100200.始値 = "100";
            record100200.終値 = "200";

            Record record150250 = new Record();
            record150250.始値 = "150";
            record150250.終値 = "250";
            Record record050150 = new Record();
            record050150.始値 = "50";
            record050150.終値 = "150";
            Record record110190 = new Record();
            record110190.始値 = "110";
            record110190.終値 = "190";
            Record record090210 = new Record();
            record090210.始値 = "90";
            record090210.終値 = "210";

            Debug.Assert(IncludeMapping(record100200, record150250) == true);
            Debug.Assert(IncludeMapping(record100200, record050150) == true);
            Debug.Assert(IncludeMapping(record100200, record110190) == true);
            Debug.Assert(IncludeMapping(record100200, record090210) == true);

            Record record250150 = new Record();
            record250150.始値 = "250";
            record250150.終値 = "150";
            Record record150050 = new Record();
            record150050.始値 = "150";
            record150050.終値 = "50";
            Record record190110 = new Record();
            record190110.始値 = "190";
            record190110.終値 = "110";
            Record record210090 = new Record();
            record210090.始値 = "210";
            record210090.終値 = "90";

            Debug.Assert(IncludeMapping(record100200, record250150) == true);
            Debug.Assert(IncludeMapping(record100200, record150050) == true);
            Debug.Assert(IncludeMapping(record100200, record190110) == true);
            Debug.Assert(IncludeMapping(record100200, record210090) == true);

            Record record200100 = new Record();
            record200100.始値 = "200";
            record200100.終値 = "100";

            Debug.Assert(IncludeMapping(record200100, record150250) == true);
            Debug.Assert(IncludeMapping(record200100, record050150) == true);
            Debug.Assert(IncludeMapping(record200100, record110190) == true);
            Debug.Assert(IncludeMapping(record200100, record090210) == true);

            Debug.Assert(IncludeMapping(record200100, record250150) == true);
            Debug.Assert(IncludeMapping(record200100, record150050) == true);
            Debug.Assert(IncludeMapping(record200100, record190110) == true);
            Debug.Assert(IncludeMapping(record200100, record210090) == true);

            Record record080090 = new Record();
            record080090.始値 = "80";
            record080090.終値 = "90";
            Record record090080 = new Record();
            record090080.始値 = "90";
            record090080.終値 = "80";

            Debug.Assert(IncludeMapping(record100200, record080090) == false);
            Debug.Assert(IncludeMapping(record100200, record090080) == false);
            Debug.Assert(IncludeMapping(record200100, record080090) == false);
            Debug.Assert(IncludeMapping(record200100, record090080) == false);

            Record record210220 = new Record();
            record210220.始値 = "210";
            record210220.終値 = "220";
            Record record220210 = new Record();
            record220210.始値 = "220";
            record220210.終値 = "210";

            Debug.Assert(IncludeMapping(record100200, record210220) == false);
            Debug.Assert(IncludeMapping(record100200, record220210) == false);
            Debug.Assert(IncludeMapping(record200100, record210220) == false);
            Debug.Assert(IncludeMapping(record200100, record220210) == false);
        }

        static void Main(string[] args)
        {
            TestIncludeMapping();

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

                        if (column.Length == 16)
                        {
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
                            record.特別空売り料 = column[11];
                            record._5DMA = column[12];
                            record._25DMA = column[13];
                            record._5DVMA = column[14];
                            record._25DVMA = column[15];
                        }
                        else if (column.Length == 11)
                        {
                            record.日付 = column[0];
                            record.始値 = column[1];
                            record.高値 = column[2];
                            record.安値 = column[3];
                            record.終値 = column[4];
                            record.前日比 = "0";
                            record.出来高 = column[5];
                            record.貸株残高 = "0";
                            record.融資残高 = "0";
                            record.貸借倍率 = "0";
                            record.逆日歩 = "0";
                            record.特別空売り料 = "0";
                            record._5DMA = column[9];
                            record._25DMA = column[10];
                            record._5DVMA = "0";
                            record._25DVMA = "0";
                        }
                        else
                        {
                            throw new Exception(column.Length.ToString());
                        }

                        records.Add(record);
                    }

                    records.RemoveAt(0);

                    CheckDateSort(records);

                    double today = GetTrend(records.GetRange(1, records.Count - 1));
                    double yesterday = GetTrend(records.GetRange(0, records.Count - 1));

                    double block1 = GetTrend(records.GetRange(00, records.Count - 40));
                    double block2 = GetTrend(records.GetRange(20, records.Count - 40));
                    double block3 = GetTrend(records.GetRange(40, records.Count - 40));

                    //if (0 < block1 && block1 < block2 && block2 < block3)
                    {
                        Score score = new Score();

                        score.コード = s.Substring(2, 4);
                        score.最低株価 = GetLowPrice(records);
                        score.株価 = double.Parse(records[records.Count - 1].終値);
                        score.最高株価 = GetHighPrice(records);
                        score.加重平均株価 = GetWeightAveragePrice(records);
                        score.直近日当たりの上昇率 = (score.株価 + today) / score.株価; // 株価の伸び率＝（今日の株価＋傾き）÷今日の株価
                        score.直近傾きの上昇率 = today / yesterday;
                        score.注文買値1 = GetPrice(records, 1);
                        score.注文買値2 = GetPrice(records, 2);
                        score.注文買値3 = GetPrice(records, 3);
                        score._25DMA上昇回数 = GetUpCount25DMA(records);
                        score.取引量 = (ulong)GetAllAmount(records);
                        score.is25up = double.Parse(records[records.Count - 2]._25DMA) < double.Parse(records[records.Count - 1]._25DMA);
                        score._25DMA乖離率 = (double.Parse(records[records.Count - 1].終値) / double.Parse(records[records.Count - 1]._25DMA) - 1) * 100;
                        score.加重平均乖離率 = score.株価 / score.加重平均株価;
                        score.is25RateUP = GetIs25RateUP(records);
                        score.convex = GetConvex(records);
                        score.one_per_win = GetOnePerWin(records);
                        score.price_line = GetPriceLine(records);
                        score.mapping_candle = GetMappingCandle(records);
                        score.price_line2 = GetPriceLine2(records);
                        score.price_line3 = GetPriceLine3(records);
                        score.貸借倍率 = CutComma(records[records.Count - 1].貸借倍率);

                        score = Get_minus_min(records, score);

                        scores.Add(score);
                    }
                }
            }

            scores.Sort();

            TextWriter tw = new StreamWriter(DateTime.Now.Ticks + ".csv");
            tw.WriteLine(Score.GetHeader());
            foreach (Score score in scores)
            {
                tw.WriteLine(score.ToString());
            }
            tw.Flush();
        }

        private static string GetPriceLine2(List<Record> records)
        {
            Dictionary<string, double> hashtable = new Dictionary<string, double>();

            foreach (Record record in records)
            {
                double price = (double.Parse(record.高値) + double.Parse(record.安値) + double.Parse(record.始値) + double.Parse(record.終値)) / 4;
                string[] price_str = price.ToString().Split(new char[] { '.' });

                if (price_str.Length == 1)
                {
                    price_str[0] += ".0";
                }
                else
                {
                    price_str[0] += "." + price_str[1].Substring(0, 1);
                }

                try
                {
                    hashtable[price_str[0]] += double.Parse(record.出来高) * price;
                }
                catch (KeyNotFoundException)
                {
                    hashtable[price_str[0]] = double.Parse(record.出来高) * price;
                }
            }

            double max = double.MinValue;
            string key = "";

            foreach (KeyValuePair<string, double> de in hashtable)
            {
                if (max < de.Value)
                {
                    max = de.Value;
                    key = de.Key;
                }
            }

            return CutComma(key);
        }

        private struct Box
        {
            public double volume;
            public Range range;
        }

        private struct Range
        {
            public double up;
            public double down;
        }

        private static double GetPriceLine3(List<Record> records)
        {
            Box[] boxes = new Box[1000];

            double lowprice = GetLowPrice(records);
            double highprice = GetHighPrice(records);
            double diff = (highprice - lowprice) / boxes.Length;

            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i] = new Box();
                boxes[i].volume = 0.0;
                boxes[i].range = new Range();

                boxes[i].range.down = lowprice + diff * i;
                boxes[i].range.up = lowprice + diff * (i + 1);
            }

            Dictionary<string, double> hashtable = new Dictionary<string, double>();

            foreach (Record record in records)
            {
                double price = (double.Parse(record.高値) + double.Parse(record.安値) + double.Parse(record.始値) + double.Parse(record.終値)) / 4;
                bool check = false;

                for (int i = 0; i < boxes.Length; i++)
                {
                    if (boxes[i].range.down <= price && price <= boxes[i].range.up)
                    {
                        boxes[i].volume += double.Parse(record.出来高) * price;
                        check = true;
                        break;
                    }
                }

                if (check == false)
                {
                    throw new Exception();
                }
            }

            double cmp = double.MinValue;
            int index = -1;

            for (int i = 0; i < boxes.Length; i++)
            {
                if (cmp < boxes[i].volume)
                {
                    cmp = boxes[i].volume;
                    index = i;
                }
            }

            return (boxes[index].range.up + boxes[index].range.down) / 2;
        }

        private static string CutComma(string str)
        {
            return str.Replace(",", "");
        }

        private static double GetMappingCandle(List<Record> records)
        {
            List<int> mapping_volumes = new List<int>(new int[records.Count]);
            int i = -1;

            foreach (Record record in records)
            {
                i++;

                foreach (Record cmp_record in records)
                {
                    if (record.日付 == cmp_record.日付)
                    {
                        continue;
                    }

                    if (IncludeMapping(record, cmp_record) == true)
                    {
                        mapping_volumes[i] += int.Parse(CutComma(cmp_record.出来高));
                    }
                }
            }

            int max = int.MinValue;
            int max_index = int.MinValue;
            i = -1;

            foreach (int mapping_volume in mapping_volumes)
            {
                i++;

                if (max < mapping_volume)
                {
                    max = mapping_volume;
                    max_index = i;
                }
            }

            return (double.Parse(records[max_index].始値) + double.Parse(records[max_index].終値)) / 2;
        }

        private static bool IncludeMapping(Record record, Record cmp_record)
        {
            if (double.Parse(record.始値) <= double.Parse(record.終値))
            {
                if (double.Parse(cmp_record.始値) <= double.Parse(cmp_record.終値))
                {
                    if (double.Parse(record.終値) >= double.Parse(cmp_record.始値) && double.Parse(record.始値) <= double.Parse(cmp_record.終値))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (double.Parse(record.始値) <= double.Parse(cmp_record.始値) && double.Parse(record.終値) >= double.Parse(cmp_record.終値))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (double.Parse(cmp_record.始値) <= double.Parse(cmp_record.終値))
                {
                    if (double.Parse(record.始値) >= double.Parse(cmp_record.始値) && double.Parse(record.終値) <= double.Parse(cmp_record.終値))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (double.Parse(record.終値) <= double.Parse(cmp_record.始値) && double.Parse(record.始値) >= double.Parse(cmp_record.終値))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private static string GetPriceLine(List<Record> records)
        {
            Hashtable hashtable = new Hashtable();
            int i = 0;

            foreach (Record record in records)
            {
                if (hashtable[records[i].始値] == null)
                {
                    hashtable[records[i].始値] = double.Parse(records[i].出来高) / 4;
                }
                else
                {
                    hashtable[records[i].始値] = double.Parse(records[i].出来高) / 4 + (double)hashtable[records[i].始値];
                }

                if (hashtable[records[i].終値] == null)
                {
                    hashtable[records[i].終値] = double.Parse(records[i].出来高) / 4;
                }
                else
                {
                    hashtable[records[i].終値] = double.Parse(records[i].出来高) / 4 + (double)hashtable[records[i].終値];
                }

                if (hashtable[records[i].安値] == null)
                {
                    hashtable[records[i].安値] = double.Parse(records[i].出来高) / 4;
                }
                else
                {
                    hashtable[records[i].安値] = double.Parse(records[i].出来高) / 4 + (double)hashtable[records[i].安値];
                }

                if (hashtable[records[i].高値] == null)
                {
                    hashtable[records[i].高値] = double.Parse(records[i].出来高) / 4;
                }
                else
                {
                    hashtable[records[i].高値] = double.Parse(records[i].出来高) / 4 + (double)hashtable[records[i].高値];
                }

                i++;
            }

            double max = double.MinValue;
            string key = "";

            foreach (DictionaryEntry de in hashtable)
            {
                if (max < (double)de.Value)
                {
                    max = (double)de.Value;
                    key = (string)de.Key;
                }
            }

            return CutComma(key);
        }

        private static ushort GetOnePerWin(List<Record> records)
        {
            ushort ret = 0;

            for (int i = 0; i < records.Count - 5; i++)
            {
                double price = double.Parse(records[i].始値);

                if (false)
                {
                }
                else if (price * 1.01 < double.Parse(records[i + 1].高値))
                {
                    ret++;
                }
                else if (price * 1.01 < double.Parse(records[i + 2].高値))
                {
                    ret++;
                }
                else if (price * 1.01 < double.Parse(records[i + 3].高値))
                {
                    ret++;
                }
                else if (price * 1.01 < double.Parse(records[i + 4].高値))
                {
                    ret++;
                }
                else if (price * 1.01 < double.Parse(records[i + 5].高値))
                {
                    ret++;
                }
            }

            return ret;
        }

        private static double GetLowPrice(List<Record> records)
        {
            double ret = double.MaxValue;

            foreach (Record record in records)
            {
                double price = (double.Parse(record.高値) + double.Parse(record.安値) + double.Parse(record.始値) + double.Parse(record.終値)) / 4;
                ret = price < ret ? price : ret;
            }

            return ret;
        }

        private static double GetHighPrice(List<Record> records)
        {
            double ret = double.MinValue;

            foreach (Record record in records)
            {
                double price = (double.Parse(record.高値) + double.Parse(record.安値) + double.Parse(record.始値) + double.Parse(record.終値)) / 4;
                ret = price > ret ? price : ret;
            }

            return ret;
        }

        private static bool GetConvex(List<Record> records)
        {
            return GetConvex(records[records.Count - 3], records[records.Count - 2], records[records.Count - 1]);
        }

        private static bool GetConvex(Record 二日前, Record 一日前, Record 本日)
        {
            double 二日前_25DMA = double.Parse(二日前._25DMA);
            double 一日前_25DMA = double.Parse(一日前._25DMA);
            double 本日_25DMA = double.Parse(本日._25DMA);
            
            return 本日_25DMA / 一日前_25DMA > 一日前_25DMA / 二日前_25DMA;
        }

        private static bool GetIs25RateUP(List<Record> records)
        {
            double prevday_price = double.Parse(records[records.Count - 2].終値);
            double prevday_25DMA = double.Parse(records[records.Count - 2]._25DMA);
            double prevday_rate = prevday_price / prevday_25DMA;

            double lastday_price = double.Parse(records[records.Count - 1].終値);
            double lastday_25DMA = double.Parse(records[records.Count - 1]._25DMA);
            double lastday_rate = lastday_price / lastday_25DMA;

            return prevday_rate < lastday_rate;
        }

        private static double GetAllAmount(List<Record> records)
        {
            double ret = 0;

            for (int i = 0; i < records.Count; i++)
            {
                double price = (double.Parse(records[i].高値) + double.Parse(records[i].安値) + double.Parse(records[i].始値) + double.Parse(records[i].終値)) / 4;
                ret += price * double.Parse(records[i].出来高);
            }

            return ret;
        }

        private static byte GetUpCount25DMA(List<Record> records)
        {
            byte ret = 0;

            for (int i = 0; i < records.Count - 1; i++)
            {
                if (double.Parse(records[i]._25DMA) < double.Parse(records[i + 1]._25DMA))
                {
                    ret++;
                }
            }

            return ret;
        }

        private static double GetWeightAveragePrice(List<Record> records)
        {
            double ret = 0.0;
            double vol_sum = 0.0;

            for (int i = 0; i < records.Count; i++)
            {
                ret += double.Parse(records[i].終値) * double.Parse(records[i].出来高);
                vol_sum += double.Parse(records[i].出来高);
            }

            return ret / vol_sum;
        }

        private static void CheckDateSort(List<Record> records)
        {
            CheckDateSort(records[0], records[1]);
        }

        private static void CheckDateSort(Record 一番目, Record 二番目)
        {
            DateTime first_record_date  = DateTime.Parse(一番目.日付.Trim('\"'));
            DateTime second_record_date = DateTime.Parse(二番目.日付.Trim('\"'));
            
            if(first_record_date < second_record_date)
            {
            }
            else
            {
                throw new ApplicationException();
            }
        }

        private static void CheckDuplicateFileSize(string[] files)
        {
            long[] filesizes = new long[files.Length];
            string[] second_records = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                if (CheckIsStockCSV(files[i]) == false)
                {
                    continue;
                }

                FileInfo fi = new FileInfo(files[i]);

                StreamReader sr = fi.OpenText();
                sr.ReadLine();
                string second_record = sr.ReadLine();

                for (int j = 0; j < filesizes.Length; j++)
                {
                    if (filesizes[j] == fi.Length && second_records[j] == second_record)
                    {
                        throw new ApplicationException(files[i]);
                    }
                }

                filesizes[i] = fi.Length;
                second_records[i] = second_record;
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

            score.current_wave = c_waves_sum[0];

            if(m_waves_sum.Count == 0 || p_waves_sum.Count == 0)
            {
                return score;
            }

            score.minus_min = m_waves_sum.Min();
            score.minus_ave = m_waves_sum.Average();
            score.minus_max = m_waves_sum.Max();

            score.plus_min = p_waves_sum.Min();
            score.plus_ave = p_waves_sum.Average();
            score.plus_max = p_waves_sum.Max();

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
