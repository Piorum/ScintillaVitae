using ScintillaVitae.Discord;

namespace ScintillaVitae;

public class Program
{
    public static readonly CancellationTokenSource _cts = new();
    public static async Task Main()
    {
        await DiscordBot.StartAsync();

        try
        {
            while (!_cts.Token.IsCancellationRequested)
                await Task.Delay(1000);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"{ex}");
        }

        //Cleanup
        await DiscordBot.StopAsync();

        Environment.Exit(0);
    }
}
