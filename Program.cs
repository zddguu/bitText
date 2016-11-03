using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
namespace helenthing
{
    class Program
    {
        const int ngram = 4;
        const string referenceText = @"./ref/A pocketful of wry.txt";
       // const string[] comparisonText = @"A:\a1\compare\"; 
        const string comparisonTextDir = @"./compare/";
       public static string[] comparisonTexts;
        static void Main(string[] args)
        {

            bool compareFlag = true;
            
            string output = "";

            string input;
            stringWorker work = new stringWorker();
            //output =  work.parseString(input);

            fileWorker file = new fileWorker(referenceText);

           int numCompareTexts = file.setupComparisonTexts(comparisonTexts, comparisonTextDir);

           Dictionary<string, int>[] comparisonTextDic = new Dictionary<string, int>[numCompareTexts];


            input = file.text;
            output = work.parseString(input);
            //Clippy.PushAnsiStringToClipboard(output);
            //Console.WriteLine(output);

            Dictionary<string, int> a = work.analyzeyo(output, ngram);

            work.doComparisonTexts(comparisonTexts, comparisonTextDic, ngram);

            //  Console.Write(work.printdic(a, ngram));
            file.nomalizeDictionary(a);

            for (int i = 0; i < comparisonTexts.Length; i++ )
            {
                file.nomalizeDictionary(comparisonTextDic[i]);
            }

                file.saveXML(a);
            Console.WriteLine("DONE");
            Console.WriteLine("Press Enter to close");
            Console.In.ReadLine();
        }
    }

    class fileWorker
    {
        public string text;

        public fileWorker(string p)
        {

            this.text = File.ReadAllText(p);
        }


        public void nomalizeDictionary(Dictionary<string, int> dic)
        {
           int total = dic.Sum(p => p.Value);
            dic["Total"] = total;
        }

        public int setupComparisonTexts(string[] comparisonTexts, string comparisonTextDir)
        {
            comparisonTexts = Directory.GetFiles(comparisonTextDir, "*.txt").ToArray();
            return comparisonTexts.Length;
        }

        public void saveXML(Dictionary<string, int> dic)
        {
            FileStream DestinationStream = File.Create("textxml.xml");

            using (XmlWriter writer = XmlWriter.Create(DestinationStream))
            {
                Boolean normalized;
                try{ 
                  int a = dic["Total"];
                  normalized = true;
                }
                catch(System.Collections.Generic.KeyNotFoundException)
                {
                    normalized = false;
                }

                writer.WriteStartDocument();
                writer.WriteStartElement("N-gram");

                foreach (KeyValuePair<string, int> ngram in dic)
                {
                    writer.WriteStartElement("Element1");
                    writer.WriteElementString("Ngram", "'" + ngram.Key + "'");

                    if (!normalized) { writer.WriteElementString("value", ngram.Value.ToString()); }
                    else {
                        writer.WriteElementString("value", (((decimal)ngram.Value) / ((decimal)dic["Total"])).ToString() );
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

        }
    }


    class stringWorker
    {

        public void doComparisonTexts(string[] comparisonTexts, Dictionary<string, int>[] dics, int ngram)
        {
            for (int i = 0; i < comparisonTexts.Length; i++)
            {
                fileWorker a = new fileWorker(comparisonTexts[i]);
                string b = parseString(a.text);
                dics[i] = analyzeyo(b, ngram);
            }
        }

        public void compare(Dictionary<string, int> refdic, Dictionary<string, int> comparedic)
        {
//I only want to compare the reference paper to one other, here, due to memory constraints
//Also, maybe get rid of the "text" in the dictionaries, and replace with ints to save on further space?
//This'd maybe get us up to a 31 ngram?


            bool normalized = true;
try{
int a =  refdic["Total"];
int b = comparedic["Total"];
normalized = true;
}
catch(System.Collections.Generic.KeyNotFoundException)
{
    normalized = false;
}

if(normalized)
{
foreach( KeyValuePair<String,int> a in refdic)
{

}
}

comparedic["COMPARED"] = 1;

        }

        public string parseString7bit(string input)
        {
            // byte[] snack = System.Text.Encoding.GetEncoding(437).GetBytes(input.ToCharArray());
            byte[] snack = System.Text.ASCIIEncoding.ASCII.GetBytes(input.ToCharArray());
            string output = "";
            foreach (byte snack1 in snack)
            {
                //  output += Convert.ToString(snack1, 2);
                output += ToBin(snack1, 7);
            }
            return output;
        }

        public string parseString(string input)
        {
            // byte[] snack = System.Text.Encoding.GetEncoding(437).GetBytes(input.ToCharArray());
            byte[] snack = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(input.ToCharArray());
            StringBuilder sb = new StringBuilder();

            foreach (byte snack1 in snack)
            {
                
               sb.Append(ToBin(snack1, 8));
            }
            return sb.ToString();
        }
        public string ToBin(int value, int len)
        {
            return (len > 1 ? ToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }

        public Dictionary<string, int> analyzeyo(string binarystring, int ngram)
        {
            int capacity = (int)Math.Pow(2, ngram);

            Dictionary<string, int> nirgendheim = new Dictionary<string, int>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                nirgendheim.Add(ToBin(i, ngram), 0);
            }

            int endpos = binarystring.Length - ngram;
            for (int i = 0; i < endpos - ngram; i++)
            {
                string subby = binarystring.Substring(i, ngram);
                nirgendheim[subby] += 1;
            }

            return nirgendheim;
        }

        public string printdic(Dictionary<string, int> dic1, int ngram)
        {
            StringBuilder sb = new StringBuilder();
            int capacity = (int)Math.Pow(2, ngram);
            for (int i = 0; i < capacity; i++)
            {
                string bb = ToBin(i, ngram);
                sb.Append(bb);
                sb.Append(" :");
                sb.Append(dic1[bb]);
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
