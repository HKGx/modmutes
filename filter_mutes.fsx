#r "nuget:FSharp.Data"

open FSharp.Data
open System.IO

System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

[<Literal>]
let SAMPLE =
    """[{"userId":"any","userTag":"Loxy#1111","date":"1/14/2021, 3:23:38 AM","length":"1 hour","moderator":"TheKrago#5448","reason":"Obrażanie"}]"""

type Mutes = JsonProvider<SAMPLE>

let mutes =
    Mutes.Load($"{__SOURCE_DIRECTORY__}./lista_mute.json")

let onlyDigits (s: string) = s |> Seq.forall System.Char.IsDigit

let onlyDigitsInUserId (m: Mutes.Root) = m.UserId |> onlyDigits

let tuple a b = (a, b)

let filtered = mutes |> Array.filter onlyDigitsInUserId

filtered
|> Array.map (fun m -> m.JsonValue.ToString())
|> String.concat ","
|> sprintf "[\n%s\n]"
|> tuple "mutes.json"
|> File.WriteAllText

printfn $"Filtered out {mutes.Length - filtered.Length} mutes"
