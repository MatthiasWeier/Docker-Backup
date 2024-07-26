namespace Services;

/// <summary>
///   Allows executing Processes via command line.
/// </summary>
internal class StartProcessService {
    public static Task<string> ExecuteCommand(string executable, params string[] arguments) => _ExecuteCommand(
        executable,
        arguments.Join(" ").ReplaceLineEndings(" ")
    );

    /// <summary>
    /// Asynchronously executes a specified executable with given arguments and manages file existence checks.
    /// </summary>
    /// <param name="executableName">The name of the executable to run.</param>
    /// <param name="arguments">The arguments to pass to the executable.</param>
    /// <returns>A task that represents the asynchronous operation, which can result in a string that contains the standard output of the executable.</returns>
    /// <exception cref="Exception">Throws an exception if the executable does not exit successfully. The exception includes detailed error information.</exception>
    private static async Task<string> _ExecuteCommand(string executableName, string arguments) {
        var psi = new ProcessStartInfo(executableName) {
            Arguments = arguments,
        };

        Console.WriteLine($"This is my command: {executableName} {psi.Arguments}");

        var isEnded = 0;
        var asyncResult = psi.BeginRedirectedRun();

        var result = await Task.Run(() => {
            try {
                return psi.EndRedirectedRun(asyncResult);
            } finally {
                Interlocked.CompareExchange(ref isEnded, 1, 0);
                Thread.MemoryBarrier();
            }
        });
        if (result.ExitCode == 0)
            return result.StandardOutput;

        throw new($"Could not execute {executableName} {result.ExitCode:X8}:{result.StandardError}") {
            Data = {
                { "Executable", executableName },
                { "Arguments", arguments },
                { "StdError", result.StandardError },
                { "StdOutput", result.StandardOutput }
            }
        };
    }
}