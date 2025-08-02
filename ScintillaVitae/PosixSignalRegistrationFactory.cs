using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ScintillaVitae;

public static class PosixSignalRegistrationFactory
{
    public static PosixSignalRegistration? SigtermRegistration { private set; get; }

    [ModuleInitializer]
    public static void Init()
    {
        SigtermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, sig =>
        {
            Console.WriteLine("Received SIGTERM, Shutting Down");
            sig.Cancel = true;
            Program._cts.Cancel();
        });
    }

}
