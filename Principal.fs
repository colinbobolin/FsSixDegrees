namespace SixDegrees

module Principal =
    open System.Data.SQLite
    open System
    open System.IO

    type Principal =
        { Tconst: string
          Ordering: string
          Nconst: string
          Category: string
          Job: string
          Characters: DateTime }

    let fromString (row: string) =
        let rowParts = row.Split "	"
        let tconst = rowParts.[0]
        let ordering = rowParts.[1]
        let nconst = rowParts.[2]
        let category = rowParts.[3]
        let job = rowParts.[4]
        let characters = rowParts.[5] |> DateTime.Parse

        { Tconst = tconst
          Ordering = ordering
          Nconst = nconst
          Category = category
          Job = job
          Characters = characters }

    let ParseFile file =
        File.ReadAllLines file
        |> Array.skip 1
        |> Array.map (fun row -> fromString row)

    let AddPrincipalsToTable titles =
        let connectionString =
            "Data Source="
            + __SOURCE_DIRECTORY__
            + @"\Data\imdb.db;Version=3;"

        use connection = new SQLiteConnection(connectionString)

        connection.Open()

        let createTable =
            "CREATE TABLE principal ("
            + "tconst text,"
            + "ordering text,"
            + "nconst text,"
            + "category text,"
            + "job text,"
            + "characters text"
            + ");"

        let createCommand =
            new SQLiteCommand(createTable, connection)

        createCommand.ExecuteNonQuery()
        |> printfn "Created table: %i"

        let insertSql =
            "INSERT INTO principal(tconst, ordering, nconst, category, job, characters) "
            + "VALUES (@tconst, @ordering, @nconst, @category, @job, @characters)"

        titles
        |> Array.map
            (fun title ->
                use command = new SQLiteCommand(insertSql, connection)

                command.Parameters.AddWithValue("@tconst", title.Tconst)
                |> ignore

                command.Parameters.AddWithValue("@ordering", title.Ordering)
                |> ignore

                command.Parameters.AddWithValue("@nconst", title.Nconst)
                |> ignore

                command.Parameters.AddWithValue("@category", title.Category)
                |> ignore

                command.Parameters.AddWithValue("@job", title.Job)
                |> ignore

                command.Parameters.AddWithValue("@characters", title.Characters)
                |> ignore

                command.ExecuteNonQuery())
        |> Array.sum
