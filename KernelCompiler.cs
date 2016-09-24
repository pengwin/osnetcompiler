using System;
using System.IO;

namespace Compiler
{

    public class AsmCompilerException : Exception
    {
        public AsmCompilerException (string message) : base(message)
        {
          
        }
    }

    public class Kernel
    {
        public string Sources { get; private set; }

        public string KernelDirectory { get; private set; }

        public string SourcesPath { get; set; }

        public string BinaryPath { get; private set; }

        public Kernel (string sources, string kernelName)
        {
            Sources = sources;
            KernelDirectory = Path.Combine("tmp", Guid.NewGuid().ToString());
            SourcesPath = Path.Combine(KernelDirectory, kernelName + ".asm");
            BinaryPath = Path.Combine(KernelDirectory, kernelName + ".bin");
        }

        public void Clean()
        {
            Directory.Delete(KernelDirectory, true);
        }
    }

    public class KernelCompiler
    {
        private readonly bool _doNotClean;

        public KernelCompiler (bool doNotClean = false)
        {
          _doNotClean = doNotClean;
        }

        public Kernel CompileKernel(string sources, string kernelName)
        {
            var compiler = new FasmHelper();
            var kernel = new Kernel(sources, kernelName);
            Prepare(kernel);
            var result = compiler.Compile(kernel.SourcesPath, kernel.BinaryPath);
            if (!string.IsNullOrEmpty(result.ErrorResult))
            {
                throw new AsmCompilerException(result.ErrorResult);
            }
            return kernel;
        }

        private void Prepare(Kernel kernel)
        {
            Directory.CreateDirectory(kernel.KernelDirectory);
            File.WriteAllText(kernel.SourcesPath, kernel.Sources);
        }
    }
}