open SixDegrees
open System.IO
open FSharp.Data.Sql
open System.Data.SQLite

[<EntryPoint>]
let main argv =
    if argv.Length > 0 then
        if argv.[0] = "--update" then
            let nameUrl =
                "https://datasets.imdbws.com/name.basics.tsv.gz"

            let compressedNameFile =
                __SOURCE_DIRECTORY__ + @"\Data\name-basics.tsv.gz"

            Http.DownloadByteFile nameUrl compressedNameFile
            |> GZip.Decompress
            |> ignore

            Name.ExecuteImportCommand |> ignore
    //|> (fun proc ->
    //proc.WaitForExit()

    //proc.ExitCode
    //|> printfn "Update Complete with exit code %i")
    //|> Name.ParseFile
    //|> Name.AddNamesToTable
    //|> printfn "Added %i names to table"

    //load title file
    // let titleUrl =
    //     "https://datasets.imdbws.com/title.basics.tsv.gz"

    // let compressedTitleFile =
    //     __SOURCE_DIRECTORY__
    //     + @"\Data\title-basics.tsv.gz"

    // Http.DownloadByteFile titleUrl compressedTitleFile
    // |> GZip.Decompress
    // |> Title.ParseFile
    // |> Title.AddTitlesToTable
    // |> printfn "Added %i titles to table"


    // //load principal file
    // let principalsUrl =
    //     "https://datasets.imdbws.com/title.principals.tsv.gz"

    // let compressedPrincipalsFile =
    //     __SOURCE_DIRECTORY__
    //     + @"\Data\title-principals.tsv.gz"

    // Http.DownloadByteFile principalsUrl compressedPrincipalsFile
    // |> GZip.Decompress
    // |> Principal.ParseFile
    // |> Principal.AddPrincipalsToTable
    // |> printfn "Added %i principals to table"

    0
