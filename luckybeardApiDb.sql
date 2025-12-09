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
GO

CREATE TABLE [ToDoUsers] (
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Surname] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ToDoUsers] PRIMARY KEY ([UserId])
);
GO

CREATE TABLE [ToDos] (
    [ToDoId] nvarchar(450) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [ToDoDescription] nvarchar(max) NOT NULL,
    [Created_At] datetime2 NOT NULL,
    [Updated_At] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    [Status] tinyint NOT NULL,
    [UserToDoId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_ToDos] PRIMARY KEY ([ToDoId]),
    CONSTRAINT [FK_ToDos_ToDoUsers_UserToDoId] FOREIGN KEY ([UserToDoId]) REFERENCES [ToDoUsers] ([UserId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ToDos_UserToDoId] ON [ToDos] ([UserToDoId]);
GO

CREATE UNIQUE INDEX [IX_ToDoUsers_Email] ON [ToDoUsers] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251208211506_firstMigration', N'8.0.22');
GO

COMMIT;
GO

