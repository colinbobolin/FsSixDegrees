open SixDegrees

[<EntryPoint>]
let main argv =
    if argv.Length > 0 then
        if argv.[0] = "--update" then
            Update.UpdateDatabase

    0
