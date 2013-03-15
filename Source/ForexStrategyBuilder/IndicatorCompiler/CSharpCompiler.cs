//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     CSharp_Compiler manages the compilation of source code to an assembly.
    ///     This class is thread safe, so multiple threads are capable
    ///     to utilize it simultaneously.
    /// </summary>
    public class CSharpCompiler
    {
        private readonly CompilerParameters compilationParameters;
        private volatile CSharpCodeProvider codeProvider;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public CSharpCompiler()
        {
            codeProvider = new CSharpCodeProvider();
            // Make sure we conduct all the operations "in memory".
            compilationParameters = new CompilerParameters {GenerateInMemory = true};
        }

        /// <summary>
        ///     For the source code to compile, it needs to have a reference to assemblies
        ///     to use the IL code inside them.
        /// </summary>
        /// <param name="assembly">An assembly to add.</param>
        public void AddReferencedAssembly(Assembly assembly)
        {
            lock (this)
            {
                compilationParameters.ReferencedAssemblies.Add(assembly.Location);
            }
        }

        /// <summary>
        ///     Compile a single source file to assembly.
        /// </summary>
        /// <param name="source">Indicator source to compile.</param>
        /// <param name="compilerErrors">Compiler errors, if any.</param>
        /// <returns>Compiled assembly or null.</returns>
        public Assembly CompileSource(string source, out Dictionary<string, int> compilerErrors)
        {
            compilerErrors = new Dictionary<string, int>();

            CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilationParameters, source);

            if (compilerResults.Errors.Count > 0)
            {
                // Compilation failed.
                foreach (CompilerError error in compilerResults.Errors)
                {
                    string errorMessage = "Line " + error.Line + " Column " + error.Column + ": " + error.ErrorText +
                                          ".";
                    int errorLine = error.Line;

                    if (!compilerErrors.ContainsKey(errorMessage))
                        compilerErrors.Add(errorMessage, errorLine);
                }

                return null;
            }

            return compilerResults.CompiledAssembly;
        }
    }
}