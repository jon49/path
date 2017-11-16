// http://builds.libav.org/windows/release-gpl/
#load "./utils/Shell.fs"
open System
open System.IO
open System.Diagnostics
open Shell

let dir = __SOURCE_DIRECTORY__
let userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
let media = Path.Combine(userDir, "Downloads", "media")

let ``youtube-dl`` = Path.Combine(dir, "../youtube-dl", "youtube-dl.exe")

type Choices =
    | LQ_MP3
    | HQ_MP3
    | LQ_Video
    | HQ_Video

let youtube choice links =
    let title = """-o "%(title)s-%(id)s.%(ext)s" """
    let audio = sprintf "-f 17 %s -x --audio-format mp3 --audio-quality %i %s" title
    match choice with
    | LQ_MP3 -> audio 9 links
    | HQ_MP3 -> audio 1 links
    | LQ_Video -> sprintf "-f 18 %s %s" title links
    | HQ_Video -> sprintf """--max-quality "mp4" %s %s""" title links

let getChoice (num : string) =
    match num.Trim() with
    | "1" -> LQ_MP3
    | "2" -> HQ_MP3
    | "3" -> LQ_Video
    | "4" -> HQ_Video
    | _ -> failwith "Can only choose 1, 2, 3, or 4."

do
    printfn "
Choose file type:
1. LQ MP3
2. HQ MP3
3. LQ Video
4. HQ Video"
    let choice = Console.ReadLine () |> getChoice
    printfn "Enter links you would like to download:"
    let links = Console.ReadLine () |> fun x -> x.Trim()
    Shell.execute media ``youtube-dl`` (youtube choice links) |> ignore
