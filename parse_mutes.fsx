#r "nuget:FSharp.Data"
#load "filter_mutes.fsx"

open FSharp.Data
open System


let stopwatch = System.Diagnostics.Stopwatch.StartNew()
printfn "Started parsing mutes"


[<Literal>]
let SAMPLE =
    """[{"userId":"646407598077247499","userTag":"Loxy#1111","date":"1/14/2021, 3:23:38 AM","length":"1 hour","moderator":"TheKrago#5448","reason":"Obrażanie"}]"""

type Mutes = JsonProvider<SAMPLE>

let mutes =
    Mutes.Load($"{__SOURCE_DIRECTORY__}/docs/mutes.json")

type Moderator =
    | Rei
    | Defous
    | Szejder
    | Barthes8
    | Irchrael
    | Mao
    | Dziecioruch
    | Gethon
    | Asiua
    | Meivels
    | Eclair
    | KotSerafin
    | Novaxon
    | MrMarshal
    | InkwizytorTybalt
    | Neasch
    | YourName
    | Pawwit
    | Ivriel
    | Yui
    | Echo
    | Mikki
    | Hkg
    | Raca
    | Sathay
    | Predator
    | DeathStrike
    | Daenora
    | WujekPienio
    | Elisia
    | Pluszak
    | Jonek
    | Altsin
    | Koza
    | Yarrow
    | S0rry
    | Madesio
    | Buniov
    | Snowcio
    | Total
    | Why
    | Daguows
    | MaleBiale
    | Tumi
    | Cerbercia
    | FuckMyDepression
    | Amesdo
    | Freeze
    | PofarbowanyLis
    | JakisTamKuba
    | Kernelly
    | Budzik
    | Naciak
    | JKR
    | Sekirei
    | RestInPeace
    | Kitty
    | Sparta
    | Ary
    | Vesti
    | UpperKind
    | Devilso
    | Eiby
    | JustMarcin
    | Misa
    | Raxor
    | Dgaday
    | Herbatka
    | Xemi
    | Miuz
    | Advance
    | Krago
    | Rzepa
    | Unknown of string

let (|Contains|_|) (what: string) (str: string) =
    if str.Contains(what) then
        Some(str)
    else
        None

let (|ContainsMany|_|) (what: string list) (str: string) =
    if what |> List.exists (fun s -> str.Contains(s)) then
        Some(str)
    else
        None

type Mute =
    { UserId: int64
      UserTag: string
      Date: DateTime
      Length: string
      Moderator: Moderator
      Reason: string }

