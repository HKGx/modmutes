#r "nuget:XPlot.GoogleCharts"
#load "parse_mutes.fsx"
#load "template.fsx"

open System
open XPlot.GoogleCharts
open Parse_mutes
open Template

let dir =
    System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + "/docs")

System.Environment.CurrentDirectory <- dir.FullName

let stopwatch = System.Diagnostics.Stopwatch.StartNew()
printfn "Started generating moderator mute charts"

let getDates (mutes: Mute array) =
    mutes
    |> Array.map (fun m -> m.Date)
    |> Array.countBy (fun d -> d.Date)


let moderatorDatesOfMutes =
    filteredUnknownMutes
    |> Array.groupBy (fun m -> m.Moderator)
    |> Array.map (fun m -> fst (m), snd (m) |> getDates)



let options =
    Options(colorAxis = ColorAxis(minValue = 0, maxValue = 6))

let moderators =
    moderatorDatesOfMutes
    |> Array.map (fst >> (fun m -> m.ToString()))

let lastMute =
    (filteredUnknownMutes |> Array.last).Date

let allMutesCount = filteredUnknownMutes |> Array.length

let getChart (md: Moderator * (DateTime * int) array) =
    let moderator = (fst md).ToString()
    let dates = snd md

    let allModeratorMutes = dates |> Array.sumBy snd

    let percentageOfAllMutes =
        float allModeratorMutes / float allMutesCount
        * 100.0

    moderator,
    dates
    |> Chart.Calendar
    |> Chart.WithTitle
        $"{moderator}, wszystkich mute: {allModeratorMutes}, procent wszystkich: %.1f{percentageOfAllMutes}%%"
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

moderatorDatesOfMutes |> Array.iter gs


("index.html", page lastMute moderators)
|> File.WriteAllText

printfn $"Generated {moderators.Length} charts! It took {stopwatch.Elapsed.TotalMilliseconds}ms"
