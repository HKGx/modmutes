#r "nuget:Giraffe.ViewEngine"

open Giraffe.ViewEngine

type PageChart = { Id: string; JS: string }


let inline_chart (chart: PageChart) =
    [ script [] [ chart.JS |> rawText ]
      div [ _id chart.Id
            _style "width: 100vw; height: 100vh" ] [] ]


let head (name: string) =
    head [] [
        link [ _href "../style.css"
               _rel "stylesheet" ]
        meta [ _charset "UTF-8" ]
        title [] [ str name ]
        script [ _src "https://www.gstatic.com/charts/loader.js" ] []
        script [] [
            rawText """google.charts.load("current", {packages:["corechart", "calendar"]});"""
        ]
    ]

let body (name: string) (charts: PageChart []) =

    body
        []
        (h1 [] [ str name ]
         :: (charts
             |> Array.map inline_chart
             |> Array.toList
             |> List.concat))



let mute_page (name: string) (charts: PageChart []) =
    html [] [ head name; body name charts ]
    |> RenderView.AsString.htmlDocument
