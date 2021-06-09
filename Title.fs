namespace SixDegrees

open System.Data.SQLite

module Title =

    open System
    open System.IO

    type Title =
        { Tconst: string
          TitleType: string
          PrimaryTitle: string
          OriginalTitle: string
          IsAdult: string
          StartYear: string
          EndYear: string
          RuntimeMinutes: string
          Genres: string }

    let fromString (row: string) =
        let rowParts = row.Split "	"
        let tconst = rowParts.[0]
        let titleType = rowParts.[1]
        let primaryTitle = rowParts.[2]
        let originalTitle = rowParts.[3]
        let isAdult = rowParts.[4]
        let startYear = rowParts.[5]
        let endYear = rowParts.[6]
        let runtimeMinutes = rowParts.[7]
        let genres = rowParts.[8]

        { Tconst = tconst
          TitleType = titleType
          PrimaryTitle = primaryTitle
          OriginalTitle = originalTitle
          IsAdult = isAdult
          StartYear = startYear
          EndYear = endYear
          RuntimeMinutes = runtimeMinutes
          Genres = genres }

    let ParseFile file =
        File.ReadAllLines file
        |> Array.skip 1
        |> Array.map (fun row -> fromString row)

    let AddTitlesToTable titles =
        let connectionString =
            "Data Source="
            + __SOURCE_DIRECTORY__
            + @"\Data\imdb.db;Version=3;"

        use connection = new SQLiteConnection(connectionString)

        connection.Open()

        let createTable =
            "CREATE TABLE title ("
            + "tconst text,"
            + "titleType text,"
            + "primaryTitle text,"
            + "originalTitle text,"
            + "isAdult text,"
            + "startYear text,"
            + "endYear text,"
            + "runtimeMinutes text,"
            + "genres text"
            + ");"

        let createCommand =
            new SQLiteCommand(createTable, connection)

        createCommand.ExecuteNonQuery()
        |> printfn "Created table: %i"

        let insertSql =
            "INSERT INTO title(tconst, titleType, primaryTitle, originalTitle, isAdult, startYear, endYear, runtimeMinutes, genres) "
            + "VALUES (@tconst, @titleType, @primaryTitle, @originalTitle, @isAdult, @startYear, @endYear, @runtimeMinutes, @genres)"

        titles
        |> Array.map
            (fun title ->
                use command = new SQLiteCommand(insertSql, connection)

                command.Parameters.AddWithValue("@tconst", title.Tconst)
                |> ignore

                command.Parameters.AddWithValue("@titleType", title.TitleType)
                |> ignore

                command.Parameters.AddWithValue("@primaryTitle", title.PrimaryTitle)
                |> ignore

                command.Parameters.AddWithValue("@originalTitle", title.OriginalTitle)
                |> ignore

                command.Parameters.AddWithValue("@isAdult", title.IsAdult)
                |> ignore

                command.Parameters.AddWithValue("@startYear", title.StartYear)
                |> ignore

                command.Parameters.AddWithValue("@endYear", title.EndYear)
                |> ignore

                command.Parameters.AddWithValue("@runtimeMinutes", title.RuntimeMinutes)
                |> ignore

                command.Parameters.AddWithValue("@genres", title.Genres)
                |> ignore

                command.ExecuteNonQuery())
        |> Array.sum
