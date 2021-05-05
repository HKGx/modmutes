open Parse_mutes

#r "nuget:Giraffe.ViewEngine"
#load "parse_mutes.fsx"

open Giraffe.ViewEngine

let moderatorListItem (moderator: Moderator) =
    li [] [
        a [ (sprintf "charts/%s.html" (moderator.ToString())
             |> _href) ] [
            str (moderator.ToString())
        ]
    ]

let head =
    head [] [
        link [ _href "style.css"
               _rel "stylesheet" ]
        meta [ _charset "UTF-8" ]
        title [] [ str "Moderator Mutes" ]
    ]

let body (lastMute: System.DateTime) (moderators: Moderator array) =
    let (activeMods, inactiveMods) = moderators |> Array.partition isActive

    body [] [
        h3 [] [
            sprintf "Ostatni mute: %s" (lastMute.ToString())
            |> str
        ]
        h2 [] [ str "Aktywni" ]
        ul
            []
            (activeMods
             |> Array.map moderatorListItem
             |> Array.toList)


        h2 [] [ str "Nieaktywni" ]
        ul
            []
            (inactiveMods
             |> Array.map moderatorListItem
             |> Array.toList)
    ]

let index (lastMute: System.DateTime) (moderators: Moderator array) =
    html [ _lang "pl" ] [
        head
        body lastMute moderators
    ]
    |> RenderView.AsString.htmlDocument
