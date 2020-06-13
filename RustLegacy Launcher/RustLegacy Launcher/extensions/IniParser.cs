using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace extensions
{
    internal sealed class IniParser
    {
        private readonly string iniFilePath;

        private readonly Hashtable keyPairs = new Hashtable();

        internal readonly string Name;

        private readonly List<IniParser.SectionPair> tmpList = new List<IniParser.SectionPair>();

        internal string[] Sections
        {
            get
            {
                List<string> strs = new List<string>();
                foreach (IniParser.SectionPair sectionPair in this.tmpList)
                {
                    if (strs.Contains(sectionPair.Section))
                    {
                        continue;
                    }
                    strs.Add(sectionPair.Section);
                }
                return strs.ToArray();
            }
        }

        internal IniParser(string iniPath)
        {
            IniParser.SectionPair sectionPair = new IniParser.SectionPair();
            string str = null;
            this.iniFilePath = iniPath;
            this.Name = Path.GetFileNameWithoutExtension(iniPath);
            if (!File.Exists(iniPath))
            {
                throw new FileNotFoundException(string.Concat("Unable to locate ", iniPath));
            }
            try
            {
                using (TextReader streamReader = new StreamReader(iniPath))
                {
                    for (string i = streamReader.ReadLine(); i != null; i = streamReader.ReadLine())
                    {
                        i = i.Trim();
                        if (i != "")
                        {
                            if (!i.StartsWith("[") || !i.EndsWith("]"))
                            {
                                if (i.StartsWith(";"))
                                {
                                    i = string.Concat(i.Replace("=", "%eq%"), "=%comment%");
                                }
                                string[] strArrays = i.Split(new char[] { '=' }, 2);
                                string str1 = null;
                                if (str == null)
                                {
                                    str = "ROOT";
                                }
                                sectionPair.Section = str;
                                sectionPair.Key = strArrays[0];
                                if ((int)strArrays.Length > 1)
                                {
                                    str1 = strArrays[1];
                                }
                                try
                                {
                                    this.keyPairs.Add(sectionPair, str1);
                                    this.tmpList.Add(sectionPair);
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                str = i.Substring(1, i.Length - 2);
                            }
                        }
                    }
                    streamReader.Close();
                }
            }
            catch
            {
            }
        }

        internal void AddSetting(string sectionName, string settingName)
        {
            this.AddSetting(sectionName, settingName, string.Empty);
        }

        internal void AddSetting(string sectionName, string settingName, string settingValue)
        {
            IniParser.SectionPair sectionPair = new IniParser.SectionPair();
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;
            if (settingValue == null)
            {
                settingValue = string.Empty;
            }
            if (this.keyPairs.ContainsKey(sectionPair))
            {
                this.keyPairs.Remove(sectionPair);
            }
            if (this.tmpList.Contains(sectionPair))
            {
                this.tmpList.Remove(sectionPair);
            }
            this.keyPairs.Add(sectionPair, settingValue);
            this.tmpList.Add(sectionPair);
        }

        internal bool ContainsSetting(string sectionName, string settingName)
        {
            IniParser.SectionPair sectionPair = new IniParser.SectionPair();
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;
            return this.keyPairs.Contains(sectionPair);
        }

        internal bool ContainsValue(string valueName)
        {
            return this.keyPairs.ContainsValue(valueName);
        }

        internal int Count()
        {
            return (int)this.Sections.Length;
        }

        internal void DeleteSetting(string sectionName, string settingName)
        {
            IniParser.SectionPair sectionPair = new IniParser.SectionPair();
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;
            if (this.keyPairs.ContainsKey(sectionPair))
            {
                this.keyPairs.Remove(sectionPair);
                this.tmpList.Remove(sectionPair);
            }
        }

        internal string[] EnumSection(string sectionName)
        {
            List<string> strs = new List<string>();
            foreach (IniParser.SectionPair sectionPair in this.tmpList)
            {
                if (sectionPair.Key.StartsWith(";") || !(sectionPair.Section == sectionName))
                {
                    continue;
                }
                strs.Add(sectionPair.Key);
            }
            return strs.ToArray();
        }

        internal bool GetBoolSetting(string sectionName, string settingName)
        {
            bool flag;
            bool.TryParse(this.GetSetting(sectionName, settingName), out flag);
            return flag;
        }

        internal string GetSetting(string sectionName, string settingName)
        {
            IniParser.SectionPair sectionPair = new IniParser.SectionPair();
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;
            return (string)this.keyPairs[sectionPair];
        }

        internal bool isCommandOn(string cmdName)
        {
            return this.GetBoolSetting("Commands", cmdName);
        }

        internal void Save()
        {
            this.SaveSettings(this.iniFilePath);
        }

        internal void SaveSettings(string newFilePath)
        {
            ArrayList arrayLists = new ArrayList();
            string item = "";
            string str = "";
            foreach (IniParser.SectionPair sectionPair in this.tmpList)
            {
                if (arrayLists.Contains(sectionPair.Section))
                {
                    continue;
                }
                arrayLists.Add(sectionPair.Section);
            }
            foreach (string arrayList in arrayLists)
            {
                str = string.Concat(str, "[", arrayList, "]\r\n");
                foreach (IniParser.SectionPair sectionPair1 in this.tmpList)
                {
                    if (sectionPair1.Section != arrayList)
                    {
                        continue;
                    }
                    item = (string)this.keyPairs[sectionPair1];
                    if (item != null)
                    {
                        item = (item != "%comment%" ? string.Concat("=", item) : "");
                    }
                    str = string.Concat(str, sectionPair1.Key.Replace("%eq%", "="), item, "\r\n");
                }
                str = string.Concat(str, "\r\n");
            }
            using (TextWriter streamWriter = new StreamWriter(newFilePath))
            {
                streamWriter.Write(str);
            }
        }

        internal void SetSetting(string sectionName, string settingName, string value)
        {
            IniParser.SectionPair sectionPair = new IniParser.SectionPair();
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }
            if (this.keyPairs.ContainsKey(sectionPair))
            {
                this.keyPairs[sectionPair] = value;
            }
        }

        private struct SectionPair
        {
            public string Section;

            public string Key;
        }
    }
}
