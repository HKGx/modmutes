#r "nuget:XPlot.GoogleCharts"
#load "parse_mutes.fsx"

open System
open XPlot.GoogleCharts
open Parse_mutes

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let getDates (mutes: Mute array) =
    mutes
    |> Array.map (fun m -> m.Date)
    |> Array.countBy (fun d -> d.Date)


let moderatorDatesOfMutes =
    filteredUnknownMutes
    |> Array.groupBy (fun m -> m.Moderator)
    |> Array.map (fun m -> fst (m), snd (m) |> getDates)



let options =
    Options(colorAxis = ColorAxis(minValue = 1, maxValue = 6, colors = [| "#ff5f38"; "#1aff1a" |]))

let moderators =
    moderatorDatesOfMutes
    |> Array.map (fst >> (fun m -> m.ToString()))

let lastMute =
    (filteredUnknownMutes |> Array.last)
        .Date.ToString()

let getChart (md: Moderator * (DateTime * int) array) =
    let moderator = (fst md).ToString()
    let dates = snd md
    let allMutes = Array.sumBy snd dates

    moderator,
    dates
    |> Chart.Calendar
    |> Chart.WithTitle $"{moderator}, wszystkich mute: {allMutes}"
    |> Chart.WithHeight 600
    |> Chart.WithWidth 1000
    |> Chart.WithOptions options

open System.IO

let save (mc: string * GoogleChart) =
    let (moderator, chart) = mc

    let html =
        chart
            .GetHtml()
            .Replace("<body>",
                     $"""<body style="background-color:#181a1b;"><h3 style="color: #ffffff"> Stan na {lastMute}</h3>""")

    let curr = Directory.GetCurrentDirectory()
    let file = $"{moderator}.html"
    Directory.CreateDirectory("charts") |> ignore
    let path = Path.Combine(curr, "charts", file)
    File.WriteAllText(path, html)


let gs = getChart >> save

// moderatorDatesOfMutes |> Array.iter gs
moderatorDatesOfMutes |> Array.iter gs



let moderatorsHtmlList (mods: string array) =
    let getLi m =
        sprintf """        <li> <a href="charts/%s.html">%s</li>""" m m

    let listItems =
        Array.map getLi mods |> String.concat "\n"

    $"<ul>\n{listItems}\n    </ul>"



let template = File.ReadAllText("./_index.html")

let htmlList =
    moderators |> Array.sort |> moderatorsHtmlList



("index.html",
 template
     .Replace("{{LIST}}", htmlList.ToString())
     .Replace("{{LAST}}", lastMute))
|> File.WriteAllText


printfn $"Generated {moderators.Length} charts!"
