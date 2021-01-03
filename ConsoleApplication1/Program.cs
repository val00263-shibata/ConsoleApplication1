using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo di = new DirectoryInfo("a");
            FileInfo[] files = di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
            List<int> all = new List<int>();

            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.FullName);
                List<int> buf = ReadFile(new StreamReader(file.FullName));
                all.AddRange(buf);
            }

            DirectoryInfo di2 = new DirectoryInfo("b");
            FileInfo[] files2 = di2.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
            List<int> all2 = new List<int>();

            foreach (FileInfo file in files2)
            {
                Console.WriteLine(file.FullName);
                List<int> buf = ReadFile(new StreamReader(file.FullName));
                all2.AddRange(buf);
            }

            Console.WriteLine("");
            Console.WriteLine(all.Count);
            Console.WriteLine(all2.Count);
            Console.WriteLine("");

            foreach (int code in all)
            {
                if (all2.Exists(x => x == code))
                {
                    Console.WriteLine(code);
                }
            }
        }

        static List<int> ReadFile(StreamReader sr)
        {
            List<int> ret = new List<int>();

            while (true)
            {
                string line = sr.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.StartsWith("\t") == true)
                {
                    continue;
                }

                string code = line.Substring(0, 4);
                int check;

                if (int.TryParse(code, out check) == false)
                {
                    continue;
                }

                ret.Add(check);
            }

            return ret;
        }
    }
}