let parseModerator (m: Mutes.Root) =
    let moderator =
        match m.Moderator.ToLower() with
        | Contains "rei~" _ -> Rei
        | Contains "defous" _ -> Defous
        | Contains "szejder" _ -> Szejder
        | Contains "barthes" _ -> Barthes8
        | ContainsMany [ "irchrael"; "ק๏๓ภเк" ] _ -> Irchrael
        | Contains "mao" _ -> Mao
        | ContainsMany [ "turbodres"; "dziecioruch" ] _ -> Dziecioruch
        | Contains "gethon" _ -> Gethon
        | Contains "asiua" _ -> Asiua
        | Contains "meivels" _ -> Meivels
        | Contains "eclair" _ -> Eclair
        | ContainsMany [ "kotserafin"; "kot serafin" ] _ -> KotSerafin
        | Contains "novaxon" _ -> Novaxon
        | Contains "mrmarshal" _ -> MrMarshal
        | Contains "inkwizytor tybalt" _ -> InkwizytorTybalt
        | Contains "neasch von" _ -> Neasch
        | Contains "your.name" _ -> YourName
        | Contains "pawwit" _ -> Pawwit
        | Contains "ivriel" _ -> Ivriel
        | ContainsMany [ "yuichi" ] _ -> Yui
        | ContainsMany [ "echo"; "ram#3140" ] _ -> Echo
        | ContainsMany [ "mikki"; "migren" ] _ -> Mikki
        | ContainsMany [ "krulkrukuw"; "hkg" ] _ -> Hkg
        | Contains "raca" _ -> Raca
        | Contains "sathay" _ -> Sathay
        | Contains "predator" _ -> Predator
        | Contains "deathstrike" _ -> DeathStrike
        | Contains "daenora" _ -> Daenora
        | Contains "wujekpienio" _ -> WujekPienio
        | Contains "elisia" _ -> Elisia
        | ContainsMany [ "justme"; "pluszak" ] _ -> Pluszak
        | Contains "jonek" _ -> Jonek
        | Contains "koza" _ -> Koza
        | Contains "yarrow" _ -> Yarrow
        | Contains "altsin" _ -> Altsin
        | Contains "s0rry" _ -> S0rry
        | Contains "madesio" _ -> Madesio
        | Contains "buniov" _ -> Buniov
        | Contains "snow" _ -> Snowcio
        | Contains "total" _ -> Total
        | ContainsMany [ "why"; "spojówka" ] _ -> Why
        | Contains "daguows" _ -> Daguows
        | Contains "małe" _ -> MaleBiale
        | Contains "tumi" _ -> Tumi
        | Contains "cerbercia" _ -> Cerbercia
        | ContainsMany [ "𝖋𝖚𝖈𝖐 𝖒𝖞 𝖉𝖊𝖕𝖗𝖊𝖘𝖘𝖎𝖔𝖓"; "fuck my depression" ] _ -> FuckMyDepression
        | Contains "amesdo" _ -> Amesdo
        | Contains "freeze" _ -> Freeze
        | Contains "pofarbowany lis" _ -> PofarbowanyLis
        | Contains "jakistamkuba" _ -> JakisTamKuba
        | Contains "kernelly" _ -> Kernelly
        | Contains "budzik" _ -> Budzik
        | Contains "naciak" _ -> Naciak
        | Contains "jkr" _ -> JKR
        | ContainsMany [ "stachu220"; "sekirei" ] _ -> Sekirei
        | Contains "řƹȿʈ ɨɲ ρƹąȼƹ" _ -> RestInPeace
        | Contains "kitty" _ -> Kitty
        | ContainsMany [ "sp4rt4"; "baby yoda" ] _ -> Sparta
        | ContainsMany [ "larysa"; "ary" ] _ -> Ary
        | ContainsMany [ "carnotaurus"; "saturn"; "raza'kiri" ] _ -> Vesti
        | Contains "upperkind" _ -> UpperKind
        | Contains "devilso" _ -> Devilso
        | Contains "eiby" _ -> Eiby
        | Contains "dgaday" _ -> Dgaday
        | Contains "just marcin" _ -> JustMarcin
        | Contains "misa" _ -> Misa
        | Contains "ra1x1or" _ -> Raxor
        | Contains "herbatka" _ -> Herbatka
        | Contains "xemi" _ -> Xemi
        | Contains "miuz" _ -> Miuz
        | Contains "advancee" _ -> Advance
        | Contains "thekrago" _ -> Krago
        | Contains "turnip" _ -> Rzepa
        | s when String.IsNullOrEmpty s -> Unknown "brak danych"
        | s -> Unknown s

    { UserId = m.UserId
      UserTag = m.UserTag
      Date = m.Date
      Length = m.Length
      Reason = m.Reason
      Moderator = moderator }

let moderatorMutes = mutes |> Array.map parseModerator

let containsRuletka (m: Mute) =
    [ "ruletk"; "na potęgę" ]
    |> List.exists (fun r -> m.Reason.ToLower().Contains r)

let shortenedMute (m: Mute) = m.Length.StartsWith("-")

let muteFilter m =
    containsRuletka m |> not && shortenedMute m |> not

let filteredMutes =
    moderatorMutes |> Array.filter muteFilter

let fromUnknown m =
    match m.Moderator with
    | Unknown (_) -> true
    | _ -> false

let filteredUnknownMutes =
    filteredMutes |> Array.filter (fromUnknown >> not)


stopwatch.Stop()

printfn $"Parsed mutes in {stopwatch.Elapsed.TotalMilliseconds}ms"