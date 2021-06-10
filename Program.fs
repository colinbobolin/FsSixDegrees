open FSharp.Data
open System.IO
open System.IO.Compression
open System.Diagnostics

type ImportTableConfig =
    { SourceUrl: string
      TableName: string
      CreateTableSql: string }

let CopyTo path (stream: Stream) =
    File.Delete(path)
    let destination = File.OpenWrite(path)
    stream.CopyTo destination
    destination.Close()
    path

let DownloadByteFile url =
    let tempFile = Path.GetTempFileName()

    Http.RequestStream(url).ResponseStream
    |> CopyTo tempFile

let SeqToFile file (streamSeq: string seq) =
    File.Delete file
    File.WriteAllLines(file, streamSeq)

let RemoveUnacceptableCharacters (string: string) = string.Replace("\"", "'")

let ReadGZipFile stream =
    seq {
        use sr =
            new StreamReader(new GZipStream(stream, CompressionMode.Decompress))

        while not sr.EndOfStream do
            yield sr.ReadLine()
    }

let Decompress file =
    let fileinfo = FileInfo file
    let tempFile = Path.GetTempFileName()

    seq {
        use gz =
            new GZipStream(File.OpenRead(fileinfo.FullName), CompressionMode.Decompress)

        use sr = new StreamReader(gz)

        // discard the first row since it will be the headers
        sr.ReadLine() |> ignore

        while not sr.EndOfStream do
            yield sprintf "%s" (sr.ReadLine() |> RemoveUnacceptableCharacters)
    }
    |> SeqToFile tempFile

    tempFile

let CreateSqlScript (importFile: string) (createTableSql: string) (tableName: string) =
    let tempFile = Path.GetTempFileName()

    let importFileWithEscapedChars = importFile.Replace(@"\", @"\\")

    let sql =
        sprintf
            "
%s
.mode tabs
.import %s %s"
            createTableSql
            importFileWithEscapedChars
            tableName

    File.WriteAllText(tempFile, sql)

    tempFile

let ImportTsvToSqlite createTableSql sqliteCommandFile tableName fileToImport =
    let command =
        CreateSqlScript fileToImport createTableSql tableName
        |> sprintf "sqlite3.exe %s < %s" sqliteCommandFile

    let procStartInfo =
        new ProcessStartInfo(FileName = "CMD.exe", Arguments = @"/c" + command, UseShellExecute = true)

    Process.Start procStartInfo

[<EntryPoint>]
let main argv =
    if argv.Length > 0 then
        if argv.[0] = "--update" then
            let nameSql =
                "CREATE TABLE IF NOT EXISTS name ("
                + "nconst text PRIMARY KEY,"
                + "primaryName text,"
                + "birthYear text,"
                + "deathYear text,"
                + "primaryProfession text,"
                + "knownForTitles text"
                + ");"

            let titleSql =
                "CREATE TABLE IF NOT EXISTS title ("
                + "tconst text PRIMARY KEY,"
                + "titleType text,"
                + "primaryTitle text,"
                + "originalTitle text,"
                + "isAdult text,"
                + "startYear text,"
                + "endYear text,"
                + "runtimeMinutes text,"
                + "genres text"
                + ");"

            let principalSql =
                "CREATE TABLE IF NOT EXISTS principal ("
                + "tconst text,"
                + "ordering text,"
                + "nconst text,"
                + "category text,"
                + "job text,"
                + "characters text,"
                + "PRIMARY KEY (tconst, ordering, nconst),"
                + "FOREIGN KEY (tconst) REFERENCES title(tconst),"
                + "FOREIGN KEY (nconst) REFERENCES name(nconst)"
                + ");"

            let configs =
                [| { SourceUrl = "https://datasets.imdbws.com/name.basics.tsv.gz"
                     TableName = "name"
                     CreateTableSql = nameSql }
                   { SourceUrl = "https://datasets.imdbws.com/title.basics.tsv.gz"
                     TableName = "title"
                     CreateTableSql = titleSql }
                   { SourceUrl = "https://datasets.imdbws.com/title.principals.tsv.gz"
                     TableName = "principal"
                     CreateTableSql = principalSql } |]

            let dbFile =
                __SOURCE_DIRECTORY__ + @"\Data\imdb.sqlite"

            // Begin Execution

            File.Delete(dbFile)

            configs
            |> Array.iter
                (fun { SourceUrl = url
                       TableName = table
                       CreateTableSql = sql } ->
                    DownloadByteFile url
                    |> Decompress
                    |> ImportTsvToSqlite sql dbFile table
                    |> ignore)

    0
