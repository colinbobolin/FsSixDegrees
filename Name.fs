namespace SixDegrees

module Name =

    open System
    open System.IO
    open System.Data.SQLite
    open SixDegrees

    type Name =
        { Nconst: string
          PrimaryName: string
          BirthYear: string
          DeathYear: string
          PrimaryProfession: string
          KnownForTitles: string }

    let fromString (row: string) =
        let rowParts = row.Split "	"

        let nconst = rowParts.[0]
        let primaryName = rowParts.[1]
        let birthYear = rowParts.[2]
        let deathYear = rowParts.[3]
        let primaryProfession = rowParts.[4]
        let knownForTitles = rowParts.[5]

        { Nconst = nconst
          PrimaryName = primaryName
          BirthYear = birthYear
          DeathYear = deathYear
          PrimaryProfession = primaryProfession
          KnownForTitles = knownForTitles }

    let ParseFile file =
        File.ReadAllLines file
        |> Array.skip 1
        |> Array.map (fun row -> fromString row)

    let AddNamesToTable names =
        let connectionString =
            "Data Source="
            + __SOURCE_DIRECTORY__
            + @"\Data\imdb.db;Version=3;"

        use connection = new SQLiteConnection(connectionString)

        SQLiteConnection.CreateFile(@"Data\imdb.db")

        connection.Open()

        let createTable =
            "CREATE TABLE name ("
            + "nconst text,"
            + "primaryName text,"
            + "birthYear text,"
            + "deathYear text,"
            + "primaryProfession text,"
            + "knownForTitles text"
            + ");"

        let createCommand =
            new SQLiteCommand(createTable, connection)

        createCommand.ExecuteNonQuery()
        |> printfn "Created table: %i"

        let insertSql =
            "INSERT INTO name(nconst, primaryName, birthYear, deathYear, primaryProfession, knownForTitles) "
            + "VALUES (@nconst, @primaryName, @birthYear, @deathYear, @primaryProfession, @knownForTitles)"

        names
        |> Array.map
            (fun name ->
                use command = new SQLiteCommand(insertSql, connection)

                command.Parameters.AddWithValue("@nconst", name.Nconst)
                |> ignore

                command.Parameters.AddWithValue("@primaryName", name.PrimaryName)
                |> ignore

                command.Parameters.AddWithValue("@birthYear", name.BirthYear)
                |> ignore

                command.Parameters.AddWithValue("@deathYear", name.DeathYear)
                |> ignore

                command.Parameters.AddWithValue("@primaryProfession", name.PrimaryProfession)
                |> ignore

                command.Parameters.AddWithValue("@knownForTitles", name.KnownForTitles)
                |> ignore

                command.ExecuteNonQuery())
        |> Array.sum
