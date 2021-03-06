﻿module Board

open System
open System.IO
open Pure
open PBEM

// ======================================== DATA TYPES ========================================

(*
    Terrain is an enumeration to represent the terrain in the grid. Note that this can easily be expanded
    by adding new values to the list, i.e. High Hills, Wooded Hills, Muddy Ground, etc.
*)
// #NO_DEPENDENCY
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
    
(*
    A Road references a minimum of two ends and a maximum of 8. The numbers are always ordered from
    lowest to highest with no duplicates. A value of '1' points due North, counting clockwise with 
    '8' equalling Northwest.

    Example: [| 1; 5 |] is a road from North to South.
*)
// #NO_DEPENDENCY
type Road = int []

(*
    A Cell represent space on the Board, i.e. a square on a square-gridded board or a hex on a hex-
    gridded board. It contains the definition of the space, which includes the row and column 
    reference, the terrain present, the road present (if any), and the game piece present (if any).
*)
type Cell = {
    Row : int
    Col : int
    Terrain : Terrain
    Road : int[] option
    Piece : Piece.T option
}

(*
    The Board represents the playing surface of the game. It has a reference to the number of rows 
    and columns and a list of all of the Cells that comprise the board, i.e. the squares in a 
    square-gridded game or the hexes in a hex-gridded game.

    TODO: Change the Off and Dead lists from List<Piece option> to List<Piece>. These lists should
    always contain only pieces and never have None.
*)
type Board = {
    RowCount : int
    ColCount : int
    Cells : List<Cell>
    Off : List<Piece.T>
    Dead : List<Piece.T>
}

// ======================================== CALCULATION FUNCTIONS ========================================

(*
    Calculate a cell location into a single dimensional array based on the number of columns on the board,
    the cell's row, and the cell's column numbers.

    For example for a 3 x 3 grid: row 1, column 1 would return 0; row 2, column 2 would return 4; and row
    3, column 3 would return 8.
*)
// #NO_DEPENDENCY
let CalculateCellNumber numColumns rowNum colNum =
    ((rowNum - 1) * numColumns) + (colNum - 1)

(*
    Calculate the row and column values of a cell given the number of the cell in a single-dimension array.

    For example, for a 3 x 3 grid a cellNumber of: 0 would be row 1, column 1; 4 would be row 2, col 2; and 
    8 would be row 3, column 3.
*)
// #NO_DEPENDENCY
let CalculateRowColFromNumber numColumns cellNumber =
    let numRows = cellNumber / numColumns
    let row = numRows + 1
    let col = cellNumber - (numRows * numColumns) + 1
    (row, col)

(*
    Return a single dimension boolean array the size of the board's grid, initialized to false.
    You can use these arrays to "check off" locations in the grid that have roads, have been checked,
    etc.
*)
// #NO_DEPENDENCY
let CreateBoolArrayOfGrid rows cols =
    Array.zeroCreate (rows * cols) : bool[]

(*

*)
// #NO_DEPENDENCY
let StringArrayToIntArray strings =
    Array.map (fun str -> int str) strings

// ======================================== FORWARD FUNCTIONS ========================================

(*
    Returns whether a grid location (indicated by row and column) is a valid location on the board.
*)
// #NO_DEPENDENCY
let IsValidRowCol rowCount colCount row col =
    // Is the row or col is 0 or the row or col is outside of the row and column count?
    if (row < 1 || row > rowCount || col < 1 || col > colCount) then false
    else true

(*
    Returns a cell on the board given the row and column. Note that this relies on the fact that when 
    the board was originally created, the cells were all validated as being present and none were
    duplicated. Given that, this function searches through the list of cells to find the first cell
    that matches.
*)
let GetCell cells row col =
    let matches = cells |> List.filter (fun c -> c.Row = row && c.Col = col)
    match matches.Length with
    | 0 -> None
    | _ -> Some matches.[0]

