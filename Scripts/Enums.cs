using JamEnums;

using Godot;

using System;
using System.Data;

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
        Dash,
    };

    public enum Key {
        PadA,
        PadB,
        PadX,
        PadY,
        Menu,
    };

    public enum EffectsAnimations {
        BloodDash,
    }

    public enum ActorType {
        Player,
    };

    public enum Item {
        RedScroll,
        BlueScroll,
    }

    static class EnumExtensions {
        public static string Name(this MoveMode mode) {
            switch (mode) {
                case MoveMode.Run: return "run";
                case MoveMode.Walk: return "walk";
                case MoveMode.Dash: return "dash";
            }

            throw new ArgumentException("Unexpected move mode: " + mode);
        }

        public static string Name(this Direction d) {
            switch (d) {
                case Direction.Up: return "up";
                case Direction.Down: return "down";
                case Direction.Right: return "right";
                case Direction.Left: return "left";
            }
            throw new ArgumentException("Unexpected direction: " + d);
        }

        public static string Name(this Key keyName) {
            switch (keyName) {
                case Key.PadA: return "pad_a";
                case Key.PadB: return "pad_b";
                case Key.PadX: return "pad_x";
                case Key.PadY: return "pad_y";
                case Key.Menu: return "menu";
            }

            throw new ArgumentException("Unexpected KeyName " + keyName);
        }

        public static string Name(this EffectsAnimations effect) {
            switch (effect) {
                case EffectsAnimations.BloodDash: return "blood_dash";
            }

            throw new ArgumentException("Unknown effect animation: " + effect);
        }
    
        public static string Name(this Item i) {
            switch (i) {
                case Item.RedScroll: return "red_scroll";
                case Item.BlueScroll: return "blue_scroll";
            }

            throw new ArgumentException("Invalid item: " + i);
        }
    }
}