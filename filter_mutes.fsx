#r "nuget:FSharp.Data"

open FSharp.Data
open System.IO

let dir =
    System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + "/docs")

System.Environment.CurrentDirectory <- dir.FullName


let stopwatch = System.Diagnostics.Stopwatch.StartNew()
printfn "Started filtering mutes"

[<Literal>]
let SAMPLE =
    """[{"userId":"any","userTag":"Loxy#1111","date":"1/14/2021, 3:23:38 AM","length":"1 hour","moderator":"TheKrago#5448","reason":"Obrażanie"}]"""

type Mutes = JsonProvider<SAMPLE>

let mutes =
    Mutes.Load($"{__SOURCE_DIRECTORY__}/lista_mute.json")

let lastValue = (mutes |> Array.last).JsonValue


let mutable lastFilterMuteFlag = true

[<Literal>]
let LAST_FILTER_MUTE_PATH =
    __SOURCE_DIRECTORY__ + "/last_filter_mute.json"

if File.Exists(LAST_FILTER_MUTE_PATH) then
    let value =
        JsonValue.Load(File.OpenRead(LAST_FILTER_MUTE_PATH))

    if lastValue = value then
        lastFilterMuteFlag <- false
    else
        let last = mutes |> Array.last
        File.WriteAllText(LAST_FILTER_MUTE_PATH, last.JsonValue.ToString())
else
    let last = mutes |> Array.last
    File.WriteAllText(LAST_FILTER_MUTE_PATH, last.JsonValue.ToString())




let onlyDigits (s: string) = s |> Seq.forall System.Char.IsDigit

let onlyDigitsInUserId (m: Mutes.Root) = m.UserId |> onlyDigits

let tuple a b = (a, b)


if lastFilterMuteFlag then
    let filtered = mutes |> Array.filter onlyDigitsInUserId

    filtered
    |> Array.map (fun m -> m.JsonValue.ToString(JsonSaveOptions.DisableFormatting))
    |> String.concat ","
    |> sprintf "[%s]"
    |> tuple "mutes.json"
    |> File.WriteAllText

    stopwatch.Stop()
    printfn $"Filtered out {mutes.Length - filtered.Length} mutes in {stopwatch.Elapsed.TotalMilliseconds}ms"
else
    stopwatch.Stop()

    printfn $"Last mute hasn't changed. It took us {stopwatch.Elapsed.TotalMilliseconds}ms"
