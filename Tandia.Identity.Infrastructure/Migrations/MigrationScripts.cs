namespace Tandia.Identity.Infrastructure.Migrations;

public static class MigrationScripts
{
    public const string InitialMigration = @"
    CREATE TABLE IF NOT EXISTS ""Users"" (
        ""Id"" UUID PRIMARY KEY,
        ""RegistrationDate"" TIMESTAMP NOT NULL
    );

    CREATE TABLE IF NOT EXISTS ""UserCredentials"" (
        ""Id"" UUID PRIMARY KEY,
        ""Email"" VARCHAR(255) NOT NULL UNIQUE,
        ""PasswordHash"" VARCHAR(255) NOT NULL,
        ""Salt"" VARCHAR(255) NOT NULL,
        FOREIGN KEY (""Id"") REFERENCES ""Users""(""Id"")
    );

    CREATE TABLE IF NOT EXISTS ""RefreshTokens"" (
        ""Id"" UUID PRIMARY KEY,
        ""UserId"" UUID NOT NULL,
        ""Token"" VARCHAR(255) NOT NULL,
        ""ExpiryDate"" TIMESTAMP NOT NULL,
        FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE
    );";
}
