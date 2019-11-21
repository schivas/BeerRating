CREATE TABLE [dbo].[BlindTest] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [Name]                   NVARCHAR (100) NOT NULL,
    [IsCompleted]            BIT            CONSTRAINT [DF_BlindTest_Completed] DEFAULT ((0)) NOT NULL,
    [AllowAdHocParticipants] BIT            CONSTRAINT [DF_BlindTest_AllowNewUsersAsWeGo] DEFAULT ((0)) NULL,
    [Created]                DATETIME       CONSTRAINT [DF_BlindTest_Started] DEFAULT (getdate()) NULL,
    [Updated]                DATETIME       CONSTRAINT [DF_BlindTest_Updated] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BlindTest] PRIMARY KEY CLUSTERED ([Id] ASC)
);

