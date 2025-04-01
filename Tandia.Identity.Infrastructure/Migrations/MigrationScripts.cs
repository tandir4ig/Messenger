using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tandia.Identity.Infrastructure.Migrations;

public static class MigrationScripts
{
    public const string InitialMigration = @"
        CREATE TABLE ""Users"" (
            Id UUID PRIMARY KEY,
            RegistrationDate TIMESTAMP NOT NULL
        );

        CREATE TABLE ""UserCredentials"" (
            Id UUID PRIMARY KEY,
            Email VARCHAR(255) NOT NULL UNIQUE,
            PasswordHash VARCHAR(255) NOT NULL,
            Salt VARCHAR(255) NOT NULL,
            FOREIGN KEY (Id) REFERENCES Users(Id)
        );";
}
