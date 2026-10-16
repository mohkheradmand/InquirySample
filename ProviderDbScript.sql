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

COMMIT;
GO

