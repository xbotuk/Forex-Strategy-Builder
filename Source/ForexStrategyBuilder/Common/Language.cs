// Language class
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Forex_Strategy_Builder.Properties;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Manages the language translations of the program
    /// </summary>
    public static class Language
    {
        private static Dictionary<String, String> _languageFiles; // Language files <Language><FileName>

        static Language()
        {
            MissingPhrases = new List<string>();
        }

        /// <summary>
        /// Gets the language dictionary.
        /// </summary>
        public static Dictionary<string, string> Translation { get; private set; }

        /// <summary>
        /// Gets the language files list.
        /// </summary>
        public static string[] LanguageList { get; private set; }

        /// <summary>
        /// Gets language file name.
        /// </summary>
        public static string LanguageFileName
        {
            get { return Path.GetFileName(_languageFiles[Configs.Language]); }
        }

        /// <summary>
        /// Gets the author name.
        /// </summary>
        public static string Author { get; private set; }

        /// <summary>
        /// Gets the author's website.
        /// </summary>
        public static string AuthorsWebsite { get; private set; }

        /// <summary>
        /// Gets the author's email.
        /// </summary>
        public static string AuthorsEmail { get; private set; }

        /// <summary>
        /// Gets the list of missing phrases.
        /// </summary>
        public static List<string> MissingPhrases { get; private set; }

        /// <summary>
        /// Language Translation.
        /// </summary>
        public static string T(string phrase)
        {
            if (Configs.Language == "English" || string.IsNullOrEmpty(phrase))
                return phrase;

            string translated = phrase;

            if (Translation.ContainsKey(phrase))
                translated = Translation[phrase];
            else if (!MissingPhrases.Contains(phrase))
                MissingPhrases.Add(phrase);

            return translated;
        }

        /// <summary>
        /// Initializes the languages.
        /// </summary>
        public static void InitLanguages()
        {
            _languageFiles = new Dictionary<string, string>();
            bool bIsLanguageSet = false;

            if (Directory.Exists(Data.LanguageDir) && Directory.GetFiles(Data.LanguageDir).Length > 0)
            {
                string[] asLangFiles = Directory.GetFiles(Data.LanguageDir);

                foreach (string langFile in asLangFiles)
                {
                    if (!langFile.EndsWith(".xml", true, null)) continue;
                    try
                    {
                        var xmlLanguage = new XmlDocument();
                        xmlLanguage.Load(langFile);
                        XmlNode node = xmlLanguage.SelectSingleNode("lang//language");

                        if (node == null)
                        {
                            // There is no language specified it the lang file
                            string sMessageText = "Language file: " + langFile + Environment.NewLine +
                                                  Environment.NewLine +
                                                  "The language is not specified!";
                            MessageBox.Show(sMessageText, "Language Files Loading", MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                        }
                        else if (_languageFiles.ContainsKey(node.InnerText))
                        {
                            // This language has been already loaded
                            string sMessageText = "Language file: " + langFile + Environment.NewLine +
                                                  Environment.NewLine +
                                                  "Duplicated language!";
                            MessageBox.Show(sMessageText, "Language Files Loading", MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            // It looks OK
                            string sLanguage = node.InnerText;
                            _languageFiles.Add(sLanguage, langFile);

                            if (sLanguage == Configs.Language)
                            {
                                LoadLanguageFile(langFile);
                                bIsLanguageSet = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        string sMessageText = "Language file: " + langFile + Environment.NewLine + Environment.NewLine +
                                              "Error in the language file!" + Environment.NewLine + Environment.NewLine +
                                              e.Message;
                        MessageBox.Show(sMessageText, "Language Files Loading", MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                    }
                }
            }

            if (!_languageFiles.ContainsKey("English"))
                _languageFiles.Add("English", "System");

            if (!_languageFiles.ContainsKey("Български"))
                _languageFiles.Add("Български", "System");

            if (!bIsLanguageSet)
            {
                if (Configs.Language == "Български")
                    LoadLanguageFile("Български");
                else
                {
                    LoadLanguageFile("English");
                    Configs.Language = "English";
                }
            }

            CheckLangFile();

            LanguageList = new string[_languageFiles.Count];
            _languageFiles.Keys.CopyTo(LanguageList, 0);
            Array.Sort(LanguageList);
        }

        /// <summary>
        /// Loads a language dictionary.
        /// </summary>
        private static void LoadLanguageFile(string sLangFile)
        {
            var xmlLanguage = new XmlDocument();

            if (sLangFile == "Български" || sLangFile == "English")
                xmlLanguage.InnerXml = Resources.Bulgarian;
            else
                xmlLanguage.Load(sLangFile);

            Author = xmlLanguage.SelectSingleNode("lang//translatedby").InnerText;
            AuthorsWebsite = xmlLanguage.SelectSingleNode("lang//website").InnerText;
            AuthorsEmail = xmlLanguage.SelectSingleNode("lang//corrections").InnerText;

            XmlNodeList xmlStringList = xmlLanguage.GetElementsByTagName("str");

            int iStrings = xmlStringList.Count;
            Translation = new Dictionary<string, string>(iStrings);

            foreach (XmlNode nodeString in xmlStringList)
            {
                string sMain = nodeString.SelectSingleNode("main").InnerText;
                string sAlt = nodeString.SelectSingleNode("alt").InnerText;

                if (Data.Debug && Translation.ContainsValue(sAlt))
                {
                    string sMessage = "The string" + ": " + sAlt + Environment.NewLine +
                                      "appears more than once in the language file";
                    MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (Translation.ContainsKey(sMain))
                {
                    string sMessage = "The string" + ": " + sMain + Environment.NewLine +
                                      "appears more than once in the language file";
                    MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    Translation.Add(sMain, sAlt);
                }
            }
        }

        /// <summary>
        /// Generates English.xml and Bulgarian.xml.
        /// </summary>
        public static void GenerateLangFiles()
        {
            // Generate Bulgarian.xml
            string sFilePath = Path.Combine(Data.LanguageDir, "Bulgarian.xml");
            string sContent = Resources.Bulgarian;
            SaveTextFile(sFilePath, sContent);

            // Generate English.xml
            const string patternMain = @"<main>(?<main>.*)</main>";
            const string patternAlt = @"<alt>(?<alt>.*)</alt>";
            var expression = new Regex(".*" + patternMain + patternAlt + ".*", RegexOptions.Compiled);
            sFilePath = Path.Combine(Data.LanguageDir, "English.xml");
            var sb = new StringBuilder();
            foreach (string line in sContent.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                Match match = expression.Match(line);
                if (match.Success)
                {
                    string main = match.Groups["main"].Value;
                    string alt = match.Groups["alt"].Value;
                    sb.AppendLine(line.Replace(alt, main));
                }
                else
                    sb.AppendLine(line);
            }
            sContent = sb.ToString();
            sContent = sContent.Replace("Български", "English");
            SaveTextFile(sFilePath, sContent);
        }

        /// <summary>
        /// Generates a new language file.
        /// </summary>
        public static bool GenerateNewLangFile(string sFileName, string sLang, string sAuthor, string sWebsite,
                                               string sEmail)
        {
            string sContent = Resources.Bulgarian;
            const string patternMain = @"<main>(?<main>.*)</main>";
            const string patternAlt = @"<alt>(?<alt>.*)</alt>";
            var expression = new Regex(".*" + patternMain + patternAlt + ".*", RegexOptions.Compiled);
            string filePath = Path.Combine(Data.LanguageDir, sFileName);
            var sb = new StringBuilder();
            foreach (string sLine in sContent.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                Match match = expression.Match(sLine);
                if (match.Success)
                {
                    string main = match.Groups["main"].Value;
                    string alt = match.Groups["alt"].Value;
                    sb.AppendLine(sLine.Replace(alt, main));
                }
                else
                    sb.AppendLine(sLine);
            }
            sContent = sb.ToString();
            sContent = sContent.Replace("Български", sLang);
            sContent = sContent.Replace("Forex Software Ltd.", sAuthor);
            sContent = sContent.Replace(@"http://forexsb.com", sWebsite);
            sContent = sContent.Replace(@"info@forexsb.com", sEmail);

            return SaveTextFile(filePath, sContent);
        }

        /// <summary>
        /// Generates a new language file.
        /// </summary>
        public static void SaveLangFile(Dictionary<string, string> dict, string sAuthor, string sWebsite, string sEmail)
        {
            string path = _languageFiles[Configs.Language];
            var xmlLanguage = new XmlDocument();
            xmlLanguage.Load(path);

            xmlLanguage.SelectSingleNode("lang//translatedby").InnerText = sAuthor;
            xmlLanguage.SelectSingleNode("lang//website").InnerText = sWebsite;
            xmlLanguage.SelectSingleNode("lang//corrections").InnerText = sEmail;

            XmlNodeList xmlStringList = xmlLanguage.GetElementsByTagName("str");

            foreach (XmlNode nodeString in xmlStringList)
            {
                nodeString.SelectSingleNode("alt").InnerText = dict[nodeString.SelectSingleNode("main").InnerText];
            }

            try
            {
                xmlLanguage.Save(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Repairs all the language files.
        /// </summary>
        public static string RapairAllLangFiles()
        {
            string[] langFiles = Directory.GetFiles(Data.LanguageDir);
            string report = "";

            foreach (string langFile in langFiles)
            {
                if (!langFile.EndsWith(".xml", true, null)) continue;
                var xmlBaseLanguage = new XmlDocument {InnerXml = Resources.Bulgarian};

                var xmlLanguage = new XmlDocument();
                xmlLanguage.Load(langFile);

                try
                {
                    xmlBaseLanguage.SelectSingleNode("lang//language").InnerText =
                        xmlLanguage.SelectSingleNode("lang//language").InnerText;
                    xmlBaseLanguage.SelectSingleNode("lang//translatedby").InnerText =
                        xmlLanguage.SelectSingleNode("lang//translatedby").InnerText;
                    xmlBaseLanguage.SelectSingleNode("lang//website").InnerText =
                        xmlLanguage.SelectSingleNode("lang//website").InnerText;
                    xmlBaseLanguage.SelectSingleNode("lang//corrections").InnerText =
                        xmlLanguage.SelectSingleNode("lang//corrections").InnerText;

                    XmlNodeList xmlBaseStringList = xmlBaseLanguage.GetElementsByTagName("str");
                    XmlNodeList xmlStringList = xmlLanguage.GetElementsByTagName("str");
                    foreach (XmlNode nodeBaseString in xmlBaseStringList)
                    {
                        string main = nodeBaseString.SelectSingleNode("main").InnerText;
                        nodeBaseString.SelectSingleNode("alt").InnerText = main;

                        foreach (XmlNode nodeString in xmlStringList)
                            if (nodeString.SelectSingleNode("main").InnerText == main)
                                nodeBaseString.SelectSingleNode("alt").InnerText =
                                    nodeString.SelectSingleNode("alt").InnerText;
                    }

                    report += xmlLanguage.SelectSingleNode("lang//language").InnerText + " - OK" + Environment.NewLine;
                }
                catch (Exception e)
                {
                    string message = "Language file: " + langFile + Environment.NewLine + Environment.NewLine +
                                     "Error in the language file!" + Environment.NewLine + Environment.NewLine +
                                     e.Message;
                    MessageBox.Show(message, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                xmlBaseLanguage.Save(langFile);
            }

            return report;
        }

        /// <summary>
        /// Saves a text file
        /// </summary>
        private static bool SaveTextFile(string sFilePath, string sContent)
        {
            bool success = false;

            try
            {
                // Pass the file path and filename to the StreamWriter Constructor
                var sw = new StreamWriter(sFilePath);

                // Write the text
                sw.Write(sContent);

                // Close the file
                sw.Close();

                success = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return success;
        }

        /// <summary>
        /// Checks the language file.
        /// </summary>
        private static void CheckLangFile()
        {
            var xmlLanguage = new XmlDocument {InnerXml = Resources.Bulgarian};
            XmlNodeList xmlStringList = xmlLanguage.GetElementsByTagName("str");

            int iStrings = xmlStringList.Count;
            var listPhrases = new List<string>(iStrings);

            foreach (XmlNode nodeString in xmlStringList)
            {
                XmlNode selectSingleNode = nodeString.SelectSingleNode("main");
                if (selectSingleNode == null) continue;
                string sMain = selectSingleNode.InnerText;

                if (listPhrases.Contains(sMain))
                {
                    string sMessage = "The string" + ": " + sMain + Environment.NewLine +
                                      "appears more than once in the base language file";
                    MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    listPhrases.Add(sMain);
                }
            }

            string errors = "";

            foreach (var kvp in Translation)
                if (!listPhrases.Contains(kvp.Key))
                    errors += kvp.Key + Environment.NewLine;

            if (errors != "")
            {
                string sMessage = "Unused phrases:" + Environment.NewLine + Environment.NewLine + errors;
                MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            errors = "";

            foreach (string sPhrase in listPhrases)
                if (!Translation.ContainsKey(sPhrase))
                    errors += sPhrase + Environment.NewLine;

            if (errors != "")
            {
                string sMessage = "The language file does not contain the phrases:" + Environment.NewLine +
                                  Environment.NewLine + errors;
                MessageBox.Show(sMessage, "Language Files Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Shows the phrases in a web browser.
        /// </summary>
        /// <param name="whatToShow">1 - English, 2 - Alt, 3 - Both, 4 - Wiki</param>
        public static void ShowPhrases(int whatToShow)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">");
            sb.AppendLine("<head><meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\" />");
            sb.AppendLine("<title>" + Configs.Language + "</title>");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {padding: 0 10px 10px 10px; margin: 0px; font-family: Verdana, Helvetica, Arial, Sans-Serif; font-size: 62.5%; background-color: #fffffe; color: #000033}");
            sb.AppendLine(".content h1 {font-size: 1.9em; text-align: center;}");
            sb.AppendLine(".content h2 {font-size: 1.6em;}");
            sb.AppendLine(".content p {color: #000033; font-size: 1.3em; text-align: left}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"content\" id=\"header\">");

            sb.AppendLine("<h1>" + T("Language Phrases") + "</h1>");

            var asEnglishPhrases = new string[Translation.Count];
            var asAltPhrases = new string[Translation.Count];
            Translation.Keys.CopyTo(asEnglishPhrases, 0);
            Translation.Values.CopyTo(asAltPhrases, 0);

            string sTranslating = "<p>" +
                                  T("Translated by") + ": " + Author + "<br />" +
                                  T("Website") + ": <a href=\"" + AuthorsWebsite + "\" target=\"_blanc\">" +
                                  AuthorsWebsite + "</a>" + "<br />" +
                                  T("Contacts") + ": " + AuthorsEmail + "</p><hr />";

            if (whatToShow == 1)
            {
                sb.AppendLine("<h2>" + T("Useful for Automatic Translation") + "</h2>");
                sb.AppendLine(sTranslating);
                sb.AppendLine("<p>");
                foreach (string sPhrase in asEnglishPhrases)
                {
                    sb.AppendLine(sPhrase + "<br/>");
                }
                sb.AppendLine("</p>");
            }
            else if (whatToShow == 2)
            {
                sb.AppendLine("<h2>" + T("Useful for Spell Check") + "</h2>");
                sb.AppendLine(sTranslating);
                sb.AppendLine("<p>");
                foreach (string sPhrase in asAltPhrases)
                {
                    sb.AppendLine(sPhrase + "<br/>");
                }
                sb.AppendLine("</p>");
            }
            else if (whatToShow == 3)
            {
                sb.AppendLine("<h2>" + T("Useful for Translation Check") + "</h2>");
                sb.AppendLine(sTranslating);
                sb.AppendLine("<p>");
                foreach (string sPhrase in asEnglishPhrases)
                {
                    sb.AppendLine(sPhrase + " - " + Translation[sPhrase] + "<br/>");
                }
                sb.AppendLine("</p>");
            }
            else if (whatToShow == 4)
            {
                sb.AppendLine("<h2>" + T("Wiki Format") + "</h2>");
                sb.AppendLine(sTranslating);
                sb.AppendLine("<p>");
                sb.AppendLine("====== " + Configs.Language + " ======" + "<br/><br/>");
                sb.AppendLine("Please edit the right column only!<br/><br/>");
                sb.AppendLine("^ English ^" + Configs.Language + "^" + "<br/>");
                foreach (string sPhrase in asEnglishPhrases)
                {
                    sb.AppendLine("| " + sPhrase + " | " + Translation[sPhrase] + " |" + "<br/>");
                }
                sb.AppendLine("</p>");
            }

            // Footer
            sb.AppendLine("</div></body></html>");

            var browser = new Browser(T("Translation"), sb.ToString());
            browser.Show();
        }

        /// <summary>
        /// Imports a language file.
        /// </summary>
        public static void ImportLanguageFile(string sLangFile)
        {
            const string patternMain = @"<main>(?<main>.*)</main>";
            const string patternAlt = @"<alt>(?<alt>.*)</alt>";
            var expression = new Regex(".*" + patternMain + patternAlt + ".*", RegexOptions.Compiled);

            string content = Resources.Bulgarian;
            string sFilePath = Path.Combine(Data.LanguageDir, "Imported.xml");
            string[] asPhrases = sLangFile.Split(Environment.NewLine.ToCharArray()[0]);

            var sb = new StringBuilder();
            int phraseNumber = 0;
            foreach (string sLine in content.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                Match match = expression.Match(sLine);
                if (match.Success)
                {
                    if (phraseNumber >= asPhrases.Length)
                        break;

                    int index = match.Groups["alt"].Index;
                    int lenght = match.Groups["alt"].Length;

                    string translation = asPhrases[phraseNumber].Trim();
                    sb.AppendLine(sLine.Remove(index, lenght).Insert(index, translation));
                    phraseNumber++;
                }
                else
                    sb.AppendLine(sLine);
            }
            content = sb.ToString();
            content = content.Replace("Български", "Imported");
            SaveTextFile(sFilePath, content);
        }
    }
}