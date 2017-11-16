namespace Shell
module Shell =

    open System.Diagnostics

    let execute workingDirectory program args =
        let startInfo = ProcessStartInfo()
        startInfo.FileName <- program
        startInfo.Arguments <- args
        startInfo.UseShellExecute <- true
        startInfo.WorkingDirectory <- workingDirectory

        let proc = Process.Start(startInfo)
        proc.WaitForExit()
        ()
