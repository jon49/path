#load "./utils/Shell.fs"
open Shell
open Shell.Shell

do

    [
        "microsoft-edge:"
        "outlook"
        "cmder"
    ]
    |> List.iter Shell.run

    [
        //(Shell.create @"C:\Program Files (x86)\GitExtensions\GitExtensions.exe")
        { (Shell.create @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe") with
            WorkingDirectory = Some @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\" }
    ]
    |> List.iter Shell.execute
