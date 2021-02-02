#load "parse_mutes.fsx"
#load "template.fsx"

open Parse_mutes

let args = fsi.CommandLineArgs |> Array.tail

let shortenModMute (m: Mute) =
    {| moderator = m.Moderator
       reason = m.Reason
       date = m.Date
       length = m.Length |}

let shortenAndToString = shortenModMute >> (sprintf "%A")

let printQueryResult (result: Mute []) =
    printfn "Count: %i" result.Length

    result
    |> Array.map shortenAndToString
    |> String.concat "\n\n"
    |> printf "%s"

let queryId (id: int64): Mute [] =
    filteredMutes
    |> Array.filter (fun m -> m.UserId = id)


(match args.[0] with
 | "id" -> queryId (int64 (args.[1]))
 | _ -> failwith ("unknown query"))
|> printQueryResult
