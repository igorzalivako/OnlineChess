# Online Chess

An online chess mobile app for real-time matches against other players or a game‑theory–based chess bot.

## Features
- Real-time multiplayer matches
- Play vs human or chess bot
- Live board sync and move validation
- User accounts: registration, login
- ELO-like rating
- Game history with FEN export
- In‑game timers
- Illegal‑move handling

## Tech Stack
- Backend: ASP.NET Core, SignalR, Entity Framework
- Database: MySQL
- Frontend: .NET MAUI
- Auth: JWT
- Bot: minimax/alpha‑beta, move ordering, opening book

## Architecture
- Client ↔ SignalR Hub for real-time events (moves, timers)
- ASP.NET Core endpoints for auth
- EF (ORM) for DB access; migrations for schema changes
- Separation of concerns:
  - ChessEngine: chess rules, FEN, bot
  - Infrastructure: EF repositories, MySQL
  - ChessServer: controllers, hubs
  - ChessClient: UI
