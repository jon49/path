#load "./utils/Shell.fs"
open Shell
open Shell.Shell

do

    [
        "microsoft-edge:"
        "outlook"
        @"C:\Users\jon.nyman\Downloads\Offerpad\Offerpad.kdbx"
    ]
    |> List.iter Shell.run

    [
        { (Shell.create @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe") with
            Arguments = "start"
            WorkingDirectory = Some @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\" }
    ]
    |> List.iter Shell.execute
