#load "./utils/Utils.fs"
open Utils

type Args =
    | Start
    | Stop
    | Help

let args = fsi.CommandLineArgs

let command =
    match args.Length with
    | x when x > 1 ->
        match args.[1].ToLower() with
        | "start" -> Start
        | "end" -> Stop
        | "--help" | "help" | "-?" | "h" | "-h" | _ -> Help
    | _ -> Help

match command with
| Help ->
    printfn """
    | "start" -> Start
    | "end" -> Stop
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

let workPrograms =
    [
        Program.InitHard "vivaldi" "vivaldi"
        Program.InitHard "outlook" "outlook"
        Program.InitHard "" "chrome"
        Program.InitSoft "" "Ssms"
        // Program.Create @"C:\Users\jon.nyman\Downloads\Offerpad\Offerpad.kdbx"
        { (Program.InitHard @"C:\Users\jon.nyman\AppData\Local\Microsoft\Teams\Update.exe" "Teams")
            with Start = Some """--processStart "Teams.exe" """; WorkingDirectory = Some @"C:\Users\jon.nyman\AppData\Local\Microsoft\Teams"  }
        Program.InitHard @"C:\Program Files (x86)\Microsoft Office\root\Office16\lync.exe" "lync"
        { (Program.InitHard @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" "AzureStorageEmulator")
            with Start = Some "start"; Stop = Some "stop"; WorkingDirectory = Some @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\" }
        Program.InitHard @"C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe" "CosmosDBEmulator"
        Program.InitHard @"C:\Users\jon.nyman\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Atlassian\SourceTree" "SourceTree"
        Program.InitSoft @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Visual Studio Code\Visual Studio Code" "Code.exe"
        Program.InitSoft "" "devenv"
    ]

let programs = workPrograms

match command with
| Start ->
    programs
    |> List.iter (fun x ->

        match x.App.Length with
        | 0 -> ()
        | _ -> printfn "Opening program '%s'." x.App

        try
            match x with
            | x when x.App.Length = 0 -> ()
            | {Start = None; WorkingDirectory = None} -> Shell.run x.App
            | {Start = None; WorkingDirectory = Some _} -> Shell.execute { (Shell.create x.App) with WorkingDirectory = x.WorkingDirectory }
            | {Start = Some args} -> Shell.execute { (Shell.create x.App) with WorkingDirectory = x.WorkingDirectory; Arguments = args }
        with
            | ex -> printfn "The program '%s' could not be opened." x.App
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
| _ -> ()
