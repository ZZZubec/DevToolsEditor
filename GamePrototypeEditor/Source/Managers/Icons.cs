using Urho3DNet;

namespace GPE
{
    public class Icons
    {
        public static Texture2D select = Context.Instance.ResourceCache.GetResource<Texture2D>("Icons/select.png");
        public static Texture2D terrain = Context.Instance.ResourceCache.GetResource<Texture2D>("Icons/terrain.png");
        public static Texture2D paint = Context.Instance.ResourceCache.GetResource<Texture2D>("Icons/paint.png");
        public static Texture2D UI = Context.Instance.ResourceCache.GetResource<Texture2D>("Icons/UI.png");
        public static Texture2D UI_panel = Context.Instance.ResourceCache.GetResource<Texture2D>("Icons/UI_panel.png");

        public static int font1_symbols_width = 42;
        public static int font1_glyph_width = 6;
        public static int font1_glyph_height = 11;
        public static string font_symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
        public static Texture2D font1 = Context.Instance.ResourceCache.GetResource<Texture2D>("Icons/font1.png");
        public static int font2_symbols_width = 36;
        public static int font2_glyph_width = 7;
        public static int font2_glyph_height = 11;
        public static Texture2D font2 = Context.Instance.ResourceCache.GetResource<Texture2D>("Icons/font2.png");
    }
}