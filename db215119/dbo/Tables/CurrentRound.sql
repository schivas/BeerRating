CREATE TABLE [dbo].[CurrentRound] (
    [BlindTestId]   INT             NOT NULL,
    [ParticipantId] INT             NOT NULL,
    [Vote]          DECIMAL (18, 1) NOT NULL,
    [Created]       DATETIME        CONSTRAINT [DF_CurrentRound_Created] DEFAULT (getdate()) NOT NULL,
    [Updated]       DATETIME        CONSTRAINT [DF_CurrentRound_Updated] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_CurrentRound] PRIMARY KEY CLUSTERED ([BlindTestId] ASC, [ParticipantId] ASC)
);

