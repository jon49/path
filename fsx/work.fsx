#load "./utils/Utils.fs"
open Utils

type Args =
    | Start
    | Stop
    | Help

let command =
    let args = fsi.CommandLineArgs
    match args.Length with
    | 2 ->
        match args.[1].ToLower() with
        | "start" | "--start" | "b" | "-b" -> Start
        | "stop" | "--stop" | "e" | "-e" -> Stop
        | "--help" | "-h" | _ -> Help
    | _ -> Help

match command with
| Help ->
    printfn """
    | "--start" | "-b" -> Start
    | "--stop" | "-e" -> Stop
    | "--help" | "-h" | _ -> Help
    """
    failwith ""
| _ -> ()

type Program =
    { App: string
      CloseName: string
      Start: string option
      Stop: string option
      WorkingDirectory: string option }
    static member Create app closeName =
        { App = app 
          CloseName = closeName
          Start = None
          Stop = None
          WorkingDirectory = None }

let programs =
    [
        Program.Create "microsoft-edge:" "MicrosoftEdge"
        Program.Create "outlook" "outlook"
        Program.Create "Teams" "Teams"
        // Program.Create @"C:\Users\jon.nyman\Downloads\Offerpad\Offerpad.kdbx"
        Program.Create @"C:\Program Files (x86)\Microsoft Office\root\Office16\lync.exe" "lync"
        { (Program.Create @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" "AzureStorageEmulator")
            with Start = Some "start"; Stop = Some "stop"; WorkingDirectory = Some @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\" }
        { (Program.Create @"C:\Users\jon.nyman\AppData\Local\SourceTree\SourceTree.exe" "SourceTree")
            with WorkingDirectory = Some @"C:\Users\jon.nyman\AppData\Local\SourceTree\app-2.3.5" }
    ]

match command with
| Start ->
    programs
    |> List.iter (fun x ->
        match x with
        | {Start = None; WorkingDirectory = None} -> Shell.run x.App
        | {Start = None; WorkingDirectory = Some _} -> Shell.execute { (Shell.create x.App) with WorkingDirectory = x.WorkingDirectory }
        | {Start = Some args} -> Shell.execute { (Shell.create x.App) with WorkingDirectory = x.WorkingDirectory; Arguments = args }
    )
| Stop ->
    programs
    |> List.iter (fun x ->
        match x with
        | {Stop = None; CloseName = name} -> Shell.close name
        | {Stop = Some args} ->
            printfn "Closing %s" x.CloseName
            Shell.execute { (Shell.create x.App) with WorkingDirectory = x.WorkingDirectory; Arguments = args }
    )
| Help -> ()
