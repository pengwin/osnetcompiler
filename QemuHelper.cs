using System;

namespace Compiler
{

    public class Register
    {
        private readonly string _register;

        public Register (string register)
        {
            _register = register;
        } 

        public override string ToString()
        {
            return _register; 
        }
    }

    public static class Registers
    {
        public static readonly Register EAX = new Register("eax");
    }

    public class QemuHelper
    {
        private readonly string _qemuGreetings;

        private readonly CommandExecutor _qemu;
        public QemuHelper() : this("qemu-system-i386", "2.5.0")
        {
        }

        public QemuHelper(string qemuCommand, string qemuVersion)
        {
            _qemuGreetings = string.Format("QEMU {0} monitor - type 'help' for more information", qemuVersion);
            _qemu = new CommandExecutor(qemuCommand);
        }

        public QemuHelper Start(string kernel)
        {
            var args = new [] { "-monitor stdio", string.Format("-kernel {0}", kernel)};

            StartProcess(args);

            return PrintDebugMesage("Qemu started");
        }

        public QemuHelper Stop()
        {
            _qemu.SendCommand("print $eax")
            .SendCommand("q")
            .WaitForEnd();
            PrintDebugMesage("Qemu ended");
            return this;
        }

        public QemuHelper WaitForAddressValue(UInt32 address, UInt32 value)
        {
            var adressFormatted = string.Format("xp /1dwx 0x{0}", address.ToString("X").ToLower());
            var valueFormatted = string.Format("{0}: 0x{1}", address.ToString("X16"), value.ToString("X").ToLower());
            _qemu.WaitCommandResult(adressFormatted, valueFormatted);

            return this;
        }

        public QemuHelper WaitForRegisterValue(Register register, UInt32 value)
        {
            var adressFormatted = string.Format("print ${0}", register.ToString());
            var valueFormatted = string.Format("0x{0}", value.ToString("X").ToLower());
            _qemu.WaitCommandResult(adressFormatted, valueFormatted);

            return this;
        }

        public QemuHelper PrintDebugMesage(string message)
        {
            Console.WriteLine("DEBUG: {0}", message);
            return this;
        }

        private void StartProcess(string[] arguments)
        {
            _qemu.Start(arguments)
            .WaitLastOutputLine(_qemuGreetings);
        }
    }
}