module PBEM.xTests.Board

open Xunit
open Board
open System
open System.IO
open GamePieces

[<Fact>]
let ``Create valid board test`` () =
    let cells = [
        CreateCell 1 1 Terrain.Flat [| 0 |];
        CreateCell 1 2 Terrain.Flat [| 0 |];
        CreateCell 2 1 Terrain.Woods [| 0 |];
        CreateCell 2 2 Terrain.Flat [| 0 |]
    ]
    let board = { RowCount = 2; ColCount = 2; Cells = cells; Off = List<Piece option>.Empty; Dead = List<Piece option>.Empty }
    let expected = true
    let actual = IsValidBoard board
    Assert.Equal (expected, actual)

[<Fact>]
let ``Create invalid board test (missing cell)`` () =
    let cells = [
        CreateCell 1 1 Terrain.Flat [| 0 |];
        CreateCell 1 2 Terrain.Flat [| 0 |];
        CreateCell 2 2 Terrain.Flat [| 0 |]
    ]
    let board = { RowCount = 2; ColCount = 2; Cells = cells; Off = List<Piece option>.Empty; Dead = List<Piece option>.Empty }
    let expected = false
    let actual = IsValidBoard board
    Assert.Equal (expected, actual)

[<Fact>]
let ``Create invalid board test (cell duplicated)`` () =
    let cells = [
        CreateCell 1 1 Terrain.Flat [| 0 |];
        CreateCell 1 2 Terrain.Flat [| 0 |];
        CreateCell 2 2 Terrain.Flat [| 0 |];
        CreateCell 2 2 Terrain.Flat [| 0 |]
    ]
    let board = { RowCount = 2; ColCount = 2; Cells = cells; Off = List<Piece option>.Empty; Dead = List<Piece option>.Empty }
    let expected = false
    let actual = IsValidBoard board
    Assert.Equal (expected, actual)
    
[<Fact>]
let ``Create valid board from file test`` () =
    let pathAndFileName = @"C:\Users\dalehu\source\repos\F\PBEM\SampleMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.Equal (board.RowCount, 3)
    Assert.Equal (board.ColCount, 3)
    Assert.Equal (board.Cells.Length, 9)

[<Fact>]
let ``Provide invalid filename for creating board`` () =
    let pathAndFileName = @"C:\filename.txt"
    Assert.Throws<FileNotFoundException> (fun () -> CreateBoardFromFile pathAndFileName |> ignore)

[<Fact>]
let ``Use file without correct header for creating board`` () =
    let pathAndFileName = @"C:\Users\dalehu\source\repos\F\PBEM\BadMap.txt"
    Assert.Throws<InvalidDataException> (fun () -> CreateBoardFromFile pathAndFileName |> ignore)

[<Fact>]
let ``Use file with bad cell data for creating board`` () =
    let pathAndFileName = @"C:\Users\dalehu\source\repos\F\PBEM\BadParseMap.txt"
    Assert.Throws<InvalidDataException> (fun () -> CreateBoardFromFile pathAndFileName |> ignore)

[<Fact>]
let ``Show a valid road path is valid`` () =
    let pathAndFileName = @"C:\Users\dalehu\source\repos\F\PBEM\SampleMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    let mutable validRoadsFound = CreateBoolArrayOfGrid board
    Assert.True (IsValidRoad board 1 validRoadsFound)

[<Fact>]
let ``Show an invalid road path is invalid`` () =
    let pathAndFileName = @"C:\Users\dalehu\source\repos\F\PBEM\BadRoadMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    let mutable validRoadsFound = CreateBoolArrayOfGrid board
    Assert.False (IsValidRoad board 1 validRoadsFound)

[<Fact>]
let ``Map with four-way intersection is valid`` () =
    let pathAndFileName = @"C:\Users\dalehu\source\repos\F\PBEM\FourWayIntersection.txt"
    let board = CreateBoardFromFile pathAndFileName
    let mutable validRoadsFound = CreateBoolArrayOfGrid board
    Assert.True (IsValidBoard board)
    Assert.True (IsValidRoad board 1 validRoadsFound)

[<Fact>]
let ``Map with two roads is valid`` () =
    let pathAndFileName = @"C:\Users\dalehu\source\repos\F\PBEM\TwoRoadsMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.True (IsValidBoard board)
    Assert.True (IsValidAllRoads board)

[<Fact>]
let ``Map with two roads is invalid`` () =
    let pathAndFileName = @"C:\Users\dalehu\source\repos\F\PBEM\BadTwoRoadsMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.True (IsValidBoard board)
    Assert.False (IsValidAllRoads board)
   
