namespace Shell
module Shell =

    open System.Diagnostics

    type StartInfo = {
        Arguments: string
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
          WaitForExit = false }

    let run (program : string) =
        Process.Start(program)
        |> ignore

    let execute settings =
        let startInfo = ProcessStartInfo()
        startInfo.FileName <- settings.Program
        startInfo.Arguments <- settings.Arguments
        startInfo.UseShellExecute <- settings.UseShellExecute
        match settings.WorkingDirectory with
        | Some x -> startInfo.WorkingDirectory <- x
        | None -> ()

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
