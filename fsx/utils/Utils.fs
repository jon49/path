namespace Utils

module CLI =

    open System

    /// e.g., Console.ReadLine() OR proc.StandardOutput.ReadLine ()
    let rec readlines f = seq {
        let line = f ()
        if line <> null then
            yield line
            yield! readlines f
    }

module Shell =

    open System.Diagnostics

    type StartInfo = {
        Arguments: string
        CreateNoWindow: bool
        Program: string
        UseShellExecute: bool
        WorkingDirectory: string option
        WaitForExit: bool
        }

    let create program =
        { Arguments = ""
          Program = program
          UseShellExecute = false
          WorkingDirectory = None
          CreateNoWindow = true
          WaitForExit = false }

    let run (program : string) =
        Process.Start(program)
        |> ignore

    let private startInfo settings =
        let startInfo = ProcessStartInfo()
        startInfo.FileName <- settings.Program
        startInfo.Arguments <- settings.Arguments
        startInfo.UseShellExecute <- settings.UseShellExecute
        match settings.WorkingDirectory with
        | Some x -> startInfo.WorkingDirectory <- x
        | None -> ()
        startInfo

    let read settings =
        let startInfo = startInfo settings
        startInfo.RedirectStandardOutput <- true
        let proc = Process.Start(startInfo)
        CLI.readlines (fun () -> proc.StandardOutput.ReadLine ())

    let execute settings =
        let startInfo = startInfo settings
        let proc = Process.Start(startInfo)
        if settings.WaitForExit then proc.WaitForExit() else ()

    //let execute workingDirectory program args =
    //    let startInfo = ProcessStartInfo()
    //    startInfo.FileName <- program
    //    startInfo.Arguments <- args
    //    startInfo.UseShellExecute <- true
    //    startInfo.WorkingDirectory <- workingDirectory

    //    let proc = Process.Start(startInfo)
    //    proc.WaitForExit()
    //    ()