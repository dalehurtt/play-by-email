module Board

open System
open GamePieces
open System.Data
open System.IO
open System.IO

// ======================================== DATA TYPES ========================================

(*
    A Road references the two ends. The From is always the lower number, while the To is the larger. 
    Further, the two values cannot be the same. A value of '1' points due North, counting clockwise with 
    '8' equalling Northwest.
    Example: From = 1 and To = 5 is a road from North to South.
*)
type Road = {
    From : int
    To: int
}

(*
    Terrain is an enumeration to represent the terrain in the grid. Note that this can easily be expanded
    by adding new values to the list, i.e. High Hills, Wooded Hills, Muddy Ground, etc.
*)
type Terrain =
    | Flat = 0
    | Woods = 1
    | River = 2
    | Hills = 3
    | Town = 4
    | Swamp = 5     // blocks line of sight
    | Marsh = 6     // does not block line of sight
    | Bridge = 7
    | Ford = 8
    
type Cell = {
    Row : int
    Col : int
    Terrain : Terrain
    Road : Road option
    Piece : Piece option
}

type Board = {
    RowCount : int
    ColCount : int
    Cells : List<Cell>
}

// ======================================== VALIDATION FUNCTIONS ========================================

(*
    Validate that an int falls within acceptable boundaries.
    mi : The minimum allowable integer value.
    ma : The maximum allowable integer value.
    n  : The value to validate.
    NOTE this will throw an ArgumentOutOfRange exception if the value is out of the range indicated.
*)
let ValidateInt mi ma n =
    match n with
    | n when n >= mi && n <= ma -> n
    | _ -> raise (ArgumentOutOfRangeException (sprintf "%i out of bounds %i..%i" n mi ma))

let IsValidBoard (board : Board) =
    let cellCount = board.RowCount * board.ColCount
    let mutable cellsFound : bool array = Array.zeroCreate cellCount
    match cellCount = board.Cells.Length with
    | false -> false
    | true ->
        for (c : Cell) in board.Cells do
            let cellNum = ((c.Row - 1) * board.ColCount) + (c.Col - 1)
            cellsFound.[cellNum] <- true
        not (Array.contains false cellsFound)

// ======================================== CONSTRUCTOR FUNCTIONS ========================================

(*
    Create a Road option type. See Road type definition for more information. Note that the From value will
    always be the lower of the two. The two arguments will always be validated and None will be returned if
    either argument is invalid or both values are the same. 
    NOTE that passing 0 0 is the correct way to indicate that there is no road.
    a : One of the From/To values.
    b : The other of the From/To values.
*)
let CreateRoad a b =
    match a = b with
    | true -> None
    | _ -> 
        try
            Some { From = ValidateInt 1 8 (min a b); To = ValidateInt 1 8 (max a b) }
        with
            | ex -> 
                match a with
                | 0 -> None
                | _ -> printfn "Removing road %i %i because the values are %s" a b (ex.Message); None

(*
    Create a Cell type.
    row : The row number of the cell.
    col : The column number of the cell.
    terrain : The type of terrain in the cell.
    (roadFrom : The direction the road starts from. See type Road for more details.
    roadTo) : The direction the road goes toward. This is part of a tuple merely to logically group data.
*)
let CreateCell row col terrain (roadFrom, roadTo) =
    { 
        Row = row
        Col = col
        Terrain = terrain
        Road = (CreateRoad roadFrom roadTo)
        Piece = None
    }


let AddPieceToCell piece cell =
    { cell with Piece = piece }

let RemovePieceFromCell piece cell =
    { cell with Piece = None }

let CreateBoardFromFile fileNameAndPath =
    let mutable cells : List<Cell> = List<Cell>.Empty
    let mutable board = { RowCount = 0; ColCount = 0; Cells = cells }
    try
        let boardDefinition = IO.File.ReadAllLines fileNameAndPath
        // The first character of the first line must be B
        match boardDefinition.[0].[0] with
        | 'B' ->
            for line in boardDefinition do
                let parts = line.Split ([|'`'|])
                match parts.[0] with
                | "B" ->
                    board <- { board with RowCount = (int parts.[1]); ColCount = (int parts.[2]) }
                | "C" ->
                    cells <- cells @ [(CreateCell (int parts.[1]) (int parts.[2]) (enum<Terrain> (int parts.[3])) ((int parts.[4]), (int parts.[5])))]
                    board <- { board with Cells = cells }
                | _ -> raise (new InvalidDataException (sprintf "ERROR: Parsing failure on line %s" line))
            if IsValidBoard board then board
            else failwith "Board is invalid"
        | _ -> raise (new InvalidDataException ("ERROR: Invalid header information"))
    with
        | :? FileNotFoundException as fnfex -> raise fnfex
        | :? InvalidDataException as idex -> raise idex
        | _ as ex -> raise ex


// ======================================== EXAMPLE CALLS ========================================
(*
let road1 = CreateRoad 1 8      // success
let road2 = CreateRoad 1 9      // error
let road3 = CreateRoad 7 2      // success with swapped values
let cell11 = CreateCell 1 1 Terrain.Flat (5, 1)  // Square 1,1 with Flat terrain and a road from North to South.
let cell12 = CreateCell 1 2 Terrain.Flat (0, 0)  // Square 1,2 with Flat terrain and no road.
*)