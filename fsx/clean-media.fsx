
open System.IO
open System

// let dir = __SOURCE_DIRECTORY__
let userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
let downloads = Path.Combine(userDir, "Downloads")
let media = Path.Combine(downloads, "media")
let cleaned = Path.Combine(media, "cleaned")

do
    // Move files in ~/Downloads/*.mp* to ~/Downloads/media/*
    printfn "Moving media files from ~/Downloads"
    Directory.GetFiles(downloads, "*.mp*")
    |> Array.iter (fun x ->
        let filename = Path.GetFileName(x)
        printfn "%s" filename
        File.Move(x, Path.Combine(media, filename))
    )

    // Delete files in ~/Downloads/media if already in ~/Downloads/media/cleaned
    printfn "Removing files in ~Downloads/media which have been cleaned"
    Directory.GetFiles(cleaned)
    |> Array.iter (fun x ->
        let filename = Path.Combine(media, Path.GetFileName(x))
        match File.Exists(filename) with
        | true ->
            printfn "%s" (Path.GetFileName(filename))
            File.Delete(filename)
        | false -> ()
    )
