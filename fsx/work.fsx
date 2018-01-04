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
        | "start" | "--start" | "begin" | "--begin" | "b" | "-b" -> Start
        | "stop" | "--stop" | "end" | "--end" | "e" | "-e" -> Stop
        | "--help" | "help" | "-?" | "h" | "-h" | _ -> Help
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
      HardClose: bool
      Start: string option
      Stop: string option
      WorkingDirectory: string option }
    static member Init hardClose app closeName =
        { App = app 
          CloseName = closeName
          HardClose = hardClose
          Start = None
          Stop = None
          WorkingDirectory = None }
    static member InitHard = Program.Init true
    static member InitSoft = Program.Init false

let CreateCloseHard = Program.Init true

let programs =
    [
        Program.InitHard "microsoft-edge:" "MicrosoftEdge"
        Program.InitHard "outlook" "outlook"
        Program.InitHard "Teams" "Teams"
        // Program.Create @"C:\Users\jon.nyman\Downloads\Offerpad\Offerpad.kdbx"
        Program.InitHard @"C:\Program Files (x86)\Microsoft Office\root\Office16\lync.exe" "lync"
        { (Program.InitHard @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" "AzureStorageEmulator")
            with Start = Some "start"; Stop = Some "stop"; WorkingDirectory = Some @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\" }
        { (Program.InitHard @"C:\Users\jon.nyman\AppData\Local\SourceTree\SourceTree.exe" "SourceTree")
            with WorkingDirectory = Some @"C:\Users\jon.nyman\AppData\Local\SourceTree\app-2.3.5" }
        Program.InitSoft "" "devenv"
    ]

match command with
| Start ->
    programs
    |> List.iter (fun x ->
        match x with
        | x when x.App.Length = 0 -> ()
        | {Start = None; WorkingDirectory = None} -> Shell.run x.App
        | {Start = None; WorkingDirectory = Some _} -> Shell.execute { (Shell.create x.App) with WorkingDirectory = x.WorkingDirectory }
        | {Start = Some args} -> Shell.execute { (Shell.create x.App) with WorkingDirectory = x.WorkingDirectory; Arguments = args }
    )
| Stop ->
    programs
    |> List.iter (fun x ->
        match x with
        | {Stop = None; CloseName = name; HardClose = true} -> Shell.hardClose name
        | {Stop = None; CloseName = name; HardClose = false} -> Shell.softClose name
        | {Stop = Some args} ->
            printfn "Closing %s" x.CloseName
            Shell.execute { (Shell.create x.App) with WorkingDirectory = x.WorkingDirectory; Arguments = args }
    )
| Help -> ()
