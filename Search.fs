namespace SixDegrees

module Search =
    open System.Data.SQLite
    open Dapper

    type Name = {
        nconst: string
        primaryName: string
    }

    type Title = {
        tconst: string
        primaryTitle: string
    }

    type Searchable =
    | Name of Name
    | Title of Title

    let GetNamesForTitle (title:Title) =
        let connectionString = "Data Source=" + __SOURCE_DIRECTORY__ + @"\Data\imdb.sqlite;Version=3;"
        let connection = new SQLiteConnection(connectionString)
        connection.Open()
        let sql = 
            "SELECT name.nconst, "
            + "name.primaryName "
            + "FROM name "
            + "JOIN principal "
            + "ON name.nconst = principal.nconst "
            + "WHERE principal.tconst = @tconst"

        // TODO async
        connection.Query<Name>(sql, title) //might need to extract title.tconst
        |> seq

    let GetTitlesForName (name:Name) =
        let connectionString = "Data Source=" + __SOURCE_DIRECTORY__ + @"\Data\imdb.sqlite;Version=3;"
        let connection = new SQLiteConnection(connectionString)
        connection.Open()
        let sql = 
            "SELECT title.tconst, "
            + "title.primaryTitle "
            + "FROM title "
            + "JOIN principal "
            + "ON title.tconst = principal.tconst "
            + "WHERE principal.nconst = @nconst"

        // TODO async
        connection.Query<Title>(sql, name) //might need to extract name.nconst
        |> seq

    let GetAssociatedNames name =
        let sql = "SELECT 
    n1.nconst,
    n1.primaryName,
    p1.category,
    p1.characters,
    t1.tconst,
    t1.primaryTitle,
    t1.titleType,
    n2.nconst,
    n2.primaryName,
    p2.category,
    p2.characters
FROM name n1
JOIN principal p1
    ON n1.nconst = p1.nconst
JOIN title t1
    ON p1.tconst = t1.tconst
JOIN principal p2
    ON t1.tconst = p2.tconst
JOIN name n2
    ON p2.nconst = n2.nconst
WHERE n1.nconst = 'nm1024677'
    AND n1.nconst <> n2.nconst;"

    let SearchName name target =
        GetTitlesForName name
        

    let Search (name:Name) (target:Name)=
        let titles = GetTitlesForName name
        // get names for titles

    let BreadthFirstSearch start target =
        Search start