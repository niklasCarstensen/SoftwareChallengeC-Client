﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareChallengeClient
{
    /// <summary>
    /// This Class contains the X and Y coordinates the Move starts from, aswell as the Direction of the Move and DebugHints.
    /// <para>The coordinates the Move ends on can be calculated by calling GetEndpointOn</para> 
    /// </summary>
    public class Move
    {
        public int X, Y;
        public Direction MoveDirection;
        public List<string> DebugHints;

        /// <summary>
        /// Creates a New Move
        /// </summary>
        /// <param name="X"> X coordinate of the starting location </param>
        /// <param name="Y"> Y coordinate of the starting location </param>
        public Move(int X, int Y, Direction MoveDirection)
        {
            this.X = X;
            this.Y = Y;
            this.MoveDirection = MoveDirection;
            DebugHints = new List<string>();
        }

        /// <summary>
        /// Checks if this move can be performed on Board B
        /// </summary>
        /// <param name="B"> The Board this move should be performed on </param>
        /// <param name="Team"> The Team that wants to perform the move </param>
        public bool IsLegalOn(Board B, PlayerColor Team) // https://youtu.be/nz20lu2AM2k?t=8
        {
            int moveDistance = B.NumberOfFishInRow(X, Y, MoveDirection);
            Point endPoint = this.GetEndpointOn(moveDistance);
            return endPoint.X >= 0 &&
                   endPoint.X < Board.BoardWidth &&
                   endPoint.Y >= 0 &&
                   endPoint.Y < Board.BoardHeight &&
                   B.Fields[X, Y].HasPiranha() &&
                   B.Fields[X, Y].State == Team.ToFieldState() &&
                   B.GetFieldsInDir(X, Y, MoveDirection, moveDistance).
                    TrueForAll(x => x.State != Team.OtherTeam().ToFieldState()) &&
                   (B.Fields[endPoint.X, endPoint.Y].State == FieldState.EMPTY ||
                    B.Fields[endPoint.X, endPoint.Y].HasPiranha() &&
                    B.Fields[endPoint.X, endPoint.Y].State != Team.ToFieldState());
        }

        /// <summary>
        /// Calculates the Coordinates the Move will land on.
        /// <para>Caution: This Method neither checks if the Move is legal nor does it check if the EndPoint is even on the Board!</para> 
        /// </summary>
        /// <param name="B"> The Board this move should be performed on </param>
        public Point GetEndpointOn(Board B)
        {
            int moveDistance = B.NumberOfFishInRow(X, Y, MoveDirection);
            return GetEndpointOn(moveDistance);
        }
        /// <summary>
        /// Calculates the Coordinates the Move will land on.
        /// <para>Caution: This Method neither checks if the Move is legal nor does it check if the EndPoint is even on the Board!</para> 
        /// </summary>
        /// <param name="MoveDistance"> The Distance your fish will go in this Move </param>
        public Point GetEndpointOn(int MoveDistance)
        {
            switch (MoveDirection)
            {
                case Direction.DOWN:
                    return new Point(X, Y - MoveDistance);

                case Direction.DOWN_LEFT:
                    return new Point(X - MoveDistance, Y - MoveDistance);

                case Direction.DOWN_RIGHT:
                    return new Point(X + MoveDistance, Y - MoveDistance);

                case Direction.LEFT:
                    return new Point(X - MoveDistance, Y);

                case Direction.RIGHT:
                    return new Point(X + MoveDistance, Y);

                case Direction.UP:
                    return new Point(X, Y + MoveDistance);

                case Direction.UP_LEFT:
                    return new Point(X - MoveDistance, Y + MoveDistance);

                case Direction.UP_RIGHT:
                    return new Point(X + MoveDistance, Y + MoveDistance);

                default:
                    throw new Exception("That direction doesn't exist!");
            }
        }

        /// <summary>
        /// Converts this Move to XML
        /// <para>This is used to pack the Move into a format that can be send to the Server</para> 
        /// <para>You usually wont need this Method if you are programming your Client Logic</para> 
        /// </summary>
        public string ToXML()
        {
            if (this == null)
                return "";
            
            return $"<room roomId=\"{Program.RoomID}\"><data class=\"move\" x=\"{X}\" y=\"{Y}\" direction=\"{MoveDirection}\">" +
                $"{(DebugHints.Count > 0 ? DebugHints.Select(x => $"<hint content=\"{x}\"/>").Aggregate((x, y) => x + y) : "")}</data></room>";
        }
    }

    public enum Direction { UP, UP_RIGHT, RIGHT, DOWN_RIGHT, DOWN, DOWN_LEFT, LEFT, UP_LEFT }
}