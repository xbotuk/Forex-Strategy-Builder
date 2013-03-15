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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Manages the operation of indicators.
    /// </summary>
    public class IndicatorCompilationManager
    {
        private readonly CSharpCompiler compiler;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public IndicatorCompilationManager()
        {
            CustomIndicatorsList = new List<Indicator>();
            compiler = new CSharpCompiler();

            foreach (Assembly assembly in GetReferencedAndInitialAssembly(Assembly.GetEntryAssembly()))
            {
                compiler.AddReferencedAssembly(assembly);
            }
        }

        /// <summary>
        ///     Gets a list of the loaded custom indicators
        /// </summary>
        public List<Indicator> CustomIndicatorsList { get; private set; }

        /// <summary>
        ///     Gather all assemblies referenced from current assembly.
        /// </summary>
        private static IEnumerable<Assembly> GetReferencedAndInitialAssembly(Assembly initialAssembly)
        {
            AssemblyName[] names = initialAssembly.GetReferencedAssemblies();
            var assemblies = new Assembly[names.Length + 1];

            for (int i = 0; i < names.Length; i++)
            {
                assemblies[i] = Assembly.Load(names[i]);
            }

            assemblies[assemblies.Length - 1] = initialAssembly;

            return assemblies;
        }

        /// <summary>
        ///     Load file, compile it and create/load the indicators into the CustomIndicatorsList.
        /// </summary>
        /// <param name="filePath">Path to the source file</param>
        /// <param name="errorMessages">Resulting error messages, if any.</param>
        public void LoadCompileSourceFile(string filePath, out string errorMessages)
        {
            string errorLoadSourceFile;
            string source = LoadSourceFile(filePath, out errorLoadSourceFile);

            if (string.IsNullOrEmpty(source))
            {
                // Source file loading failed.
                errorMessages = errorLoadSourceFile;
                return;
            }

            // Rename namespaces
            source = source.Replace("Forex_Strategy_Builder", "ForexStrategyBuilder");

            Dictionary<string, int> dictCompilationErrors;
            Assembly assembly = compiler.CompileSource(source, out dictCompilationErrors);

            if (assembly == null)
            {
                // Assembly compilation failed.
                var sbCompilationError = new StringBuilder();
                sbCompilationError.AppendLine("ERROR: Indicator compilation failed in file [" +
                                              Path.GetFileName(filePath) + "]");

                foreach (string error in dictCompilationErrors.Keys)
                {
                    sbCompilationError.AppendLine('\t' + error);
                }

                errorMessages = sbCompilationError.ToString();
                return;
            }

            string errorGetIndicator;
            string indicatorFileName = Path.GetFileNameWithoutExtension(filePath);
            Indicator newIndicator = GetIndicatorInstanceFromAssembly(assembly, indicatorFileName, out errorGetIndicator);

            if (newIndicator == null)
            {
                // Getting an indicator instance failed.
                errorMessages = errorGetIndicator;
                return;
            }

            // Check for a repeated indicator name among the custom indicators
            foreach (Indicator indicator in CustomIndicatorsList)
                if (indicator.IndicatorName == newIndicator.IndicatorName)
                {
                    errorMessages = "The name '" + newIndicator.IndicatorName + "' found out in [" +
                                    Path.GetFileName(filePath) + "] is already in use.";
                    return;
                }

            // Check for a repeated indicator name among the original indicators
            foreach (string indicatorName in IndicatorStore.OriginalIndicatorNames)
                if (indicatorName == newIndicator.IndicatorName)
                {
                    errorMessages = "The name '" + indicatorName + "' found out in [" + Path.GetFileName(filePath) +
                                    "] is already in use.";
                    return;
                }

            // Test the new custom indicator
            string errorTestIndicator;
            if (!IndicatorTester.CustomIndicatorFastTest(newIndicator, out errorTestIndicator))
            {
                // Testing the indicator failed.
                errorMessages = errorTestIndicator;
                return;
            }

            // Adds the custom indicator to the list
            CustomIndicatorsList.Add(newIndicator);

            errorMessages = string.Empty;
        }

        /// <summary>
        ///     Reads the source code from file contents.
        /// </summary>
        private string LoadSourceFile(string pathToIndicator, out string errorLoadSourceFile)
        {
            string result = string.Empty;

            if (!File.Exists(pathToIndicator))
            {
                errorLoadSourceFile = "ERROR The source file does not exist: " + Path.GetFileName(pathToIndicator);
                return result;
            }

            try
            {
                using (var sr = new StreamReader(pathToIndicator))
                {
                    result = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch
            {
                errorLoadSourceFile = "ERROR Cannot read the file: " + Path.GetFileName(pathToIndicator);
                return result;
            }

            errorLoadSourceFile = string.Empty;
            return result;
        }

        /// <summary>
        ///     Creates an indicator instance from the assembly given.
        /// </summary>
        private static Indicator GetIndicatorInstanceFromAssembly(Assembly assembly, string indicatorFileName,
                                                                  out string errorMessage)
        {
            Type[] assemblyTypes = assembly.GetTypes();
            foreach (Type typeAssembly in assemblyTypes)
            {
                if (typeAssembly.IsSubclassOf(typeof (Indicator)))
                {
                    ConstructorInfo[] aConstructorInfo = typeAssembly.GetConstructors();

                    // Looking for an appropriate constructor
                    foreach (ConstructorInfo constructorInfo in aConstructorInfo)
                    {
                        ParameterInfo[] parameterInfo = constructorInfo.GetParameters();
                        if (constructorInfo.IsConstructor &&
                            constructorInfo.IsPublic &&
                            parameterInfo.Length == 1 &&
                            parameterInfo[0].ParameterType == typeof (SlotTypes))
                        {
                            try
                            {
                                errorMessage = string.Empty;
                                return (Indicator) constructorInfo.Invoke(new object[] {SlotTypes.NotDefined});
                            }
                            catch (Exception exc)
                            {
                                errorMessage = "ERROR: [" + indicatorFileName + "] " + exc.Message;
                                if (!string.IsNullOrEmpty(exc.InnerException.Message))
                                    errorMessage += Environment.NewLine + "\t" + exc.InnerException.Message;
                                return null;
                            }
                        }
                    }

                    errorMessage = "ERROR: Cannot find an appropriate constructor for " + indicatorFileName + ".";
                    return null;
                }
            }

            errorMessage = "ERROR: Cannot create an instance of an indicator from " + assembly + ".";
            return null;
        }
    }
}