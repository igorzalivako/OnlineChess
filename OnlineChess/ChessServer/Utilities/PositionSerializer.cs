using ChessEngine;

namespace ChessServer.Utilities
{
    public static class PositionSerializer
    {
        public static byte[] SerializeToBytes(Position position)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            
            // Сериализация основных полей
            writer.Write(position.EnPassant);
            writer.Write(position.WhiteLongCastling);
            writer.Write(position.WhiteShortCastling);
            writer.Write(position.BlackLongCastling);
            writer.Write(position.BlackShortCastling);
            writer.Write(position.WhiteCastlingHappened);
            writer.Write(position.BlackCastlingHappened);
            writer.Write(position.MoveCounter);
            writer.Write(position.FiftyMovesCounter);

            WritePiecesBytes(position.Pieces, writer);

            writer.Write(position.Hash.Hash);

            WriteRepetitionHistoryBytes(position.RepetitionHistory, writer);

            return stream.ToArray();
        }

        private static void WriteRepetitionHistoryBytes(RepetitionHistory repetitionHistory, BinaryWriter writer)
        {
            writer.Write(repetitionHistory.Hashes.Count);
            foreach (ZobristHash zobristHash in repetitionHistory.Hashes)
            {
                writer.Write(zobristHash.Hash);
            }
        }

        private static void WritePiecesBytes(Pieces pieces, BinaryWriter writer)
        {
            for (int i = 0; i < (int)PieceColor.None; i++)
            {
                for (int j = 0; j < (int)PieceType.None; j++)
                {
                    writer.Write(pieces.PieceBitboards[i, j].Value);
                }
            }
            writer.Write(pieces.SideBitboards[(int)PieceColor.White].Value);
            writer.Write(pieces.SideBitboards[(int)PieceColor.Black].Value);
            writer.Write(pieces.InversionSideBitboards[(int)PieceColor.White].Value);
            writer.Write(pieces.InversionSideBitboards[(int)PieceColor.Black].Value);
            writer.Write(pieces.All.Value);
            writer.Write(pieces.Empty.Value);
        }

        public static Position DeserializeFromBytes(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);

            // Десериализация основных полей
            byte enPassant = reader.ReadByte();
            bool whiteLongCastling = reader.ReadBoolean();
            bool whiteShortCastling = reader.ReadBoolean();
            bool blackLongCastling = reader.ReadBoolean();
            bool blackShortCastling = reader.ReadBoolean();
            bool whiteCastlingHappened = reader.ReadBoolean();
            bool blackCastlingHappened = reader.ReadBoolean();
            float moveCounter = reader.ReadSingle();
            int fiftyMovesCounter = reader.ReadInt32();

            // Десериализация Pieces
            Pieces pieces;
            ReadPiecesBytes(out pieces, reader);

            ulong hash = reader.ReadUInt64();
            ZobristHash zobristHash = new ZobristHash(hash);

            RepetitionHistory repetitionHistory = ReadRepetitionHistory(reader);

            return new Position(pieces, enPassant, whiteLongCastling, whiteShortCastling, blackLongCastling,
                                blackShortCastling, whiteCastlingHappened, blackCastlingHappened, moveCounter,
                                zobristHash, repetitionHistory, fiftyMovesCounter);
        }

        private static RepetitionHistory ReadRepetitionHistory(BinaryReader reader)
        {
            RepetitionHistory repetitionHistory = new RepetitionHistory();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ulong hash = reader.ReadUInt64();
                ZobristHash zobristHash = new ZobristHash(hash);
                repetitionHistory.AddPosition(zobristHash);
            }
            return repetitionHistory;
        }

        private static void ReadPiecesBytes(out Pieces pieces, BinaryReader reader)
        {
            Bitboard all, empty;
            Bitboard[,] pieceBitboards = new Bitboard[2, 6];
            Bitboard[] inversionSideBitboards = new Bitboard[2];
            Bitboard[] sideBitboards = new Bitboard[2];
            for (int i = 0; i < (int)PieceColor.None; i++)
            {
                for (int j = 0; j < (int)PieceType.None; j++)
                {
                    pieceBitboards[i, j].Value = reader.ReadUInt64();
                }
            }
            sideBitboards[0].Value = reader.ReadUInt64();
            sideBitboards[1].Value = reader.ReadUInt64();
            inversionSideBitboards[0].Value = reader.ReadUInt64();
            inversionSideBitboards[1].Value = reader.ReadUInt64();
            all.Value = reader.ReadUInt64();
            empty.Value = reader.ReadUInt64();
            pieces = new Pieces(all, empty, sideBitboards, inversionSideBitboards, pieceBitboards);
        }
    }
}