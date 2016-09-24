namespace Compiler
{

    public class FasmResult
    {
        public string ErrorResult { get; set; }
        public string Output { get; set; }
    }

    public class FasmHelper
    {
        private readonly CommandExecutor _fasm;
        public FasmHelper() : this("fasm")
        {
        }

        public FasmHelper(string fasmCommand)
        {
            _fasm = new CommandExecutor(fasmCommand);
        }

        public FasmResult Compile(string fileName, string outputFile)
        {
            _fasm.Start(new [] { fileName, outputFile }).WaitForEnd();
            return new FasmResult
            {
                Output = _fasm.ToTextOutput(),
                ErrorResult = _fasm.ToErrorOutput()
            };
        }

        
    }
}