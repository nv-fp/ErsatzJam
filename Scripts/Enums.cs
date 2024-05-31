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
        PadLT,
        Menu,
    };

    public enum EffectsAnimations {
        BloodDash,
        Attack,
    }

    public enum ActorType {
        Player,
    };

    public enum Item {
        RedScroll,
        BlueScroll,
        Sword,
        Bow,
        Shield,
        Apple,
        Cheese,
    }

    public enum BehaviorModel {
        None,
        Guard,
        Civilian,
    }

    public enum GuardWanderMode {
        None,
        Patrol,
        Wander,
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
                case Key.PadA: return "action";
                case Key.PadX: return "run";
                case Key.PadY: return "dodge";
                case Key.PadB: return "attack";
                case Key.PadLT: return "power";
                case Key.Menu: return "menu";
            }

            throw new ArgumentException("Unexpected KeyName " + keyName);
        }

        public static string Name(this EffectsAnimations effect) {
            switch (effect) {
                case EffectsAnimations.BloodDash: return "blood_dash";
                case EffectsAnimations.Attack: return "attack";
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

        public static Texture2D GetTexture(this Item i) {
            var itemSheet = TextureFactory.Instance.Get("res://Art/ItemSheet.png");
            var atlasTex = new AtlasTexture();
            atlasTex.Atlas = itemSheet;
            var x = 0;
            var y = 0;
            switch (i) {
                case Item.RedScroll:
                    x = 4;
                    y = 4;
                    break;
                case Item.BlueScroll:
                    x = 5;
                    y = 4;
                    break;
                case Item.Sword:
                    x = 0;
                    y = 0;
                    break;
                case Item.Bow:
                    x = 0;
                    y = 2;
                    break;
                case Item.Shield:
                    x = 0;
                    y = 3;
                    break;
                case Item.Apple:
                    x = 0;
                    y = 8;
                    break;
                case Item.Cheese:
                    x = 1;
                    y = 8;
                    break;
            }
            atlasTex.Region = new Rect2(x * 16, y * 16, 16, 16);
            return atlasTex;

        }
    }
}