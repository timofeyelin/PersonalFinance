IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;


BEGIN TRANSACTION;
CREATE TABLE [Categories] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [MonthlyBudget] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);

CREATE TABLE [ExpenseArticles] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [IsActive] bit NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ExpenseArticles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExpenseArticles_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Transactions] (
    [Id] uniqueidentifier NOT NULL,
    [Date] date NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Comment] nvarchar(250) NULL,
    [ArticleId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transactions_ExpenseArticles_ArticleId] FOREIGN KEY ([ArticleId]) REFERENCES [ExpenseArticles] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_ExpenseArticles_CategoryId] ON [ExpenseArticles] ([CategoryId]);

CREATE INDEX [IX_Transactions_ArticleId] ON [Transactions] ([ArticleId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260411104736_InitialCreate', N'10.0.5');

COMMIT;


BEGIN TRANSACTION;
ALTER TABLE [Transactions] ADD [Emoji] nvarchar(50) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260413195210_AddEmoji', N'10.0.5');

COMMIT;

