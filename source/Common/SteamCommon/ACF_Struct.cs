﻿
using PluginsCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SteamCommon
{
    //From https://stackoverflow.com/a/42876399
    class AcfReader
    {
        public string FileLocation { get; set; }

        public AcfReader(string FileLocation)
        {
            if (FileSystem.FileExists(FileLocation))
            {
                this.FileLocation = FileLocation;
            }
            else
            {
                throw new FileNotFoundException("Error", FileLocation);
            }
        }

        public AcfReader()
        {

        }

        public bool CheckIntegrity()
        {
            string Content = File.ReadAllText(FileLocation);
            int quote = Content.Count(x => x == '"');
            int braceleft = Content.Count(x => x == '{');
            int braceright = Content.Count(x => x == '}');

            return ((braceleft == braceright) && (quote % 2 == 0));
        }

        public ACF_Struct ACFFileToStruct()
        {
            return ACFStringToStruct(File.ReadAllText(FileLocation));
        }

        public ACF_Struct ACFStringToStruct(string RegionToReadIn)
        {
            ACF_Struct ACF = new ACF_Struct();
            int LengthOfRegion = RegionToReadIn.Length;
            int CurrentPos = 0;
            while (LengthOfRegion > CurrentPos)
            {
                int FirstItemStart = RegionToReadIn.IndexOf('"', CurrentPos);
                if (FirstItemStart == -1)
                {
                    break;
                }
                int FirstItemEnd = RegionToReadIn.IndexOf('"', FirstItemStart + 1);
                CurrentPos = FirstItemEnd + 1;
                string FirstItem = RegionToReadIn.Substring(FirstItemStart + 1, FirstItemEnd - FirstItemStart - 1);

                int SecondItemStartQuote = RegionToReadIn.IndexOf('"', CurrentPos);
                int SecondItemStartBraceleft = RegionToReadIn.IndexOf('{', CurrentPos);
                if (SecondItemStartQuote != -1 && (SecondItemStartBraceleft == -1 || SecondItemStartQuote < SecondItemStartBraceleft))
                {
                    int SecondItemEndQuote = RegionToReadIn.IndexOf('"', SecondItemStartQuote + 1);
                    string SecondItem = RegionToReadIn.Substring(SecondItemStartQuote + 1, SecondItemEndQuote - SecondItemStartQuote - 1);
                    CurrentPos = SecondItemEndQuote + 1;
                    ACF.SubItems.Add(FirstItem, SecondItem);
                }
                else
                {
                    int SecondItemEndBraceright = RegionToReadIn.NextEndOf('{', '}', SecondItemStartBraceleft + 1);
                    ACF_Struct ACFS = ACFStringToStruct(RegionToReadIn.Substring(SecondItemStartBraceleft + 1, SecondItemEndBraceright - SecondItemStartBraceleft - 1));
                    CurrentPos = SecondItemEndBraceright + 1;
                    ACF.SubACF.Add(FirstItem, ACFS);
                }
            }

            return ACF;
        }

    }

    class ACF_Struct
    {
        public Dictionary<string, ACF_Struct> SubACF { get; private set; }
        public Dictionary<string, string> SubItems { get; private set; }

        public ACF_Struct()
        {
            SubACF = new Dictionary<string, ACF_Struct>();
            SubItems = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return ToString(0);
        }

        private string ToString(int Depth)
        {
            StringBuilder SB = new StringBuilder();
            foreach (KeyValuePair<string, string> item in SubItems)
            {
                SB.Append('\t', Depth);
                SB.AppendFormat("\"{0}\"\t\t\"{1}\"\r\n", item.Key, item.Value);
            }
            foreach (KeyValuePair<string, ACF_Struct> item in SubACF)
            {
                SB.Append('\t', Depth);
                SB.AppendFormat("\"{0}\"\n", item.Key);
                SB.Append('\t', Depth);
                SB.AppendLine("{");
                SB.Append(item.Value.ToString(Depth + 1));
                SB.Append('\t', Depth);
                SB.AppendLine("}");
            }
            return SB.ToString();
        }
    }

    static class Extension
    {
        public static int NextEndOf(this string str, char Open, char Close, int startIndex)
        {
            if (Open == Close)
            {
                throw new Exception("\"Open\" and \"Close\" char are equivalent!");
            }
            int OpenItem = 0;
            int CloseItem = 0;
            for (int i = startIndex; i < str.Length; i++)
            {
                if (str[i] == Open)
                {
                    OpenItem++;
                }
                if (str[i] == Close)
                {
                    CloseItem++;
                    if (CloseItem > OpenItem)
                    {
                        return i;
                    }
                }
            }
            throw new Exception("Not enough closing characters!");
        }
    }
}
