using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader a = new StreamReader("a.txt");
            StreamReader b = new StreamReader("b.txt");
            StreamReader c = new StreamReader("c.txt");
            StreamReader d = new StreamReader("d.txt");

            List<string> a_code_list = ReadFile(a);
            List<string> b_code_list = ReadFile(b);
            List<string> c_code_list = ReadFile(c);
            List<string> d_code_list = ReadFile(d);

            StreamWriter e = new StreamWriter("e.txt");

            MatchCode(e, a_code_list, b_code_list, c_code_list, d_code_list);
            MatchCode(e, b_code_list, a_code_list, c_code_list, d_code_list);
            MatchCode(e, c_code_list, a_code_list, b_code_list, d_code_list);
            MatchCode(e, d_code_list, a_code_list, b_code_list, c_code_list);
        }

        static void MatchCode(StreamWriter e, params List<string>[] code_lists)
        {
            foreach (string item in code_lists[0])
            {
                int count = 1;

                if (code_lists[1].Exists(pre => pre == item) == true)
                {
                    count++;
                }
                if (code_lists[2].Exists(pre => pre == item) == true)
                {
                    count++;
                }
                if (code_lists[3].Exists(pre => pre == item) == true)
                {
                    count++;
                }

                if (count == 4)
                {
                    throw new ApplicationException(item);
                }

                e.WriteLine(item + " : " + count);
                e.Flush();
            }
        }

        static List<string> ReadFile(StreamReader sr)
        {
            List<string> ret = new List<string>();

            while (true)
            {
                string line = sr.ReadLine();

                if (line == null)
                {
                    break;
                }

                string code = line.Substring(0, 4);
                int check;

                if (int.TryParse(code, out check) == false)
                {
                    continue;
                }

                ret.Add(code);
            }

            return ret;
        }
    }
}
