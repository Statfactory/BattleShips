namespace BattleShips

type Agent<'T> = MailboxProcessor<'T>

module GameOps =

    type ShipAlignment = 
        | Horizontal
        | Vertical
    
    type PositionedShip =
        {
            StartRow : int
            StartCol : int
            Alignment : ShipAlignment
            Length : int
        }

    type ShipState =
        {
            Ship : PositionedShip
            IsCellHit : bool[]
        }

    type PlayMove =
        | AddShip of ship : PositionedShip
        | Attack of row : int * col : int * reply : AsyncReplyChannel<bool>
        | CheckIsGameFinished of reply : AsyncReplyChannel<bool>

    type GameState =
        {
            SetupDone: bool
            ShipsState: ShipState list
        }

    let isShipWithinBoard (boardSize: int) (ship: PositionedShip) =
        match ship with
            | {StartRow = row; StartCol = col; Alignment = Horizontal; Length = n } -> 
                row >= 0 && col >= 0 && n > 0 && col < boardSize && (col + n - 1 < boardSize)
            | {StartRow = row; StartCol = col; Alignment = Vertical; Length = n } -> 
                row >= 0 && col >= 0 && n > 0 && row < boardSize && (row + n - 1 < boardSize)

    let shipsOverlap (ship1 : PositionedShip) (ship2: PositionedShip) =
        match ship1, ship2 with
            | {StartRow = row1; StartCol = col1; Alignment = Horizontal; Length = n1 },
              {StartRow = row2; StartCol = col2; Alignment = Horizontal; Length = n2 } ->
                  row1 = row2 && (if col1 < col2 then col1 + n1 > col2 else col2 + n2 > col1)
            | {StartRow = row1; StartCol = col1; Alignment = Vertical; Length = n1 },
              {StartRow = row2; StartCol = col2; Alignment = Vertical; Length = n2 } ->
                  col1 = col2 && (if row1 < row2 then row1 + n1 > row2 else row2 + n2 > row1)
            | {StartRow = row1; StartCol = col1; Alignment = Horizontal; Length = n1 },
              {StartRow = row2; StartCol = col2; Alignment = Vertical; Length = n2 } ->
                  col1 <= col2 && col2 <= col1 + n1 - 1 && row2 <= row1 && row1 <= row2 + n2 - 1                                
            | {StartRow = row1; StartCol = col1; Alignment = Vertical; Length = n1 },
              {StartRow = row2; StartCol = col2; Alignment = Horizontal; Length = n2 } ->
                  col2 <= col1 && col1 <= col2 + n2 - 1 && row1 <= row2 && row2 <= row1 + n1 - 1                    

    let tryAddShip (shipsState : ShipState list) (ship: PositionedShip) =
        let hasOverlap = shipsState |> List.map (fun x -> x.Ship) |> List.map (shipsOverlap ship)
                                           |> List.fold (||) false
        if hasOverlap = false then 
            let shipState = {Ship = ship; IsCellHit = Array.create ship.Length false}
            shipState :: shipsState
        else shipsState

    let isShipHit (row: int) (col : int) (ship : PositionedShip) =
        let virtualShip = {StartRow = row; StartCol = col; Alignment = Horizontal; Length = 1}
        shipsOverlap virtualShip ship

    let isAnyShipHit (row : int) (col : int) (shipsState : ShipState list) =
        shipsState |> List.map (fun x -> x.Ship) |> List.map (isShipHit row col)
                   |> List.fold (||) false

    let updateHitShip (row : int) (col : int) (shipState: ShipState) =
        let ship = shipState.Ship
        let isCellHit = shipState.IsCellHit
        let virtualShip = {StartRow = row; StartCol = col; Alignment = Horizontal; Length = 1}
        if shipsOverlap virtualShip ship then
            match ship with
                | {StartRow = _; StartCol = startCol; Alignment = Horizontal; Length = n } ->
                    isCellHit[col - startCol] <- true
                    shipState
                | {StartRow = startRow; StartCol = _; Alignment = Vertical; Length = n } ->
                    isCellHit[row - startRow] <- true
                    shipState
        else
            shipState

    let areAllHit (shipsState : ShipState list) =
        shipsState |> List.map (fun x -> x.IsCellHit |> Array.fold (&&) true)
                   |> List.fold (&&) true

    type GameManager() =
        let n = 10

        let agent = Agent<PlayMove>.Start(fun inbox ->
            
            let rec play (gameState: GameState) =
                async {
                    let! msg = inbox.Receive()
                    match msg with
                        | AddShip(ship) -> 
                            if gameState.SetupDone then
                                return! play gameState
                            else
                                let isWithinBoard = isShipWithinBoard n ship
                                if isWithinBoard then
                                    let shipsState = tryAddShip gameState.ShipsState ship
                                    return! play {gameState with ShipsState = shipsState}
                                else
                                    return! play gameState

                        | Attack(row, col, reply) ->
                            let isHit = isAnyShipHit row col gameState.ShipsState
                            reply.Reply(isHit)
                            let updatedState = gameState.ShipsState |> List.map (updateHitShip row col)
                            return! play {gameState with SetupDone = true; ShipsState = updatedState}

                        | CheckIsGameFinished(reply) -> 
                            if gameState.SetupDone = false then
                                reply.Reply(false)
                                return! play gameState
                            else
                                let allHit = areAllHit gameState.ShipsState
                                reply.Reply(allHit)
                                return! play gameState
                }
            play {SetupDone = false; ShipsState = []}
        )

        member this.AddShip(ship) =
            agent.Post(AddShip ship)
        
        member this.Attack(row, col) =
            agent.PostAndAsyncReply(fun reply -> Attack(row, col, reply))

        member this.IsGameFinished() =
            agent.PostAndAsyncReply(fun reply -> CheckIsGameFinished reply)
