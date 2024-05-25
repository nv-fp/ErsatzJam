using JamEnums;

using Godot;

using System;

namespace JamEnums {
    public enum Direction {
        Up,
        Down,
        Left,
        Right,
    };

    public enum MoveMode {
        Walk,
        Run,
    };

    public enum KeyName {
        PadA,
        PadB,
        PadX,
        PadY,
        Menu,
    };

    public class Util {
        public static string DirectionToString(Direction d) {
            switch (d) {
                case Direction.Up: return "up";
                case Direction.Down: return "down";
                case Direction.Right: return "right";
                case Direction.Left: return "left";
            }
            throw new ArgumentException("Unexpected direction: " + d);
        }

        public static Direction DirectionFromString(string s) {
            switch (s.ToLower().Trim()) {
                case "up": return Direction.Up;
                case "down": return Direction.Down;
                case "right": return Direction.Right;
                case "left": return Direction.Left;
            }
            throw new ArgumentException("Unexpected direction string: " + s);
        }

        public static MoveMode MoveModeFromString(string s) {
            switch (s.ToLower().Trim()) {
                case "run": return MoveMode.Run;
                case "walk": return MoveMode.Walk;
            }
            throw new ArgumentException("Unexpected move mode string: " + s);
        }
    }

    static class EnumExtensions {
        public static string Name(this MoveMode mode) {
            switch (mode) {
                case MoveMode.Run: return "run";
                case MoveMode.Walk: return "walk";
            }
            throw new ArgumentException("Unexpected move mode: " + mode);
        }

        public static string Name(this KeyName keyName) {
            switch (keyName) {
                case KeyName.PadA: return "pad_a";
                case KeyName.PadB: return "pad_b";
                case KeyName.PadX: return "pad_x";
                case KeyName.PadY: return "pad_y";
                case KeyName.Menu: return "menu";
            }

            throw new ArgumentException("Unexpected KeyName " + keyName);
        }
    }
}