#r "nuget:Giraffe.ViewEngine"

open Giraffe.ViewEngine

type PageChartData = { Id: string; JS: string }


let inline_chart (chart: PageChartData) =
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

type MutePageData =
    { Name: string
      AllMutesCount: int
      ModeratorMutesCount: int
      LastMute: System.DateTime }

let body (data: MutePageData) (charts: PageChartData []) =

    let percentageOfAlLMutes =
        (data.ModeratorMutesCount |> float)
        / (data.AllMutesCount |> float)
        * 100.0


    let charts =
        charts
        |> Array.map inline_chart
        |> Array.toList
        |> List.concat

    [ h1 [] [ str data.Name ]
      p [] [
          str $"""Wszystkie muty: {data.AllMutesCount}"""
      ]
      p [] [
          str $"""Wszystkie muty moderatora: {data.ModeratorMutesCount}"""
      ]
      p [] [
          str $"""Procent wszystkich mutów: %.2f{percentageOfAlLMutes}%%"""
      ]
      p [] [
          str $"""Ostatni mute moderatora: {data.LastMute}"""
      ] ]
    @ charts
    |> body []




let mute_page (data: MutePageData) (charts: PageChartData []) =
    html [] [
        head data.Name
        body data charts
    ]
    |> RenderView.AsString.htmlDocument
