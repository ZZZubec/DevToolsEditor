namespace GPE.utils
{
    public enum EnumPlatform
    {
        PC, Android, iOS
    }

    public class Platforms
    {
        public EnumPlatform currentPlatform;

        public Platforms(EnumPlatform platform)
        {
            this.currentPlatform = platform;
        }

        public static EnumPlatform PC = EnumPlatform.PC;
        public static EnumPlatform Android = EnumPlatform.Android;
        public static EnumPlatform iOS = EnumPlatform.iOS;
    }
}