(*
    Replaces a cell in the list of cells provided.
*)
let ReplaceCell oldCells newCell =
    let (cells : List<Cell>) = oldCells |> List.choose (fun c ->
        match c.Row = newCell.Row && c.Col = newCell.Col with
        | true -> Some newCell
        | false -> Some c
    )
    cells

// ======================================== ROAD FUNCTIONS ========================================

(*
    Validate that an int falls within acceptable boundaries.
        mi : The minimum allowable integer value.
        ma : The maximum allowable integer value.
        n  : The value to validate.
    NOTE this will throw an ArgumentOutOfRange exception if the value is out of the range indicated.
*)
// #NO_DEPENDENCY
let ValidateInt mi ma n =
    match n with
    | n when n >= mi && n <= ma -> n
    | _ -> raise (ArgumentOutOfRangeException (sprintf "%i out of bounds %i..%i" n mi ma))

(*
    Return a Road option type. See Road type definition for more information. Note that the From value 
    will always be the lower of the two. The two arguments will always be validated and None will be 
    returned if either argument is out of range or both values are the same. 
    NOTE that passing 0 0 is the correct way to indicate that there is no road.
        a : One of the From/To values.
        b : The other of the From/To values.
*)
let CreateRoad (a : int []) =
    match a.[0] = 0 with
    | true -> None
    | false ->
        try
            let points : int[] = Array.sort a |> Seq.distinct |> Seq.toArray |> Array.map (fun elem -> ValidateInt 1 8 elem)
            Some points
        with
            | _ as ex -> None

(*
    Recursive function that validate whether a road is valid. It does this by starting at a cell
    number, determining if that cell has a road, determines the cell it is coming from and the one it 
    is going to, and determining if those cells also have valid roads. If all cells within the 
    board's boundaries attached to that road chain are valid, the function returns true.

    Note that roads pointing off-board are still valid.
*)
let rec IsValidRoad board startFromCellNum (validatedCells : bool[]) =
    // Is the current cell already validated?
    match validatedCells.[startFromCellNum] with
    | true -> true
    | false ->
        // Get the row and column of the cell, then get the cell itself.
        let (row, col) = CalculateRowColFromNumber board.ColCount startFromCellNum
        let cell = GetCell board.Cells row col
        match cell with
        | None -> false     // I don't know how we would ever get here!
        | Some c ->
            // Is there a road in this cell?
            match c.Road with
            | None -> false
            | Some roadArray ->
                Array.set validatedCells startFromCellNum true
                Array.fold (fun result roadDirection ->
                    (
                        let mutable destCell = (row, col)
                        match roadDirection with
                        | 1 -> destCell <- (row - 1, col)
                        | 2 -> destCell <- (row - 1, col + 1)
                        | 3 -> destCell <- (row, col + 1)
                        | 4 -> destCell <- (row + 1, col + 1)
                        | 5 -> destCell <- (row + 1, col)
                        | 6 -> destCell <- (row + 1, col - 1)
                        | 7 -> destCell <- (row, col - 1)
                        | _ -> destCell <- (row - 1, col - 1)

                        let toRow, toCol = destCell
                        let validTo = IsValidRowCol board.RowCount board.ColCount toRow toCol

                        match validTo with
                        | true -> 
                            IsValidRoad board (CalculateCellNumber board.ColCount toRow toCol) validatedCells
                        | false -> true
                    ) && result
                ) true roadArray

(*
    Function that checks the entire board for roads, ensuring that they are all valid.
*)
let IsValidAllRoads board =
    let mutable validRoadsFound = CreateBoolArrayOfGrid board.RowCount board.ColCount
    List.fold (fun result cell ->
        (
            match cell.Road with
            // Note it may seem wrong to mark a cell with no Road as true. But this is because there 
            // is no road to check AND we are NOT marking the cell in the validRoadsFound array as
            // true.
            | None -> true
            | Some roadArray ->
                IsValidRoad board (CalculateCellNumber board.ColCount cell.Row cell.Col) validRoadsFound
        ) && result
    ) true board.Cells


