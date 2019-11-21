CREATE TABLE [dbo].[Participant] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [BlindTestId]  INT            NOT NULL,
    [Name]         NVARCHAR (100) NOT NULL,
    [ImageIndex]   NVARCHAR (50)  NULL,
    [ConnectionId] NVARCHAR (100) NULL,
    [Created]      DATETIME       CONSTRAINT [DF_Person_Created] DEFAULT (getdate()) NULL,
    [Updated]      DATETIME       CONSTRAINT [DF_Participant_Updated] DEFAULT (getdate()) NULL,
    [UpdateReason] NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Participant] PRIMARY KEY CLUSTERED ([Id] ASC)
);



