CREATE TABLE [dbo].[Round] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [BlindTestId]   INT             NOT NULL,
    [ParticipantId] INT             NOT NULL,
    [RoundNo]       INT             NOT NULL,
    [BrandName]     NVARCHAR (100)  NOT NULL,
    [ABV]           DECIMAL (18, 1) NOT NULL,
    [Vote]          DECIMAL (18, 1) CONSTRAINT [DF_Round_Vote] DEFAULT ((3.5)) NULL,
    [Overridden]    INT             CONSTRAINT [DF_Round_Overriden] DEFAULT ((0)) NULL,
    [Created]       DATETIME        CONSTRAINT [DF_Round_Created] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Round] PRIMARY KEY CLUSTERED ([Id] ASC)
);

