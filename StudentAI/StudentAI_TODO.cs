using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;
using FullSailAFI.GamePlaying.CoreAI;
using System;
using System.Collections.Generic;
using FullSailAFI.GamePlaying.CoreAI;

namespace FullSailAFI.GamePlaying
{
    public class StudentAI : Behavior
    {
        TreeVisLib treeVisLib;
        bool visualizationFlag = false;

        private static readonly int[,] POSITION_WEIGHTS = new int[8, 8] {
            { 100, -20,  10,   5,   5,  10, -20, 100 },
            { -20, -30,  -5,  -5,  -5,  -5, -30, -20 },
            {  10,  -5,   1,   1,   1,   1,  -5,  10 },
            {   5,  -5,   1,   1,   1,   1,  -5,   5 },
            {   5,  -5,   1,   1,   1,   1,  -5,   5 },
            {  10,  -5,   1,   1,   1,   1,  -5,  10 },
            { -20, -30,  -5,  -5,  -5,  -5, -30, -20 },
            { 100, -20,  10,   5,   5,  10, -20, 100 }
        };

        public StudentAI()
        {
            if (visualizationFlag == true)
            {
                if (treeVisLib == null)
                    treeVisLib = TreeVisLib.getTreeVisLib();
            }
        }

        public ComputerMove Run(int nextColor, Board board, int lookAheadDepth)
        {
            if (lookAheadDepth <= 0)
            {
                return FindBestImmediateMove(nextColor, board);
            }
            return GetBestMove(nextColor, board, lookAheadDepth, int.MinValue, int.MaxValue);
        }

        private ComputerMove FindBestImmediateMove(int color, Board board)
        {
            ComputerMove bestMove = null;
            int bestValue = int.MinValue;

            for (int row = 0; row < Board.Height; row++)
            {
                for (int col = 0; col < Board.Width; col++)
                {
                    if (board.IsValidMove(color, row, col))
                    {
                        Board tempB = new Board();
                        tempB.Copy(board);
                        tempB.MakeMove(color, row, col);

                        int ValueOfMove;
                        ValueOfMove = Evaluate(tempB, color);

                        if (ValueOfMove > bestValue)
                        {
                            bestValue = ValueOfMove;
                            bestMove = new ComputerMove(row, col);
                            bestMove.rank = ValueOfMove;
                        }
                    }
                }
            }

            return bestMove;
        }

        private ComputerMove GetBestMove(int color, Board board, int depth, int alpha, int beta)
        {
            ComputerMove bestMove = null;
            int bestValue = int.MinValue;

            for (int row = 0; row < Board.Height; row++)
            {
                for (int col = 0; col < Board.Width; col++)
                {
                    if (board.IsValidMove(color, row, col))
                    {
                        Board tempB = new Board();
                        tempB.Copy(board);
                        tempB.MakeMove(color, row, col);

                        int ValueOfMove = Minimax(tempB, depth - 1, false, color, alpha, beta);

                        if (ValueOfMove > bestValue)
                        {
                            bestValue = ValueOfMove;
                            bestMove = new ComputerMove(row, col);
                            bestMove.rank = ValueOfMove;
                        }

                        alpha = Math.Max(alpha, bestValue);
                        if (beta <= alpha)
                            break;
                    }
                }
            }

            return bestMove;
        }

        private int Minimax(Board board, int depth, bool MaximizingQuestionMark, int originalColor, int alpha, int beta)
        {
            if (depth <= 0 || board.IsTerminalState())
            {
                return Evaluate(board, originalColor);
            }

            int currentColor = MaximizingQuestionMark ? originalColor : (originalColor == Board.Black ? Board.White : Board.Black);

            if (!board.HasAnyValidMove(currentColor))
            {
                if (!board.HasAnyValidMove(originalColor == Board.Black ? Board.White : Board.Black))
                {
                    return Evaluate(board, originalColor);
                }
                return Minimax(board, depth - 1, !MaximizingQuestionMark, originalColor, alpha, beta);
            }

            int bestValue;
            if(MaximizingQuestionMark)
            {
                bestValue = int.MinValue;
            }
            else
            {
                bestValue = int.MaxValue;
            }
            for (int row = 0; row < Board.Height; row++)
            {
                for (int col = 0; col < Board.Width; col++)
                {
                    if (board.IsValidMove(currentColor, row, col))
                    {
                        Board tempB = new Board();
                        tempB.Copy(board);
                        tempB.MakeMove(currentColor, row, col);

                        int ValueOfMove = Minimax(tempB, depth - 1, !MaximizingQuestionMark, originalColor, alpha, beta);

                        if (MaximizingQuestionMark)
                        {
                            bestValue = Math.Max(bestValue, ValueOfMove);
                            alpha = Math.Max(alpha, bestValue);
                        }
                        else
                        {
                            bestValue = Math.Min(bestValue, ValueOfMove);
                            beta = Math.Min(beta, bestValue);
                        }

                        if (beta <= alpha)
                            break;
                    }
                }
            }

            return bestValue;
        }

        private int Evaluate(Board board, int playerColor)
        {
            int score = 0;

            for (int row = 0; row < Board.Height; row++)
            {
                for (int col = 0; col < Board.Width; col++)
                {
                    int square = board.GetSquareContents(row, col);

                    if (square != Board.Empty)
                    {
                        int pieceValue = square == playerColor ? 1 : -1;
                        score += pieceValue * POSITION_WEIGHTS[row, col];
                    }
                }
            }

            if (board.IsTerminalState())
            {
                int totalScore;
                totalScore = 0;
                for (int row = 0; row < Board.Height; row++)
                {
                    for (int col = 0; col < Board.Width; col++)
                    {
                        int square = board.GetSquareContents(row, col);
                        if (square == playerColor)
                            totalScore= totalScore+1 ;
                        else if (square != Board.Empty)
                            totalScore--;
                    }
                }
                score += totalScore * 1000;
            }

            return score;
        }
    }
}