using System.Collections.Generic;
using System.IO;
using Ion.CodeGeneration.Structure;
using IonCLI.Core;

namespace IonCLI.Engines
{
    public class BuildEngine : OperationEngine
    {
        public BuildEngine(EngineContext context) : base(context)
        {
            //
        }

        public override void Invoke()
        {
            // Prepare engine.
            this.Prepare();

            // Emit the project.
            Dictionary<string, string> modules = this.context.Project.Emit();

            // Create a new tool invoker instace.
            ToolInvoker toolInvoker = new ToolInvoker(this.context.Options);

            // Create a list of emitted Bitcode files.
            List<string> outputBitcodeFiles = new List<string>();

            // Create a list of emitted IR files.
            List<string> outputIrFiles = new List<string>();

            // Loop through all the emitted results.
            foreach ((string fileName, string output) in modules)
            {
                // Form the final, expected output file name.
                string outputFileName = Path.ChangeExtension(fileName, FileExtension.IR);

                // Resolve output IR file's path.
                string outputIrFilePath = this.context.Options.PathResolver.Output(outputFileName);

                // Append IR file to the emitted list.
                outputIrFiles.Add(outputIrFilePath);

                // Write output IR file.
                File.WriteAllText(outputIrFilePath, output);

                // TODO: Ensure it was created/it exists (IR output file).

                // Invoke the LLC tool to compile to object code (Bitcode).
                toolInvoker.Invoke(ToolType.LLC, new string[]
                {
                    "-filetype",
                    "obj",
                    outputIrFilePath
                });

                // TODO: Also ensure it was created/it exists (Bitcode output file).

                // Prepare outputted bitcode filename.
                string outputBitcodeFilePath = Path.ChangeExtension(outputIrFilePath, FileExtension.Bitcode);

                // Append Bitcode file to the emitted list.
                outputBitcodeFiles.Add(outputBitcodeFilePath);
            }

            // TODO: Hard-coded extension.
            // Retrieve the application's identifier.
            string packageIdentifier = $"{this.context.Package.Identifier}.exe";

            // Create the output executable full path.
            string outputExecutablePath = this.context.Options.PathResolver.Output(packageIdentifier);

            // TODO: Hard-coded for Windows.
            // Create the argument list for LLD.
            List<string> args = new List<string>
            {
                "-flavor",
                "link",
                "/NODEFAULTLIB",
                $"/ENTRY:{SpecialName.Main}",
                $"/OUT:{outputExecutablePath}",
                "/SUBSYSTEM:Windows"
            };

            // Append all emitted bitcode files.
            args.AddRange(outputBitcodeFiles);

            // Invoke the linker with the arguments as an array.
            toolInvoker.Invoke(ToolType.LLD, args.ToArray());

            // Ensure program was compiled successful.
            if (!File.Exists(outputExecutablePath))
            {
                Log.Error("Could not create output executable.");
            }

            // Inform the user that cleaning process has begun.
            Log.Verbose("Cleaning up output files.");

            // Cleanup emitted IR files.
            foreach (string outputIrFile in outputIrFiles)
            {
                // Delete the file.
                File.Delete(outputIrFile);
            }

            // Cleanup emitted Bitcode files.
            foreach (string outputBitcodeFile in outputBitcodeFiles)
            {
                // Delete the file.
                File.Delete(outputBitcodeFile);
            }
        }
    }
}