using System;
using System.Text;

namespace Compiler
{

	public class MultibootHeader
	{
		public UInt32 MultibootStartAddress { get; private set; }

		public UInt32 MultibootMagic { get; private set; }

		public MultibootHeader()
		{
		  MultibootStartAddress = 0x00100000;
		  MultibootMagic = 0x1BADB002;
		}
	}

    public class MultibootKernelBuilder
    {

		private readonly StringBuilder _builer;

		public MultibootKernelBuilder ()
		{
		  _builer = new StringBuilder();
		}

		public MultibootKernelBuilder AddMultibootStart(MultibootHeader header)
		{
			_builer
			.AppendLine("use32")
			.AppendLine($"UMB_START = 0x{header.MultibootStartAddress:X8}")
			.AppendLine("org UMB_START");

			return this;
		}

		public MultibootKernelBuilder AddMultibootHeader(MultibootHeader header)
		{
			_builer
			.AppendLine($@"
MB_MAGIC 			= 0x{header.MultibootMagic:X}
MB_F_BOOTALIGNED	= 1
MB_F_MEMINFO		= (1 shl 1)
MB_F_VIDEOTABLE		= (1 shl 2)
MB_F_USE_MBOFFSETS	= (1 shl 16)
MB_FLAGS 			= MB_F_BOOTALIGNED or MB_F_MEMINFO or MB_F_USE_MBOFFSETS
MB_CHECKSUM 		= dword - (MB_MAGIC + MB_FLAGS)
VIDEO_MODE 			= 1 ; EGA text mode
VIDEO_WIDTH 		= 80
VIDEO_HEIGHT 		= 25
VIDEO_DEPTH 		= 0

multiboot_header:
				dd MB_MAGIC		; Multiboot magic number
flags:	   		dd MB_FLAGS
checksum:		dd MB_CHECKSUM
header_addr:	dd multiboot_header
load_addr:		dd multiboot_header	; start of program text
load_end_addr:	dd load_end		; end of program text+data
bss_end_addr:	dd bss+10000h
entry_addr:		dd entry_point		; 
mode_type:		dd VIDEO_MODE		; Video mode
width:			dd VIDEO_WIDTH		; Video Width
height:			dd VIDEO_HEIGHT		; Video Height
depth:			dd VIDEO_DEPTH		; Vide Depth");

			return this;
		}

		public MultibootKernelBuilder AddEntryPoint()
		{
			_builer.AppendLine(@"
entry_point:
	mov eax, 0x1234
	jmp $
	");
			return this;
		}

		public MultibootKernelBuilder AddEnd()
		{
			_builer.AppendLine(@"

fail:
	hlt
load_end:

bss:
mbis:	rd 0
umb_end: rd 0
");
			return this;
		}

		/*  check multiboot
		cmp eax,0x2BADB002	; Are we multibooted
		je multiboot
		*/


        public string Build()
        {
            return _builer.ToString();
        }

    }
}