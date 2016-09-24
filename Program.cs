using System; 
using System.Reflection;
using Compiler;

namespace ConsoleApplication
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LowLevelAttribute : Attribute
    {

    }

    public static class Test
    {
        [LowLevel]
        public static void SetRegister(Register reg, UInt32 value)
        {

        }
    }



    public class Program
    {
        public static void Main(string[] args)
        {

            //var command = new CommandExecutor("qemu-system-i386",new [] { "-monitor stdio", "-kernel kernel.bin"}).Start();

            //Console.ReadLine();

            //var methodBody = (typeof(Test)).GetMethod("SetRegister").GetILBytes();
            
            
            
            
            var multibootHeader = new MultibootHeader();
            var kernelSources = new MultibootKernelBuilder()
            .AddMultibootStart(multibootHeader)
            .AddMultibootHeader(multibootHeader)
            .AddEntryPoint()
            .AddEnd()
            .Build();


            var kernel = new KernelCompiler().CompileKernel(kernelSources, "kernel");

            new QemuHelper()
            .Start(kernel.BinaryPath)
            .WaitForAddressValue(0x00100000, 0x1badb002)            
            .PrintDebugMesage("Multiboot kernel loaded")
            .WaitForRegisterValue(Registers.EAX, 0x1234)
            .PrintDebugMesage("Kernel executed")
            .Stop();

            kernel.Clean();
        }
    }
}
