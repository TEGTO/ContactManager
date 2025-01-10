namespace ContactManager.Api
{
    public static class ConfigurationKeys
    {
        public static string ALLOWED_CORS_ORIGINS { get; } = "AllowedCORSOrigins";
        public static string DATABASE_CONNECTION_STRING { get; } = "Db";
        public static string EF_CREATE_DATABASE { get; } = "EFCreateDatabase";
    }
}
