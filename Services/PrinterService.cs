using System.Runtime.InteropServices;
using System.Text;

namespace NextInLine.Services;

public class PrinterService
{
    private readonly string _printerName;

    public PrinterService(IConfiguration configuration)
    {
        _printerName = configuration["Printer:Name"] ?? "XP-58";
    }

    public void PrintTicket(string ticketCode, string userName)
    {
        try
        {
            Console.WriteLine($"[Printer] Imprimiendo en: '{_printerName}'");
            var bytes = BuildTicket(ticketCode, userName);
            RawPrinterHelper.SendBytesToPrinter(_printerName, bytes);
            Console.WriteLine($"[Printer] OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Printer] ERROR: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static byte[] BuildTicket(string code, string name)
    {
        using var ms = new MemoryStream();

        void Write(params byte[] data) => ms.Write(data, 0, data.Length);
        void WriteText(string text) => ms.Write(Encoding.ASCII.GetBytes(text));

        Write(0x1B, 0x40);           // Init
        Write(0x1B, 0x61, 0x01);     // Center
        Write(0x1B, 0x45, 0x01);     // Bold ON
        WriteText("NEXT IN LINE\r\n");
        Write(0x1B, 0x45, 0x00);     // Bold OFF
        WriteText("Sistema de Turnos\r\n");
        WriteText("--------------------------------\r\n");
        Write(0x1B, 0x61, 0x00);     // Left
        WriteText($"Usuario : {name}\r\n");
        WriteText($"Fecha   : {DateTime.Now:dd/MM/yyyy HH:mm}\r\n");
        WriteText("--------------------------------\r\n");
        Write(0x1B, 0x61, 0x01);     // Center
        Write(0x1D, 0x21, 0x11);     // Double width+height
        WriteText($"{code}\r\n");
        Write(0x1D, 0x21, 0x00);     // Normal
        WriteText("--------------------------------\r\n");
        WriteText("Espere su turno\r\n");
        WriteText("Gracias por su visita\r\n");
        Write(0x0A, 0x0A, 0x0A, 0x0A, 0x0A); // Feed
        Write(0x1D, 0x56, 0x00);     // Full cut

        return ms.ToArray();
    }
}

internal static class RawPrinterHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DOCINFO
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string pDocName;
        [MarshalAs(UnmanagedType.LPWStr)] public string? pOutputFile;
        [MarshalAs(UnmanagedType.LPWStr)] public string pDataType;
    }

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFO di);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    public static void SendBytesToPrinter(string printerName, byte[] bytes)
    {
        Console.WriteLine($"[RawPrint] OpenPrinter '{printerName}'");

        if (!OpenPrinter(printerName, out var hPrinter, IntPtr.Zero))
            throw new Exception($"OpenPrinter fallo. Win32: {Marshal.GetLastWin32Error()}");

        Console.WriteLine($"[RawPrint] Handle: {hPrinter}");

        var di = new DOCINFO
        {
            pDocName    = "Ticket",
            pOutputFile = null,
            pDataType   = "RAW"
        };

        int docId = StartDocPrinter(hPrinter, 1, ref di);
        Console.WriteLine($"[RawPrint] StartDocPrinter: {docId}");

        if (docId <= 0)
        {
            int err = Marshal.GetLastWin32Error();
            ClosePrinter(hPrinter);
            throw new Exception($"StartDocPrinter fallo. Win32: {err}");
        }

        if (!StartPagePrinter(hPrinter))
        {
            int err = Marshal.GetLastWin32Error();
            EndDocPrinter(hPrinter);
            ClosePrinter(hPrinter);
            throw new Exception($"StartPagePrinter fallo. Win32: {err}");
        }

        Console.WriteLine($"[RawPrint] StartPagePrinter OK");

        var pBytes = Marshal.AllocCoTaskMem(bytes.Length);
        try
        {
            Marshal.Copy(bytes, 0, pBytes, bytes.Length);
            bool written = WritePrinter(hPrinter, pBytes, bytes.Length, out int dwWritten);
            Console.WriteLine($"[RawPrint] WritePrinter: {written}, bytes: {dwWritten}");

            if (!written)
                throw new Exception($"WritePrinter fallo. Win32: {Marshal.GetLastWin32Error()}");
        }
        finally
        {
            Marshal.FreeCoTaskMem(pBytes);
        }

        EndPagePrinter(hPrinter);
        EndDocPrinter(hPrinter);
        ClosePrinter(hPrinter);
        Console.WriteLine($"[RawPrint] Listo.");
    }
}