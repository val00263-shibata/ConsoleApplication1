using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static int csv_line_count = 500;

        static int[] nikkei225 = new int[] {
4151,
4502,
4503,
4506,
4507,
4519,
4523,
4568,
6479,
6501,
6502,
6503,
6504,
6506,
6508,
6674,
6701,
6702,
6703,
6752,
6753,
6758,
6762,
6767,
6770,
6773,
6841,
6857,
6902,
6952,
6954,
6971,
6976,
7735,
7751,
7752,
8035,
7201,
7202,
7203,
7205,
7211,
7261,
7267,
7269,
7270,
4543,
4902,
7731,
7733,
7762,
9412,
9432,
9433,
9437,
9613,
9984,
8303,
8304,
8306,
8308,
8309,
8316,
8331,
8332,
8354,
8355,
8411,
8253,
8601,
8604,
8628,
8630,
8725,
8729,
8750,
8766,
8795,
1332,
1333,
2002,
2269,
2282,
2501,
2502,
2503,
2531,
2801,
2802,
2871,
2914,
3086,
3099,
3382,
8233,
8252,
8267,
8270,
9983,
4324,
4689,
4704,
9602,
9681,
9735,
9766,
1605,
3101,
3103,
3105,
3401,
3402,
3861,
3863,
3865,
3405,
3407,
4004,
4005,
4021,
4041,
4042,
4043,
4061,
4063,
4183,
4188,
4208,
4272,
4452,
4901,
4911,
6988,
5002,
5020,
5101,
5108,
3110,
5201,
5202,
5214,
5232,
5233,
5301,
5332,
5333,
5401,
5406,
5411,
5413,
5541,
3436,
5703,
5706,
5707,
5711,
5713,
5714,
5715,
5801,
5802,
5803,
5901,
2768,
8001,
8002,
8015,
8031,
8053,
8058,
1721,
1801,
1802,
1803,
1812,
1925,
1928,
1963,
5631,
6103,
6113,
6301,
6302,
6305,
6326,
6361,
6366,
6367,
6471,
6472,
6473,
7004,
7011,
7013,
7003,
7012,
7911,
7912,
7951,
3289,
8801,
8802,
8803,
8804,
8830,
9001,
9005,
9007,
9008,
9009,
9020,
9021,
9022,
9062,
9064,
9101,
9104,
9107,
9202,
9301,
9501,
9502,
9503,
9531,
9532
};

        static void Main(string[] args)
        {
            args = new string[] {"7777"};
            args = GetFromWebAll();
            args = new string[225];

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = nikkei225[i].ToString();
                string url = "http://k-db.com/stocks/" + args[i] + "-T/a?download=csv";

                System.Net.WebClient wc = new System.Net.WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;
                string source = wc.DownloadString(url);
                wc.Dispose();

                int count = 0;
                string[] lines = source.Split(new char[] { '\n' });

                List<string> lines_list = new List<string>(lines);
                lines_list.Reverse();
                lines_list.RemoveAt(0);
                lines = lines_list.ToArray();

                for (int j = 0; j < lines.Length; j++)
                {
                    string[] colomns = lines[j].Split(new char[] { ',' });

                    if (colomns.Length != 8)
                    {
                        continue;
                    }
                    try
                    {
                        Convert.ToDouble(colomns[4]);
                    }
                    catch(FormatException)
                    {
                        continue;
                    }

                    for (int k = j + 1; k < lines.Length; k++)
                    {
                        string[] next_colomns = lines[k].Split(new char[] { ',' });

                        if (next_colomns.Length != 8)
                        {
                            continue;
                        }
                        try
                        {
                            Convert.ToDouble(next_colomns[4]);
                        }
                        catch (FormatException)
                        {
                            continue;
                        }

                        if(Convert.ToDouble(colomns[4]) < Convert.ToDouble(next_colomns[4]))
                        {
                            count++;
                        }

                        break;
                    }
                }

                Console.WriteLine(args[i] + " : " + count);
            }

            Console.Read();
        }

        private static string[] GetFromWebAll()
        {
            string list = "http://k-db.com/stocks/" + DateTime.Today.ToString("yyyy-MM-dd") + "?download=csv";

            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            string source = wc.DownloadString(list);
            wc.Dispose();

            string[] corps = source.Split(new char[] { '\n' });

            List<string> codes = new List<string>();

            for (int i = 1; i < corps.Length; i++)
            {
                string[] colomns = corps[i].Split(new char[] { ',' });

                try
                {
                    Convert.ToInt16(colomns[0].Substring(0, 4));
                    codes.Add(colomns[0].Substring(0, 4));
                }
                catch
                {
                }
            }

            return codes.ToArray();
        }

        static void Main2(string[] args)
        {
            //ProcessStock3();
            //ProcessStock2();

            Stock[] priority_stock = JoinStock(args);
            PrintStock(priority_stock);

            Code[] csv_code = GetFromWeb();
            Stock[] csv_stock = GenerateStock(csv_code);

            Stock[] filtered_stock = FilterStock(csv_stock);
            PrintStock(filtered_stock);

            Console.Read();
        }

        private static void PrintStock(Stock[] filtered_stock)
        {
            for (int i = 0; i < filtered_stock.Length; i++)
            {
                Console.WriteLine(filtered_stock[i].code + " : " + filtered_stock[i].score);
            }

            Console.WriteLine("");
        }

        private static Stock[] JoinStock(string[] priority_number)
        {
            if (priority_number == null)
            {
                return new Stock[0];
            }
            if (priority_number.Length == 0)
            {
                return new Stock[0];
            }

            Code[] priority_code = new Code[priority_number.Length];

            for (int i = 0; i < priority_code.Length; i++)
            {
                priority_code[i] = new Code();
                priority_code[i].code = priority_number[i];
            }

            List<Stock> ok = new List<Stock>(GenerateStock(priority_code));
            ok.Sort();
            return ok.ToArray();
        }

        private static Stock[] FilterStock(Stock[] csv_stock)
        {
            List<Stock> ok = new List<Stock>();

            for (int i = 0; i < csv_stock.Length; i++)
            {
                if (csv_stock[i].trend > 0)
                {
                    if (csv_stock[i].start_price < csv_stock[i].average_price && csv_stock[i].average_price < csv_stock[i].last_price)
                    {
                        if (csv_stock[i].start_volume < csv_stock[i].average_volume && csv_stock[i].average_price < csv_stock[i].last_price)
                        {
                            if (csv_stock[i].score < 1.0)
                            {
                                ok.Add(csv_stock[i]);
                            }
                        }
                    }
                }
            }

            ok.Sort();
            return ok.ToArray();
        }

        private static Stock[] GenerateStock(Code[] csv_code)
        {
            List<Stock> stocks = new List<Stock>();

            for (int i = 0; i < csv_code.Length; i++)
            {
                string url = "http://k-db.com/stocks/" + csv_code[i].code + "-T/a?download=csv";

                System.Net.WebClient wc = new System.Net.WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;
                string source = wc.DownloadString(url);
                wc.Dispose();

                string[] lines = source.Split(new char[] { '\n' });

                List<string> lines_list = new List<string>(lines);
                lines_list.Reverse();
                lines_list.RemoveAt(0);
                lines = lines_list.ToArray();

                Stock stock = new Stock();
                stock.code = csv_code[i].code;
                stock.records = new List<Record>();

                for (int j = 0; j < csv_line_count; j++)
                {
                    string[] colomns = lines[j].Split(new char[] { ',' });

                    if (colomns.Length == 8)
                    {
                    }
                    else
                    {
                        stock.code = null;
                        break;
                    }

                    Record record = new Record();

                    try
                    {
                        record.price1 = Convert.ToDouble(colomns[2]);
                        record.price2 = Convert.ToDouble(colomns[3]);
                        record.price3 = Convert.ToDouble(colomns[4]);
                        record.price4 = Convert.ToDouble(colomns[5]);
                        record.volume = Convert.ToInt32(colomns[6]);

                        record.isAM = j % 2 == 0;

                        stock.records.Add(record);
                    }
                    catch
                    {
                    }
                }

                if (stock.code == null)
                {
                }
                else if (stock.records.Count == 0)
                {
                }
                else
                {
                    stock.start_price = stock.records[0].price1;
                    stock.last_price = stock.records[stock.records.Count - 1].price4;
                    //stock.average_price = CalcAveragePrice(stock.records.ToArray());

                    //stock.trend = GetTrendRate(ConvertToDoubleArray(stock.records));

                    //Console.Write("stock.trend : "); Console.WriteLine(stock.trend);
                    double[] y = ConvertToDoubleArray(stock.records);
                    double[] x = new double[y.Length];
                    for (int k = 0; k < x.Length; k++)
                    {
                        x[k] = k + 1;
                    }
                    Tuple<double, double> ret = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(x, y);
                    //Console.Write("SimpleRegression.Fit"); Console.WriteLine(ret.Item1);
                    //Console.Write("SimpleRegression.Fit : "); Console.WriteLine(ret.Item2);
                    stock.trend = ret.Item2;

                    stock.start_volume = stock.records[0].volume;
                    stock.last_volume = stock.records[stock.records.Count - 1].volume;
                    //stock.average_volume = CalcAverageVolume(stock.records.ToArray());

                    stocks.Add(stock);
                }
            }

            return stocks.ToArray();
        }

        private static Code[] GetFromWeb()
        {
            string list = "http://k-db.com/stocks/" + DateTime.Today.ToString("yyyy-MM-dd") + "?download=csv";

            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            string source = wc.DownloadString(list);
            wc.Dispose();

            string[] corps = source.Split(new char[] { '\n' });

            List<Code> codes = new List<Code>();

            for (int i = 0; i < corps.Length; i++)
            {
                string[] colomns = corps[i].Split(new char[] { ',' });
                Code code = new Code();

                try
                {
                    code.code = colomns[0].Substring(0, 4);
                    code.amount = Convert.ToInt64(colomns[9]);

                    codes.Add(code);
                }
                catch
                {
                }
            }

            codes.Sort();
            codes = codes.GetRange(0, 200);
            return codes.ToArray();
        }

        class Record
        {
            public bool isAM;
            public double price1;
            public double price2;
            public double price3;
            public double price4;
            public int volume;
            public double price
            {
                get
                {
                    //return (price1 + price2 + price3 + price4) / 4;

                    double buf1;
                    double buf2;
                    double buf3;
                    double buf4;

                    if (isAM)
                    {
                        buf1 = price1 * volume * 3 / 9;
                        buf4 = price4 * volume * 2 / 9;
                    }
                    else
                    {
                        buf1 = price1 * volume * 2 / 9;
                        buf4 = price4 * volume * 3 / 9;
                    }

                    buf2 = price2 * volume * 2 / 9;
                    buf3 = price3 * volume * 2 / 9;

                    return (buf1 + buf2 + buf3 + buf4) / volume;
                }
            }
        }

        class Code : IComparable
        {
            public string code;
            public long amount;

            public int CompareTo(object obj)
            {
                if (((Code)obj).amount > amount)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        class Stock : IComparable
        {
            public string code;
            public double start_price;
            public double last_price;
            public int start_volume;
            public int last_volume;
            public List<Record> records;
            public double trend;
            public double score
            {
                get
                {
                    double basis = average_price;
                    basis += trend * records.Count / 2;
                    return last_price / basis;
                }
            }

            public int average_volume
            {
                get
                {
                    int ret = 0;

                    for (int i = 0; i < records.Count; i++)
                    {
                        ret += records[i].volume;
                    }

                    return ret / records.Count;
                }
            }

            public double average_price
            {
                get
                {
                    double ret = 0.0;
                    long vol_sum = 0;

                    for (int i = 0; i < records.Count; i++)
                    {
                        ret += records[i].price * records[i].volume;
                        vol_sum += records[i].volume;
                    }

                    return ret / vol_sum;
                }
            }

            public double average_price_bak
            {
                get
                {
                    double ret = 0.0;

                    for (int i = 0; i < records.Count; i++)
                    {
                        ret += records[i].price;
                    }

                    return ret / records.Count;
                }
            }

            public int CompareTo(object obj)
            {
                //return (int)(score - ((Stock)obj).score);
                //return (int)(((Stock)obj).score - score);

                if (((Stock)obj).score > score)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        private static void ProcessStock2()
        {
            string list = "http://k-db.com/stocks/" + DateTime.Today.ToString("yyyy-MM-dd") + "?download=csv";
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            string source = wc.DownloadString(list);
            wc.Dispose();

            string[] corps = source.Split(new char[] { '\n' });

            List<Code> codes = new List<Code>();

            for (int i = 0; i < corps.Length; i++)
            {
                string[] colomns = corps[i].Split(new char[] { ',' });
                Code code = new Code();

                try
                {
                    code.code = colomns[0].Substring(0, 4);
                    code.amount = Convert.ToInt64(colomns[9]);

                    codes.Add(code);
                }
                catch
                {
                    Console.Write("");
                }
            }

            codes.Sort();
            codes = codes.GetRange(0, 200);

            List<Stock> stocks = new List<Stock>();

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].amount < 1000000000)
                {
                    throw new Exception();
                }

                string url = "http://k-db.com/stocks/" + codes[i].code + "-T/a?download=csv";

                wc = new System.Net.WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;
                source = wc.DownloadString(url);
                wc.Dispose();

                string[] lines = source.Split(new char[] { '\n' });

                List<string> lines_list = new List<string>(lines);
                lines_list.Reverse();
                lines_list.RemoveAt(0);
                lines = lines_list.ToArray();

                Stock stock = new Stock();
                stock.code = codes[i].code;
                stock.records = new List<Record>();

                for (int j = 0; j < csv_line_count; j++)
                {
                    string[] colomns = lines[j].Split(new char[] { ',' });

                    if (colomns.Length == 8)
                    {
                    }
                    else
                    {
                        stock.code = null;
                        break;
                    }

                    Record record = new Record();

                    try
                    {
                        record.price1 = Convert.ToDouble(colomns[2]);
                        record.price2 = Convert.ToDouble(colomns[3]);
                        record.price3 = Convert.ToDouble(colomns[4]);
                        record.price4 = Convert.ToDouble(colomns[5]);
                        record.volume = Convert.ToInt32(colomns[6]);

                        record.isAM = j % 2 == 0;

                        stock.records.Add(record);
                    }
                    catch (FormatException)
                    {
                        Console.Write("");
                    }
                }

                if (stock.code == null)
                {
                }
                else
                {
                    stocks.Add(stock);
                }
            }

            List<Stock> ok = new List<Stock>();

            for (int i = 0; i < stocks.Count; i++)
            {
                if (stocks[i].records.Count < 500)
                {
                    Console.Write("");
                }

                stocks[i].start_price = stocks[i].records[0].price1;
                stocks[i].last_price = stocks[i].records[stocks[i].records.Count - 1].price4;
                //stocks[i].average_price = CalcAveragePrice(stocks[i].records.ToArray());

                stocks[i].trend = GetTrendRate(ConvertToDoubleArray(stocks[i].records));

                stocks[i].start_volume = stocks[i].records[0].volume;
                stocks[i].last_volume = stocks[i].records[stocks[i].records.Count - 1].volume;
                //stocks[i].average_volume = CalcAverageVolume(stocks[i].records.ToArray());

                //Console.WriteLine(stocks[i].code + " : " + stocks[i].start_price + " : " + stocks[i].last_price + " : " + stocks[i].average_price);
                //Console.WriteLine(stocks[i].code + " : " + stocks[i].start_volume + " : " + stocks[i].last_volume + " : " + stocks[i].average_volume);

                if (stocks[i].trend > 0)
                {
                    if (stocks[i].start_price < stocks[i].average_price && stocks[i].average_price < stocks[i].last_price)
                    {
                        if (stocks[i].start_volume < stocks[i].average_volume && stocks[i].average_price < stocks[i].last_price)
                        {
                            if (stocks[i].score < 1.0)
                            {
                                ok.Add(stocks[i]);
                                //Console.WriteLine(stocks[i].code + " : " + stocks[i].score);
                            }
                        }
                    }
                }
            }

            ok.Sort();
            for (int i = 0; i < ok.Count; i++)
            {
                Console.WriteLine(ok[i].code + " : " + ok[i].score);
            }

            Console.Read();
        }

        private static double[] ConvertToDoubleArray(List<Record> list)
        {
            double[] prices = new double[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                prices[i] = list[i].price;
            }

            return prices;
        }

        static int[] stocks = { 1928, 1925, 1911, 1924, 1722, 1868 };

        private static void ProcessStock3()
        {
            for (int i = 0; i < stocks.Length; i++)
            {
                ProcessOneStock(stocks[i]);
            }
        }

        private static void ProcessOneStock(int stock)
        {
            string url = "http://k-db.com/stocks/" + stock + "-T/a?download=csv";

            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            string source = wc.DownloadString(url);
            wc.Dispose();

            string[] lines = source.Split(new char[] { '\n' });
            double[] prices = new double[csv_line_count];
            double[] volumes = new double[csv_line_count];

            List<string> lines_list = new List<string>(lines);

            lines_list.Reverse();
            lines_list.RemoveAt(0);
            
            lines = lines_list.ToArray();

            for (int i = 0; i < csv_line_count; i++)
            {
                string[] colomns = lines[i].Split(new char[] { ',' });

                try
                {
                    prices[i] = (Convert.ToDouble(colomns[2]) + Convert.ToDouble(colomns[3]) + Convert.ToDouble(colomns[4]) + Convert.ToDouble(colomns[5])) / 4;
                    volumes[i] = Convert.ToDouble(colomns[6]);
                }
                catch (FormatException)
                {
                    prices[i] = prices[i - 1];
                    volumes[i] = volumes[i - 1];
                }
            }

            Console.WriteLine("stock = " + stock);

            double price_trend_rate = GetTrendRate(prices);
            Console.WriteLine("price_trend_rate = " + price_trend_rate);
            
            double volume_trend_rate = GetTrendRate(volumes);
            Console.WriteLine("volume_trend_rate = " + volume_trend_rate);

            Console.WriteLine("");
        }

        private static double GetTrendRate(double[] values)
        {
            double a = 0, b = 0;
            double sum_xy = 0, sum_x = 0, sum_y = 0, sum_x2 = 0;

            double[] x = new double[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                x[i] = i + 1;
                sum_xy += x[i] * values[i];
                sum_x += x[i];
                sum_y += values[i];
                sum_x2 += Math.Pow(x[i], 2);
            }

            a = (values.Length * sum_xy - sum_x * sum_y) / (values.Length * sum_x2 - Math.Pow(sum_x, 2));
            b = (sum_x2 * sum_y - sum_xy * sum_x) / (values.Length * sum_x2 - Math.Pow(sum_x, 2));

            return a;
        }
    }
}
