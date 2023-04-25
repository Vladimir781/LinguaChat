CREATE TABLE [dbo].[AspNetUserCredit] (
    [Id]              INT            IDENTITY(1,1) NOT NULL,
    [UserId]          NVARCHAR (450) NOT NULL,
    [TotalUsedTokens] INT            NULL,
    [CreditGranted]   INT            NULL,
    CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserCredit_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);