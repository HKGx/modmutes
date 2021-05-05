#r "nuget:XPlot.GoogleCharts"
#load "parse_mutes.fsx"
#load "template_index.fsx"
#load "template_mutes.fsx"

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
    mutesByModerators
    |> Array.map fst
    |> Array.distinct


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

let save (moderator: Moderator) (charts: GoogleChart []) =

    let chartData (chart: GoogleChart) : PageChartData =
        let id = chart.Id
        let js = chart.GetInlineJS()

        { Id = id; JS = js }

    let pageData =
        let mutes_count = filteredUnknownMutes |> Array.length

        let moderator_mutes =
            filteredUnknownMutes
            |> Array.filter (fun m -> m.Moderator = moderator)

        let moderator_mutes_count = moderator_mutes |> Array.length

        let last_mute_date = (moderator_mutes |> Array.last).Date

        { Name = moderator.ToString()
          AllMutesCount = mutes_count
          ModeratorMutesCount = moderator_mutes_count
          LastMute = last_mute_date }

    let page =
        mute_page pageData (charts |> Array.map chartData)

    let curr = Directory.GetCurrentDirectory()
    let file = $"{moderator}.html"
    Directory.CreateDirectory("charts") |> ignore
    let path = Path.Combine(curr, "charts", file)
    File.WriteAllText(path, page)

for moderator in moderatorDatesOfMutes |> Array.map fst do
    save moderator [| getCalendarHeatmapChart moderator |]


("index.html", index lastMute moderators)
|> File.WriteAllText

let indexPath =
    Path.Combine(Directory.GetCurrentDirectory(), "index.html")

printfn $"Generated {moderators.Length} charts! It took {stopwatch.Elapsed.TotalMilliseconds}ms"
printfn $"index.html located at: {indexPath}"
