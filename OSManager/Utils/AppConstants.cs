namespace OSManager.Utils
{
    public static class AppConstants
    {
        public static readonly string[] AllowedImageTypes = ["image/jpeg", "image/jpg", "image/png"];
        public static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png"];
        public const int MaxImageFileSize = 5 * 1024 * 1024; // 5MB
    }
}