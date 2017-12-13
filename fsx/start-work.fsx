#load "./utils/Shell.fs"
open Shell
open Shell.Shell

do

    [
        "microsoft-edge:"
        "outlook"
        @"C:\Users\jon.nyman\Downloads\Offerpad\Offerpad.kdbx"
        @"C:\Program Files (x86)\Microsoft Office\root\Office16\lync.exe"
    ]
    |> List.iter Shell.run

    [
        { (Shell.create @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe") with
            Arguments = "start"
            WorkingDirectory = Some @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\" }
        { (Shell.create @"C:\Users\jon.nyman\AppData\Local\SourceTree\SourceTree.exe") with
            WorkingDirectory = Some @"C:\Users\jon.nyman\AppData\Local\SourceTree\app-2.3.5" }
    ]
    |> List.iter Shell.execute
