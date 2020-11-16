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

// ======================================== CALCULATION FUNCTIONS ========================================

(*
    Calculate a cell location into a single dimensional array based on the number of columns on the board,
    the cell's row, and the cell's column numbers.
*)
let CalculateCellNumber numColumns rowNum colNum =
    ((rowNum - 1) * numColumns) + (colNum - 1)

(*

*)
let CalculateRowColFromNumber numColumns cellNumber =
    let numRows = cellNumber / numColumns
    let row = numRows + 1
    let col = cellNumber - (numRows * numColumns) + 1
    (row, col)

(*

*)
let IsValidRowCol board row col =
    if ((row < 1 || row >= board.RowCount) || (col < 1 || col >= board.ColCount)) then false
    else true

(*

*)
let GetCell board row col =
    let cells = board.Cells |> List.filter (fun c -> c.Row = row && c.Col = col)
    match cells.Length with
    | 0 -> None
    | _ -> Some cells.[0]

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
            let cellNum = CalculateCellNumber board.ColCount c.Row c.Col
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

let CreateBoardFromDefinition (boardDefinition : string[]) =
    let mutable cells : List<Cell> = List<Cell>.Empty
    let mutable board = { RowCount = 0; ColCount = 0; Cells = cells }
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

let CreateBoardFromFile fileNameAndPath =
    try
        let boardDefinition = IO.File.ReadAllLines fileNameAndPath
        CreateBoardFromDefinition boardDefinition
    with
        | :? FileNotFoundException as fnfex -> raise fnfex
        | :? InvalidDataException as idex -> raise idex
        | _ as ex -> raise ex

let rec IsValidRoad (board : Board) startFromCellNum (validatedCells : bool[]) =
    // Is the current cell already validated?
    match validatedCells.[startFromCellNum] with
    | true -> true
    | false ->
        // Get the row and column of the cell, then get the cell itself.
        let (row, col) = CalculateRowColFromNumber board.ColCount startFromCellNum
        let cell = GetCell board row col
        match cell with
        | None -> false
        | Some c ->
            // Is there a road in this cell?
            match c.Road with
            | None -> false
            | Some r ->
                let first = c.Road.Value.From
                let last = c.Road.Value.To

                printfn "Cell %i, %i road from %i to %i" row col first last

                Array.set validatedCells startFromCellNum true
                let mutable fromDest = (row, col)
                let mutable toDest = (row, col)
                match first with
                | 1 -> fromDest <- (row - 1, col)
                | 2 -> fromDest <- (row - 1, col + 1)
                | 3 -> fromDest <- (row, col + 1)
                | 4 -> fromDest <- (row + 1, col + 1)
                | 5 -> fromDest <- (row + 1, col)
                | 6 -> fromDest <- (row + 1, col - 1)
                | 7 -> fromDest <- (row, col - 1)
                | _ -> fromDest <- (row - 1, col - 1)
                match last with
                | 1 -> toDest <- (row - 1, col)
                | 2 -> toDest <- (row - 1, col + 1)
                | 3 -> toDest <- (row, col + 1)
                | 4 -> toDest <- (row + 1, col + 1)
                | 5 -> toDest <- (row + 1, col)
                | 6 -> toDest <- (row + 1, col - 1)
                | 7 -> toDest <- (row, col - 1)
                | _ -> toDest <- (row - 1, col - 1)
                let fromRow, fromCol = fromDest
                let toRow, toCol = toDest
                let validFrom = IsValidRowCol board fromRow fromCol
                let validTo = IsValidRowCol board toRow toCol
                match validFrom with
                | true -> 
                    (IsValidRoad board (CalculateCellNumber board.ColCount fromRow fromCol) validatedCells) &&
                    (match validTo with
                    | true -> IsValidRoad board (CalculateCellNumber board.ColCount toRow toCol) validatedCells
                    | false -> true)
                | false -> true

(*
let IsValidRoadNet (board : Board) =
    let cellCount = board.RowCount * board.ColCount
    // Create an array of bool representing the cells in the board.
    let mutable roadCells : bool array = Array.zeroCreate cellCount
    let mutable validRoadCells : bool array = Array.zeroCreate cellCount
    // Loop through all the cells in the board and see where the roads are.
    for (c : Cell) in board.Cells do
        let cellNum = CalculateCellNumber board.ColCount c.Row c.Col
        match c.Road with
        | Some road -> roadCells.[cellNum] <- true
        | None -> ()
    match (Array.contains true roadCells) with
    | true ->
        // Loop through the roadCells and mark validRoadCells as valid or invalid.
        for bc in roadCells do
            match bc with
            | true ->
                IsValidRoad board 
            | false -> ()
    | false -> true
*)
