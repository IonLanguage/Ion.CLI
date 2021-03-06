using System.Collections.Generic;
using System.IO;
using CommandLine;
using IonCLI.Tools;

namespace IonCLI.Core
{
    public class Options
    {
        [Value(0, MetaName = "operation", Required = false, HelpText = "The operation to perform.", Default = "build")]
        public string Operation { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Enable verbose mode, allowing verbose messages to be displayed.")]
        public bool Verbose { get; set; }

        [Option('e', "exclude", Required = false, HelpText = "Exclude certain directories from being processed.")]
        public IEnumerable<string> Exclude { get; set; }

        [Option('o', "output", Required = false, HelpText = "The output directory which the program will be emitted onto.", Default = "bin")]
        public string Output { get; set; }

        [Option('r', "root", Required = false, HelpText = "The root directory to start the scanning process from.")]
        public string Root { get; set; }

        [Option('b', "bitcode", Required = false, HelpText = "Print out the LLVM Bitcode code instead of LLVM IR.")]
        public bool Bitcode { get; set; }

        [Option('s', "silent", Required = false, HelpText = "Do not output any messages.")]
        public bool Silent { get; set; }

        [Option('i', "no-integrity", Required = false, HelpText = "Skip integrity check.")]
        public bool NoIntegrity { get; set; }

        [Option('d', "debug", Required = false, HelpText = "Use debugging mode.")]
        public bool DebugMode { get; set; }

        [Option('t', "tools-path", Required = false, HelpText = "Specify the tools directory path to use. Path is relative to the CLI's execution directory.", Default = ToolConstants.DefaultToolsPath)]
        public string ToolsPath { get; set; }

        [Option('k', "keep-emitted", Required = false, HelpText = "Do not cleanup emitted files after compilation.")]
        public bool KeepEmittedFiles { get; set; }

        [Option('x', "external-output", Required = false, HelpText = "Whether to display external executables' output. Verbose mode must also be active.")]
        public bool ExternalOutput { get; set; }

        [Option('c', "ignore-exit-code", Required = false, HelpText = "Whether to ignore the exit code of the program being run.")]
        public bool IgnoreExitCode { get; set; }

        [Option('p', "print-output-ir", Required = false, HelpText = "Print resulting IR output onto the console. Verbose and output mode must also be active.")]
        public bool PrintOutputIr { get; set; }

        public PathResolver PathResolver { get; protected set; }

        public Options()
        {
            // Initialize the path resolver.
            this.PathResolver = new PathResolver(this);
        }
    }
}
