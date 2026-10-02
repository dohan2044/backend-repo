SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF SCHEMA_ID(N'jcodepro_journey_of_faith') IS NULL
        THROW 50001, N'Schema [jcodepro_journey_of_faith] does not exist.', 1;

    IF OBJECT_ID(N'[jcodepro_journey_of_faith].[Church]', N'U') IS NULL
        THROW 50002, N'Table [jcodepro_journey_of_faith].[Church] does not exist.', 1;

    IF OBJECT_ID(N'[jcodepro_journey_of_faith].[ChurchImages]', N'U') IS NULL
    BEGIN
        CREATE TABLE [jcodepro_journey_of_faith].[ChurchImages]
        (
            [Id]          INT              IDENTITY(1, 1) NOT NULL,
            [ChurchId]    INT              NOT NULL,
            [ImageName]   NVARCHAR(MAX)    NULL,
            [CreatedUser] UNIQUEIDENTIFIER NOT NULL,
            [CreatedAt]   DATETIME2        NOT NULL,

            CONSTRAINT [PK_ChurchImages]
                PRIMARY KEY ([Id]),

            CONSTRAINT [FK_ChurchImages_Church_ChurchId]
                FOREIGN KEY ([ChurchId])
                REFERENCES [jcodepro_journey_of_faith].[Church] ([Id])
                ON DELETE CASCADE
        );

        CREATE INDEX [IX_ChurchImages_ChurchId]
            ON [jcodepro_journey_of_faith].[ChurchImages] ([ChurchId]);

        PRINT N'Created table [jcodepro_journey_of_faith].[ChurchImages].';
    END
    ELSE
    BEGIN
        PRINT N'Table [jcodepro_journey_of_faith].[ChurchImages] already exists; no changes were made.';
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
