namespace BattleShips.Tests
open System
open Xunit
open BattleShips.GameOps

module Tests =

    let n = 10

    [<Fact>]
    let horizontalShipOusideNegRow() =
        let ship = {StartRow = -1; StartCol = 0; Alignment = Horizontal; Length = 1}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let horizontalShipOusideNegCol() =
        let ship = {StartRow = 0; StartCol = -1; Alignment = Horizontal; Length = 1}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let horizontalShipOusideNonPosLen() =
        let ship = {StartRow = 0; StartCol = 0; Alignment = Horizontal; Length = 0}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let horizontalShipOusideColTooLarge() =
        let ship = {StartRow = 0; StartCol = n; Alignment = Horizontal; Length = 1}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let horizontalShipOusideLenTooLarge() =
        let ship = {StartRow = 0; StartCol = 9; Alignment = Horizontal; Length = 2}
        let res = isShipWithinBoard n ship
        Assert.False(res)
    
    [<Fact>]
    let verticalShipOusideNegRow() =
        let ship = {StartRow = -1; StartCol = 0; Alignment = Vertical; Length = 1}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let verticalShipOusideNegCol() =
        let ship = {StartRow = 0; StartCol = -1; Alignment = Vertical; Length = 1}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let verticalShipOusideNonPosLen() =
        let ship = {StartRow = 0; StartCol = 0; Alignment = Vertical; Length = 0}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let verticalShipOusideColTooLarge() =
        let ship = {StartRow = n; StartCol = 0; Alignment = Vertical; Length = 1}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let verticalShipOusideLenTooLarge() =
        let ship = {StartRow = 9; StartCol = 0; Alignment = Vertical; Length = 2}
        let res = isShipWithinBoard n ship
        Assert.False(res)

    [<Fact>]
    let horizontalShipsOverlap() =
        let ship1 = {StartRow = 3; StartCol = 0; Alignment = Horizontal; Length = 5}
        let ship2 = {StartRow = 3; StartCol = 4; Alignment = Horizontal; Length = 3}
        let res = shipsOverlap  ship1 ship2
        Assert.True(res)

    [<Fact>]
    let horizontalShipsNoOverlapSameRow() =
        let ship1 = {StartRow = 3; StartCol = 0; Alignment = Horizontal; Length = 5}
        let ship2 = {StartRow = 3; StartCol = 5; Alignment = Horizontal; Length = 3}
        let res = shipsOverlap  ship1 ship2
        Assert.False(res)

    [<Fact>]
    let horizontalShipsNoOverlapDiffRow() =
        let ship1 = {StartRow = 3; StartCol = 0; Alignment = Horizontal; Length = 5}
        let ship2 = {StartRow = 4; StartCol = 0; Alignment = Horizontal; Length = 5}
        let res = shipsOverlap  ship1 ship2
        Assert.False(res)

    [<Fact>]
    let verticalShipsOverlap() =
        let ship1 = {StartRow = 0; StartCol = 3; Alignment = Vertical; Length = 5}
        let ship2 = {StartRow = 4; StartCol = 3; Alignment = Vertical; Length = 3}
        let res = shipsOverlap  ship1 ship2
        Assert.True(res)

    [<Fact>]
    let verticalShipsNoOverlapSameCol() =
        let ship1 = {StartRow = 0; StartCol = 3; Alignment = Vertical; Length = 5}
        let ship2 = {StartRow = 5; StartCol = 3; Alignment = Vertical; Length = 3}
        let res = shipsOverlap  ship1 ship2
        Assert.False(res)

    [<Fact>]
    let verticalShipsNoOverlapDiffCol() =
        let ship1 = {StartRow = 0; StartCol = 3; Alignment = Vertical; Length = 5}
        let ship2 = {StartRow = 0; StartCol = 4; Alignment = Vertical; Length = 5}
        let res = shipsOverlap  ship1 ship2
        Assert.False(res)

    [<Fact>]
    let horVerticalShipsOverlap() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 1; StartCol = 3; Alignment = Vertical; Length = 2}
        let res = shipsOverlap  ship1 ship2
        Assert.True(res)

    [<Fact>]
    let horVerticalShipsNoOverlap() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 1; StartCol = 4; Alignment = Vertical; Length = 2}
        let res = shipsOverlap  ship1 ship2
        Assert.False(res)

    [<Fact>]
    let vertHorShipsOverlap() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 1; StartCol = 3; Alignment = Vertical; Length = 2}
        let res = shipsOverlap  ship2 ship1
        Assert.True(res)

    [<Fact>]
    let vertHorShipsNoOverlap() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 1; StartCol = 4; Alignment = Vertical; Length = 2}
        let res = shipsOverlap  ship2 ship1
        Assert.False(res)

    [<Fact>]
    let addShipOverlapNoSuccess() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 1; StartCol = 3; Alignment = Vertical; Length = 2}
        let shipState = {Ship = ship1; IsCellHit = [|false;false|]}
        let res = tryAddShip [shipState] ship2
        Assert.Equal(res.Length, 1)
        Assert.Equal(res.Head, shipState)

    [<Fact>]
    let addShipOverlapSuccess() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 1; StartCol = 4; Alignment = Vertical; Length = 2}
        let shipState1 = {Ship = ship1; IsCellHit = [|false;false|]}
        let shipState2 = {Ship = ship2; IsCellHit = [|false;false|]}
        let res = tryAddShip [shipState1] ship2
        Assert.Equal(res.Length, 2)
        Assert.Equal(res.Head, shipState2)
        Assert.Equal(res.Tail.Head, shipState1)

    [<Fact>]
    let shipIsHit() =
        let ship = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let res1 = isShipHit 1 2 ship
        let res2 = isShipHit 1 3 ship
        Assert.True(res1)

    [<Fact>]
    let shipIsNotHit() =
        let ship = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let res1 = isShipHit 1 4 ship
        let res2 = isShipHit 2 3 ship
        Assert.False(res1)
        Assert.False(res2)

    [<Fact>]
    let someShipIsHit() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 2; StartCol = 2; Alignment = Vertical; Length = 2}
        let shipState1 = {Ship = ship1; IsCellHit = [|false;false|]}
        let shipState2 = {Ship = ship2; IsCellHit = [|false;false|]}
        let res = isAnyShipHit 2 2 [shipState1;shipState2]
        Assert.True(res)

    [<Fact>]
    let noneShipIsHit() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 2; StartCol = 2; Alignment = Vertical; Length = 2}
        let shipState1 = {Ship = ship1; IsCellHit = [|false;false|]}
        let shipState2 = {Ship = ship2; IsCellHit = [|false;false|]}
        let res = isAnyShipHit 2 6 [shipState1;shipState2]
        Assert.False(res)

    [<Fact>]
    let updateShipMissed() =
        let ship = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let shipState = {Ship = ship; IsCellHit= [|false;false|]}
        let res = updateHitShip 1 4 shipState
        Assert.Equal(res, shipState)

    [<Fact>]
    let updateHorizontalShipHit() =
        let ship = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let shipState = {Ship = ship; IsCellHit= [|false;false|]}
        let res = updateHitShip 1 3 shipState
        Assert.Equal(res, {Ship = ship; IsCellHit= [|false;true|]})

    [<Fact>]
    let updateVerticalShipHit() =
        let ship = {StartRow = 1; StartCol = 2; Alignment = Vertical; Length = 2}
        let shipState = {Ship = ship; IsCellHit= [|false;false|]}
        let res = updateHitShip 2 2 shipState
        Assert.Equal(res, {Ship = ship; IsCellHit= [|false;true|]})

    [<Fact>]
    let allShipsHit() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 2; StartCol = 2; Alignment = Vertical; Length = 2}
        let shipState1 = {Ship = ship1; IsCellHit = [|true;true|]}
        let shipState2 = {Ship = ship2; IsCellHit = [|true;true|]}
        let res = areAllHit [shipState1;shipState2]
        Assert.True(res)

    [<Fact>]
    let allShipsNotHit() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 2; StartCol = 2; Alignment = Vertical; Length = 2}
        let shipState1 = {Ship = ship1; IsCellHit = [|true;true|]}
        let shipState2 = {Ship = ship2; IsCellHit = [|true;false|]}
        let res = areAllHit [shipState1;shipState2]
        Assert.False(res)

    [<Fact>]
    let attackSuccess() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let game = GameManager()
        game.AddShip(ship1)
        let res = game.Attack(1, 3) |> Async.RunSynchronously
        Assert.True(res)

    [<Fact>]
    let attackNoSuccess() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let game = GameManager()
        game.AddShip(ship1)
        let res = game.Attack(1, 4) |> Async.RunSynchronously
        Assert.False(res)

    [<Fact>]
    let gameNotFinished() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 2; StartCol = 2; Alignment = Vertical; Length = 2}
        let game = GameManager()
        game.AddShip(ship1)
        game.AddShip(ship2)
        game.Attack(1, 2) |> Async.RunSynchronously |> ignore
        game.Attack(1, 3) |> Async.RunSynchronously |> ignore
        let res = game.IsGameFinished() |> Async.RunSynchronously
        Assert.False(res)

    [<Fact>]
    let gameFinished() =
        let ship1 = {StartRow = 1; StartCol = 2; Alignment = Horizontal; Length = 2}
        let ship2 = {StartRow = 2; StartCol = 2; Alignment = Vertical; Length = 2}
        let game = GameManager()
        game.AddShip(ship1)
        game.AddShip(ship2)
        game.Attack(1, 2) |> Async.RunSynchronously |> ignore
        game.Attack(1, 3) |> Async.RunSynchronously |> ignore
        game.Attack(2, 2) |> Async.RunSynchronously |> ignore
        game.Attack(3, 2) |> Async.RunSynchronously |> ignore
        let res = game.IsGameFinished() |> Async.RunSynchronously
        Assert.True(res)