IF DB_ID(N'ProvidersDb') IS NULL
BEGIN
    CREATE DATABASE [ProvidersDb];
END;
GO

USE [ProvidersDb];
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [HSCode] nvarchar(20) NOT NULL,
    [IsLicenseValid] bit NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_Products_HSCode] ON [Products] ([HSCode]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261016003837_init', N'10.0.13');

-- 200 HsCode az 100000 ta 100199
;WITH Numbers AS
(
    SELECT TOP (200)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
)
INSERT INTO dbo.Products (HSCode, IsLicenseValid)
SELECT
    RIGHT('000000' + CAST(100000 + N - 1 AS VARCHAR(6)), 6),
    CONVERT(BIT, CHECKSUM(NEWID()) & 1)
FROM Numbers;


COMMIT;
GO

