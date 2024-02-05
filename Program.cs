using System;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace csLoader
{
    class Program
    {
        static String state;
        static String city;
        static String aptid;
        static String navaidName;
        static String pdf;

        static void Main(String[] args)
        {
            String userprofileFolder = Environment.GetEnvironmentVariable("USERPROFILE");
            String[] fileEntries = Directory.GetFiles(userprofileFolder + "\\Downloads\\", "DCS_*.zip");

            ZipArchive archive = ZipFile.OpenRead(fileEntries[0]);

            String[] fileNames = fileEntries[0].Split('\\');

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name.Length > 0)
                {
                    String s = entry.Name.Substring(entry.Name.Length - 3, 3);

                    if (String.Compare(s, "xml") == 0)
                    {
                        entry.ExtractToFile("cs.xml", true);
                        
                        break;
                    }
                }
            }

            StreamWriter sw = new StreamWriter("chartSupplement.txt");

            XmlTextReader textReader = new XmlTextReader("cs.xml");

            while (textReader.Read())
            {
                if (textReader.NodeType == XmlNodeType.Element)
                {
                    if (String.Compare(textReader.Name, "location") == 0)
                    {
                        state = textReader.GetAttribute("state");
                    }

                    if (String.Compare(textReader.Name, "aptcity") == 0)
                    {
                        city = textReader.ReadElementContentAsString();
                    }

                    if (String.Compare(textReader.Name, "aptid") == 0)
                    {
                        aptid = textReader.ReadElementContentAsString();
                    }

                    if (String.Compare(textReader.Name, "navidname") == 0)
                    {
                        // first dash is XML &#8211;
                        navaidName = textReader.ReadElementContentAsString().Replace("–", "-").Trim();
                        
                        navaidName = Regex.Replace(navaidName, @"[^\u0020-\u007E]+", string.Empty);
                    }

                    if (String.Compare(textReader.Name, "pdf") == 0)
                    {
                        pdf = textReader.ReadElementContentAsString();
                        String[] pdfa = pdf.Split('.');
                        pdf = pdfa[0].ToUpper() + '.' + pdfa[1];

                        sw.Write(aptid);
                        sw.Write("~");

                        sw.Write(city);
                        sw.Write("~");

                        sw.Write(state);
                        sw.Write("~");

                        sw.Write(navaidName);
                        sw.Write("~");

                        sw.Write(pdf);
                        sw.Write("~");

                        sw.Write(fileNames[fileNames.Length - 1]);
                        sw.Write("\r\n");

                    }
                }
            }

            textReader.Close();
            
            sw.Close();
        }

    }
}