// ======================================== PIECE FUNCTIONS ========================================

(*
    Return a cell with the indicated piece added to it.

    TODO: You cannot add the piece if there is already a piece there.
*)
let AddPieceToCell piece cell =
    { cell with Piece = Some piece }

(*
    Return a cell after removing a piece from it.

    TODO: You cannot remove a piece if none is there.
    TODO: Is the piece being passed the piece that is actually in the cell?
*)
let RemovePieceFromCell piece cell =
    { cell with Piece = None }

(*
    Moves a game piece from one cell to another on the board.
*)
let MoveGamePiece (piece : Piece.T) (fromRow, fromCol) (toRow, toCol) board =
    let fromCell =
        match fromRow + fromCol with
        | 0 -> None
        | _ -> GetCell board.Cells fromRow fromCol
    let toCell = GetCell board.Cells toRow toCol
    let fromBoard =
        match fromCell with
        | None -> board
        | Some fCell ->
            { board with Cells = ReplaceCell board.Cells (RemovePieceFromCell piece fCell) }
    let toBoard =
        match toCell with
        | None ->
            let newOff = piece :: fromBoard.Off
            { fromBoard with Off = newOff }
        | Some toCell ->
            { fromBoard with Cells = ReplaceCell fromBoard.Cells (AddPieceToCell piece toCell) }
    toBoard

// ======================================== CELL FUNCTIONS ========================================

(*
    Return a Cell type.
        row : The row number of the cell.
        col : The column number of the cell.
        terrain : The type of terrain in the cell.
        roads : The directions the road points to. See type Road for more details.
    *)
let CreateCell row col terrain roads =
    { 
        Row = row
        Col = col
        Terrain = terrain
        Road = (CreateRoad roads)
        Piece = None
    }

// ======================================== BOARD FUNCTIONS ========================================

(*
    Check the validity of the board definition. A board is not valid if every slot of the grid has a
    cell assigned and none of the cells are duplicated.

    Note that the duplication of cells is not explicitly checked for. Because the number of cells must
    match the row count times the column count, and each cell position is checked off, either these 
    checks would fail because there are more cells (one or more of them being duplicates) or there 
    would be gaps in the grid (two or more cells pointing to the same location).

    TODO: Replace for loop with list processing functions.
    TODO: Use CreateBoolArrayOfGrid function.
*)
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

(*
    Return a Board given an array of encoded strings that defines the map. See separate documentation 
    on the expected format of those strings.

    TODO: Change the format of the definition to binary to reduce its size and processing time.
    TODO: Replace for loop with list processing functions.
*)
let CreateBoardFromDefinition (boardDefinition : string[]) =
    let mutable cells : List<Cell> = List<Cell>.Empty
    let mutable board = { RowCount = 0; ColCount = 0; Cells = cells; Off = List<Piece.T>.Empty; Dead = List<Piece.T>.Empty }
    // The first character of the first line must be B
    match boardDefinition.[0].[0] with
    | 'B' ->
        Array.iter (fun (line : string) ->
            let parts = line.Split ([|'`'|])
            match parts.[0] with
            | "B" ->
                board <- { board with RowCount = (int parts.[1]); ColCount = (int parts.[2]) }
            | "C" ->
                cells <- cells @ [(CreateCell (int parts.[1]) (int parts.[2]) (enum<Terrain> (int parts.[3])) (StringArrayToIntArray (parts.[4..(parts.Length - 1)])))]
                board <- { board with Cells = cells }
            | _ -> raise (new InvalidDataException (sprintf "ERROR: Parsing failure on line %s" line))
        ) boardDefinition
        if IsValidBoard board then board
        else failwith "Board is invalid"
    | _ -> raise (new InvalidDataException ("ERROR: Invalid header information"))

