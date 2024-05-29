using System.Collections.Generic;

using Godot;

class TextureFactory {
    private static TextureFactory _instance;
    public static TextureFactory Instance {
        get {
            if (_instance == null) {
                Instance = new TextureFactory();
            }
            return _instance;
        }
        private set {
            _instance = value; 
        }
    }

    Dictionary<string, Texture2D> store;
    private TextureFactory() {
        store = new Dictionary<string, Texture2D>();
    }

    public Texture2D Get(string path) {
        if (!store.ContainsKey(path)) {
            store.Add(path, ResourceLoader.Load<Texture2D>(path));
        }
        return store[path];
    }
}
