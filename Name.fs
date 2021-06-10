namespace SixDegrees

module Sql =
    open System.IO

    let CreateSqlScript (importFile: string) (createTableSql: string) =
        let tempFile = Path.GetTempFileName()

        // let sqlFile =
        //     __SOURCE_DIRECTORY__ + @"\Scripts\importNames.sql"

        let importFileWithEscapedChars = importFile.Replace(@"\", @"\\")

        let sql =
            sprintf
                "
%s
.mode tabs
.import %s name"
                createTableSql
                importFileWithEscapedChars

        File.WriteAllText(tempFile, sql)

        tempFile


module Name =

    open System.Diagnostics

    let ExecuteImportCommand =
        let namesFile =
            __SOURCE_DIRECTORY__ + @"\Data\name-basics.tsv"

        let sqliteFile =
            __SOURCE_DIRECTORY__ + @"\Data\imdb.sqlite"

        let createTableSql = "
CREATE TABLE IF NOT EXISTS name (
    nconst text,
    primaryName text,
    birthYear text,
    deathYear text,
    primaryProfession text,
    knownForTitles text
);"

        let command =
            Sql.CreateSqlScript namesFile createTableSql
            |> sprintf "sqlite3.exe %s < %s" sqliteFile

        let procStartInfo =
            new ProcessStartInfo(FileName = "CMD.exe", Arguments = @"/c" + command)

        Process.Start procStartInfo
