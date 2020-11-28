module Utilities

open System.IO

let mutable savedPath = ""

let PrependSourcePath fileName =
    if savedPath <> "" then ()
    else 
        let mutable path = new DirectoryInfo (__SOURCE_DIRECTORY__)
        let mutable continueLooping = true
        while continueLooping && path.Name <> "play-by-email" && path.Name <> "PBEM" do
            if path.Name = "NCrunch" then
                path <- new DirectoryInfo (@"C:\Users\dalehu\source\repos\F\PBEM")
                continueLooping <- false
            else
                path <- path.Parent
        savedPath <- path.FullName
    Path.Combine (savedPath, fileName)
