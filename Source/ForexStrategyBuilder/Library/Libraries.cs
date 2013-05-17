//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.IO;
using System.Xml.Serialization;

namespace ForexStrategyBuilder.Library
{
    public class Libraries
    {
        private LibrariesSettings settings;

        public bool IfDllRelevant(string sourcePath)
        {
            string name = Path.GetFileNameWithoutExtension(sourcePath);
            LibRecord record = ReadRecord(name);

            if (record == null)
                return false;

            var sourceInfo = new FileInfo(record.SorcePath);

            if (!File.Exists(record.SorcePath))
                return false;

            if (!File.Exists(record.DllPath))
                return false;

            if (record.SurceModificationTime != sourceInfo.LastWriteTime)
                return false;

            return true;
        }

        public void AddRecord(LibRecord record)
        {
            if (settings.Records.ContainsKey(record.Name))
                settings.Records[record.Name] = record;
            else
                settings.Records.Add(record.Name, record);
        }

        private LibRecord ReadRecord(string name)
        {
            return settings.Records.ContainsKey(name) ? settings.Records[name] : null;
        }

        public void LoadSettings(string path)
        {
            settings = new LibrariesSettings();

            if (!File.Exists(path))
                return;

            var xmlSerializer = new XmlSerializer(typeof (LibrariesSettings));
            using (var reader = new StreamReader(path))
            {
                try
                {
                    settings = xmlSerializer.Deserialize(reader) as LibrariesSettings;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void SaveSettings(string path)
        {
            var xmlSerializer = new XmlSerializer(typeof (LibrariesSettings));
            using (var writer = new StreamWriter(path))
            {
                xmlSerializer.Serialize(writer, settings);
            }
        }

        public string GetDllPath(string filePath)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);
            LibRecord record = ReadRecord(name);

            return record == null ? string.Empty : record.DllPath;
        }
    }
}