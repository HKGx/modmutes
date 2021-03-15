#r "nuget:Giraffe.ViewEngine"

open Giraffe.ViewEngine

let moderatorListItem (moderator: string) =
    li [] [
        a [ (sprintf "charts/%s.html" moderator |> _href) ] [
            str moderator
        ]
    ]

let head =
    head [] [
        link [ _href "style.css"
               _rel "stylesheet" ]
        meta [ _charset "UTF-8" ]
        title [] [ str "Moderator Mutes" ]
    ]

let body (lastMute: System.DateTime) (moderators: string array) =
    body [] [
        h3 [] [
            sprintf "Ostatni mute: %s" (lastMute.ToString())
            |> str
        ]
        ul [] [
            for moderator in moderators -> moderatorListItem moderator
        ]
    ]

let index (lastMute: System.DateTime) (moderators: string array) =
    html [ _lang "pl" ] [
        head
        body lastMute moderators
    ]
    |> RenderView.AsString.htmlDocument