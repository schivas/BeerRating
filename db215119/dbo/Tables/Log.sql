CREATE TABLE [dbo].[Log] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [BlindTestId]   INT             NULL,
    [ParticipantId] INT             NULL,
    [Vote]          DECIMAL (18, 1) NULL,
    [Kilde]         NVARCHAR (50)   NULL,
    [Updated]       DATETIME        CONSTRAINT [DF_Log_Updated] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC)
);

