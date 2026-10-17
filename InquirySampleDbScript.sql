IF DB_ID(N'InquirySampleDb') IS NULL
BEGIN
    CREATE DATABASE [InquirySampleDb];
END;
GO

USE [InquirySampleDb];
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
CREATE TABLE [ErrorLogs] (
    [Id] int NOT NULL IDENTITY,
    [Source] nvarchar(100) NOT NULL,
    [Exception] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ErrorLogs] PRIMARY KEY ([Id])
);

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [HsCode] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);

CREATE TABLE [Providers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(20) NOT NULL,
    [BaseUrl] nvarchar(100) NOT NULL,
    [Priority] int NOT NULL DEFAULT 1,
    [TimeoutMs] int NOT NULL DEFAULT 2000,
    [IsEnabled] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Providers] PRIMARY KEY ([Id])
);

CREATE TABLE [Inquiries] (
    [Id] int NOT NULL IDENTITY,
    [ProductId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CompletedAt] datetime2 NULL,
    [Output] nvarchar(200) NULL,
    [ProviderId] int NULL,
    [InquiryStatus] int NOT NULL,
    CONSTRAINT [PK_Inquiries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Inquiries_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]),
    CONSTRAINT [FK_Inquiries_Providers_ProviderId] FOREIGN KEY ([ProviderId]) REFERENCES [Providers] ([Id])
);

CREATE TABLE [ProviderAttempts] (
    [Id] int NOT NULL IDENTITY,
    [InquiryId] int NOT NULL,
    [SharedInquiryId] int NULL,
    [ProviderId] int NOT NULL,
    [RequestStatus] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CompletedAt] datetime2 NULL,
    CONSTRAINT [PK_ProviderAttempts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProviderAttempts_Inquiries_InquiryId] FOREIGN KEY ([InquiryId]) REFERENCES [Inquiries] ([Id]),
    CONSTRAINT [FK_ProviderAttempts_Inquiries_SharedInquiryId] FOREIGN KEY ([SharedInquiryId]) REFERENCES [Inquiries] ([Id]),
    CONSTRAINT [FK_ProviderAttempts_Providers_ProviderId] FOREIGN KEY ([ProviderId]) REFERENCES [Providers] ([Id])
);

CREATE INDEX [IX_Inquiries_ProductId] ON [Inquiries] ([ProductId]);

CREATE INDEX [IX_Inquiries_ProviderId] ON [Inquiries] ([ProviderId]);

CREATE UNIQUE INDEX [IX_Products_HsCode] ON [Products] ([HsCode]);

CREATE INDEX [IX_ProviderAttempts_InquiryId] ON [ProviderAttempts] ([InquiryId]);

CREATE INDEX [IX_ProviderAttempts_ProviderId] ON [ProviderAttempts] ([ProviderId]);

CREATE INDEX [IX_ProviderAttempts_SharedInquiryId] ON [ProviderAttempts] ([SharedInquiryId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261016004608_init', N'10.0.13');

-- 200 HsCode az 100000 ta 100199
;WITH Numbers AS
(
    SELECT TOP (200)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
)
INSERT INTO dbo.Products (HsCode)
SELECT
    RIGHT('000000' + CAST(100000 + N - 1 AS VARCHAR(6)), 6)
FROM Numbers;

INSERT INTO dbo.Providers([Name], BaseUrl, [Priority], TimeoutMs, IsEnabled)
VALUES ('ProviderA', 'https://localhost:7021', 1, 2000, 1),
('ProviderB', 'https://localhost:7022', 2, 2000, 1);

COMMIT;
GO

