module PBEM.xTests.Board

open System.IO
open Xunit
open Utilities
open Board
open PBEM

[<Fact>]
let ``Create valid board test`` () =
    let cells = [
        CreateCell 1 1 Terrain.Flat [| 0 |];
        CreateCell 1 2 Terrain.Flat [| 0 |];
        CreateCell 2 1 Terrain.Woods [| 0 |];
        CreateCell 2 2 Terrain.Flat [| 0 |]
    ]
    let board = { RowCount = 2; ColCount = 2; Cells = cells; Off = List<Piece.T>.Empty; Dead = List<Piece.T>.Empty }
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
    let board = { RowCount = 2; ColCount = 2; Cells = cells; Off = List<Piece.T>.Empty; Dead = List<Piece.T>.Empty }
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
    let board = { RowCount = 2; ColCount = 2; Cells = cells; Off = List<Piece.T>.Empty; Dead = List<Piece.T>.Empty }
    let expected = false
    let actual = IsValidBoard board
    Assert.Equal (expected, actual)
    
[<Fact>]
let ``Create valid board from file test`` () =
    let pathAndFileName = PrependSourcePath "SampleMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.Equal (board.RowCount, 6)
    Assert.Equal (board.ColCount, 6)
    Assert.Equal (board.Cells.Length, 36)

[<Fact>]
let ``Provide invalid filename for creating board`` () =
    let pathAndFileName = PrependSourcePath "Map.txt"
    Assert.Throws<FileNotFoundException> (fun () -> CreateBoardFromFile pathAndFileName |> ignore)

[<Fact>]
let ``Use file without correct header for creating board`` () =
    let pathAndFileName = PrependSourcePath "BadMap.txt"
    Assert.Throws<InvalidDataException> (fun () -> CreateBoardFromFile pathAndFileName |> ignore)

[<Fact>]
let ``Use file with bad cell data for creating board`` () =
    let pathAndFileName = PrependSourcePath "BadParseMap.txt"
    Assert.Throws<InvalidDataException> (fun () -> CreateBoardFromFile pathAndFileName |> ignore)

[<Fact>]
let ``Show a valid road path is valid`` () =
    let pathAndFileName = PrependSourcePath "SampleMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.True (IsValidBoard board)
    Assert.True (IsValidAllRoads board)

[<Fact>]
let ``Show an invalid road path is invalid`` () =
    let pathAndFileName = PrependSourcePath "BadRoadMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.True (IsValidBoard board)
    Assert.False (IsValidAllRoads board)

[<Fact>]
let ``Map with four-way intersection is valid`` () =
    let pathAndFileName = PrependSourcePath "FourWayIntersection.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.True (IsValidBoard board)
    Assert.True (IsValidAllRoads board)

[<Fact>]
let ``Map with two roads is valid`` () =
    let pathAndFileName = PrependSourcePath "TwoRoadsMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.True (IsValidBoard board)
    Assert.True (IsValidAllRoads board)

[<Fact>]
let ``Map with two roads is invalid`` () =
    let pathAndFileName = PrependSourcePath "BadTwoRoadsMap.txt"
    let board = CreateBoardFromFile pathAndFileName
    Assert.True (IsValidBoard board)
    Assert.False (IsValidAllRoads board)
   
