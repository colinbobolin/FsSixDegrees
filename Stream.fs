namespace SixDegrees

module Seq =
    open System.IO

    let toFile file (streamSeq: string seq) =
        File.Delete file
        File.WriteAllLines(file, streamSeq)

module Stream =
    open System.IO
    open System.IO.Compression

    let readGZipFile stream =
        seq {
            use sr =
                new StreamReader(new GZipStream(stream, CompressionMode.Decompress))

            while not sr.EndOfStream do
                yield sr.ReadLine()
        }

    let CopyTo path (stream: Stream) =
        File.Delete(path)
        let destination = File.OpenWrite(path)
        stream.CopyTo destination
        destination.Close()
        path

module Http =
    open FSharp.Data

    let DownloadByteFile url path =
        Http.RequestStream(url).ResponseStream
        |> Stream.CopyTo path

module GZip =
    open System.IO.Compression
    open System.IO

    let GetNewFileName (filename: string) = filename.Replace(".gz", "")

    let Decompress file =
        let fileinfo = FileInfo file
        let newFileName = GetNewFileName fileinfo.FullName

        seq {
            use gz =
                new GZipStream(File.OpenRead(fileinfo.FullName), CompressionMode.Decompress)

            use sr = new StreamReader(gz)

            while not sr.EndOfStream do
                yield sprintf "%s" (sr.ReadLine())
        }
        |> Seq.toFile newFileName

        newFileName
