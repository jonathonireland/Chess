# C# Chess Game

A desktop chess application built with C#, .NET 8, and Windows Presentation Foundation (WPF). The solution separates reusable chess rules from presentation concerns through dedicated `ChessLogic` and `ChessUI` projects.

This project began as a guided learning exercise and was later extended with independently implemented game logic, user-interface improvements, and a computer-controlled opponent.

## Features

- Complete 8 × 8 chess board with standard starting positions
- Legal move generation for pawns, knights, bishops, rooks, queens, and kings
- King-safety validation that rejects moves leaving the current player in check
- Check notifications
- King-side and queen-side castling with movement, obstruction, and attacked-square validation
- Legal-move highlighting
- Captured-piece tracking and display
- Human White player versus computer-controlled Black player
- Material-based AI move scoring with randomized tie-breaking
- Turn and game-state management

## Architecture

- **ChessLogic** — board representation, pieces, moves, rule validation, game state, and AI selection
- **ChessUI** — WPF board rendering, input handling, captured-piece display, move highlighting, and computer-turn orchestration

Keeping the rules independent from the UI makes the game logic easier to reason about, extend, and test.

## Technologies

- C#
- .NET 8
- WPF
- Object-oriented design
- LINQ
- Asynchronous UI workflows

## Run Locally

### Prerequisites

- Windows
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Start the application

```bash
dotnet restore
dotnet run --project ChessUI/ChessUI.csproj
```

You can also open `Chess.sln` in Visual Studio and run the `ChessUI` project.

## Controls

Select a White piece, then select one of its highlighted legal destination squares. After the move completes, the Black AI automatically chooses and executes its response.

## AI Strategy

The current AI evaluates every legal Black move, applies it to a copy of the board, and scores the resulting material balance using standard relative piece values. It selects randomly among equally scored moves, producing legal and varied play without blocking the WPF interface.

## Tutorial Foundation and Original Enhancements

The initial implementation followed a multi-part C# chess tutorial. The repository was subsequently expanded with independently developed features including:

- Computer-controlled Black player
- Board evaluation and legal AI move selection
- King-safety filtering
- Check detection and notifications
- Castling validation and execution
- Captured-piece tracking and display
- Improved game-state and UI coordination

## Tutorial Series

1. [Project Setup](https://youtu.be/GEkSE6eZMGc)
2. [Positions and Directions](https://youtu.be/KuAsKRn9XD0)
3. [Pieces and the Board](https://youtu.be/NUNlVjt82m8)
4. [Drawing the Board](https://youtu.be/Z1Zi41eiNGs)
5. [Generating Moves](https://youtu.be/RDD48hIgAqU)
6. [Generating Moves II](https://youtu.be/3z_EitUuTWI)
7. [Handling Moves](https://youtu.be/cpGusMTczTQ)

## Future Improvements

- Checkmate and stalemate detection
- En passant
- Pawn-promotion selection UI
- Deeper AI search such as minimax with alpha-beta pruning
- Move history
- Save and load support
- Automated tests for rule and AI behavior
