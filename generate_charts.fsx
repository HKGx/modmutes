#r "nuget:XPlot.GoogleCharts"
#load "parse_mutes.fsx"
#load "template_index.fsx"
#load "template_mutes.fsx"

open System
open XPlot.GoogleCharts
open Parse_mutes
open Template_index
open Template_mutes

let dir =
    System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + "/docs")

System.Environment.CurrentDirectory <- dir.FullName

let stopwatch = System.Diagnostics.Stopwatch.StartNew()
printfn "Started generating moderator mute charts"




let moderators =
    (Reflection.FSharpType.GetUnionCases(typeof<Moderator>)
     |> Array.map (fun m -> m.Name))
    |> Array.filter (fun n -> n <> "Unknown")
    |> Array.sort


let lastMute =
    (filteredUnknownMutes |> Array.last).Date


let getCalendarHeatmapChart (moderator: Moderator) =
    let options =
        Options(colorAxis = ColorAxis(minValue = 0, maxValue = 6))

    let moderatorMutes =
        moderatorDatesOfMutes
        |> Array.find (fun (m, _) -> m = moderator)
        |> snd

    moderatorMutes
    |> Chart.Calendar
    |> Chart.WithTitle "Wyciszenia na dzień"
    |> Chart.WithOptions options

let getMuteByHourChart (moderator: Moderator) =
    let mutes =
        mutesByModerators
        |> Array.find (fun (m, _) -> m = moderator)
        |> snd

    let mutesByHour =
        mutes
        |> Array.groupBy (fun m -> m.Date.Hour)
        |> Array.map (fun (h, m) -> (h.ToString(), m.Length))

    mutesByHour
    |> Chart.Histogram
    |> Chart.WithTitle "Mute o danych godzinach"

open System.IO

let save (moderator: string) (charts: GoogleChart []) =

    let chartData (chart: GoogleChart) : PageChart =
        let id = chart.Id
        let js = chart.GetInlineJS()

        { Id = id; JS = js }


    let page =
        mute_page moderator (charts |> Array.map chartData)

    let curr = Directory.GetCurrentDirectory()
    let file = $"{moderator}.html"
    Directory.CreateDirectory("charts") |> ignore
    let path = Path.Combine(curr, "charts", file)
    File.WriteAllText(path, page)

for moderator in moderatorDatesOfMutes |> Array.map fst do
    save
        (moderator.ToString())
        [| getCalendarHeatmapChart moderator
           getMuteByHourChart moderator |]


("index.html", index lastMute moderators)
|> File.WriteAllText

printfn $"Generated {moderators.Length} charts! It took {stopwatch.Elapsed.TotalMilliseconds}ms"