(*
    Return a Board given a file containing multiple lines of encoded strings defining the map. See
    separate documentation on the expected format of those strings.

    This is a wrapper of the CreateBoardFromDefinition function, simply extracting the strings from a 
    file, then calling that function.

    TODO: Change the format of the definition to binary to reduce the file size.
*)
let CreateBoardFromFile fileNameAndPath =
    try
        let boardDefinition = IO.File.ReadAllLines fileNameAndPath
        CreateBoardFromDefinition boardDefinition
    with
        | :? FileNotFoundException as fnfex -> raise fnfex
        | :? InvalidDataException as idex -> raise idex
        | _ as ex -> raise ex

(*
    Visualize the map of the game board. This draws a grid to the console with the terrain, roads,
    and game pieces displayed.
*)
let VisualizeMap board =
    let bgColor = Console.BackgroundColor
    // Loop through all of the rows
    for i in 1..board.RowCount do
        // Draw the top border of the cell
        for _ in 1..board.ColCount do
            printf "+--------"
        printfn "+"
        // Draw the terrain in the cell
        for j in 1..board.ColCount do
            let cell = GetCell board.Cells i j
            match cell with
            | Some c ->
                match c.Terrain with
                | Terrain.Flat -> printf "|        "
                | Terrain.Hills ->
                    printf "|"
                    Console.BackgroundColor <- ConsoleColor.DarkYellow
                    printf "  HILL  "
                    Console.BackgroundColor <- bgColor
                | _ -> printf "|       "
            | None ->
                Console.BackgroundColor <- ConsoleColor.Red
                printfn "| ERR  "
                Console.BackgroundColor <- bgColor
        printfn "|"
        // Draw the roads in the cell
        for j in 1..board.ColCount do
            let cell = GetCell board.Cells i j
            match cell with
            | Some c ->
                let roads = c.Road
                if roads.IsSome  && roads.Value.Length > 0 then
                    printf "| %6s " (Array.fold (fun acc ele -> acc + (FacingToString ele)) "" roads.Value)
                else printf "|        "
            | None ->
                Console.BackgroundColor <- ConsoleColor.Red
                printf "|  ERR   "
                Console.BackgroundColor <- bgColor
        printfn "|"
        // Draw the game pieces on the board
        for j in 1..board.ColCount do
            let cell = GetCell board.Cells i j
            match cell with
            | Some c ->
                match c.Piece with
                | Some p -> 
                    printf "|"
                    if p.Side = Piece.Side.Red then Console.BackgroundColor <- ConsoleColor.Red
                    else Console.BackgroundColor <- ConsoleColor.Blue
                    printf "%6s %s" p.Name  (FacingToString p.Facing)
                    Console.BackgroundColor <- bgColor
                | None -> printf "|        "
            | None ->
                printf "|"
                Console.BackgroundColor <- ConsoleColor.Red
                printf "  ERR   "
                Console.BackgroundColor <- bgColor
        printfn "|"
    // Draw the border on the bottom of the grid
    for _ in 1..board.ColCount do
        printf "+--------"
    printfn "+"
    // Draw the game pieces off the board
    printf "Off-board Pieces:"
    if board.Off.Length > 0 then
        for i in 0..board.Off.Length - 1 do
            let piece = board.Off.[i]
            if piece.Side = Piece.Side.Red then Console.BackgroundColor <- ConsoleColor.Red
            else Console.BackgroundColor <- ConsoleColor.Blue
            printf " %6s" piece.Name
            Console.BackgroundColor <- bgColor
    else printf " None"
    printfn ""
    // Draw the game pieces that are dead
    printf "Dead Pieces:"
    if board.Dead.Length > 0 then
        for i in 0..board.Dead.Length - 1 do
            let piece = board.Dead.[i]
            if piece.Side = Piece.Side.Red then Console.BackgroundColor <- ConsoleColor.Red
            else Console.BackgroundColor <- ConsoleColor.Blue
            printf " %6s" piece.Name
            Console.BackgroundColor <- bgColor
    else printf " None"
    printfn ""
