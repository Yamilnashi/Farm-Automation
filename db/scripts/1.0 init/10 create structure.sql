/* ---------------------------------------------------------------------- */
/* Script generated with: DeZign for Databases 14.6.5                     */
/* Target DBMS:           MS SQL Server 2022                              */
/* Project file:          farm-to-table.dez                               */
/* Project name:          farm-to-table                                   */
/* Author:                Yamil Font                                      */
/* Script type:           Database creation script                        */
/* Created on:            2025-08-06 01:16                                */
/* ---------------------------------------------------------------------- */


/* ---------------------------------------------------------------------- */
/* Add tables                                                             */
/* ---------------------------------------------------------------------- */

GO


/* ---------------------------------------------------------------------- */
/* Add table "dbo.Sentinel"                                               */
/* ---------------------------------------------------------------------- */

GO




CREATE TABLE [dbo].[Sentinel] (
    [SentinelId] INTEGER IDENTITY(1,1) NOT NULL,
    [PositionX] DECIMAL(5,2),
    [PositionY] DECIMAL(5,2),
    [SavedDate] DATETIME2 CONSTRAINT [DEF_Sentinel_SavedDate] DEFAULT sysdatetime() NOT NULL,
    CONSTRAINT [PK_Sentinel] PRIMARY KEY ([SentinelId])
)
GO




/* ---------------------------------------------------------------------- */
/* Add table "dbo.TemperatureReadingHistory"                              */
/* ---------------------------------------------------------------------- */

GO




CREATE TABLE [dbo].[TemperatureReadingHistory] (
    [TemperatureReadingHistoryId] INTEGER IDENTITY(1,1) NOT NULL,
    [SentinelId] INTEGER NOT NULL,
    [TemperatureCelsius] DECIMAL(3,1) NOT NULL,
    [SavedDate] DATETIME2 CONSTRAINT [DEF_TemperatureReadingHistory_SavedDate] DEFAULT sysdatetime() NOT NULL,
    CONSTRAINT [PK_TemperatureReadingHistory] PRIMARY KEY ([TemperatureReadingHistoryId])
)
GO




/* ---------------------------------------------------------------------- */
/* Add table "dbo.SentinelStatus"                                         */
/* ---------------------------------------------------------------------- */

GO




CREATE TABLE [dbo].[SentinelStatus] (
    [SentinelStatusCode] INTEGER NOT NULL,
    [SentinelStatusName] VARCHAR(40),
    CONSTRAINT [PK_SentinelStatus] PRIMARY KEY ([SentinelStatusCode])
)
GO




/* ---------------------------------------------------------------------- */
/* Add table "dbo.SoilReadingHistory"                                     */
/* ---------------------------------------------------------------------- */

GO




CREATE TABLE [dbo].[SoilReadingHistory] (
    [SoilReadingHistoryId] INTEGER IDENTITY(1,1) NOT NULL,
    [SentinelId] INTEGER NOT NULL,
    [NPpm] INTEGER NOT NULL,
    [PPpm] INTEGER NOT NULL,
    [KPpm] INTEGER NOT NULL,
    [SavedDate] DATETIME2 CONSTRAINT [DEF_SoilReadingHistory_SavedDate] DEFAULT sysdatetime() NOT NULL,
    CONSTRAINT [PK_SoilReadingHistory] PRIMARY KEY ([SoilReadingHistoryId])
)
GO




/* ---------------------------------------------------------------------- */
/* Add table "dbo.MoistureReadingHistory"                                 */
/* ---------------------------------------------------------------------- */

GO




CREATE TABLE [dbo].[MoistureReadingHistory] (
    [MoistureReadingHistoryId] INTEGER IDENTITY(1,1) NOT NULL,
    [SentinelId] INTEGER NOT NULL,
    [Moisture] TINYINT NOT NULL,
    [SavedDate] DATETIME2 CONSTRAINT [DEF_MoistureReadingHistory_SavedDate] DEFAULT sysdatetime() NOT NULL,
    CONSTRAINT [PK_MoistureReadingHistory] PRIMARY KEY ([MoistureReadingHistoryId])
)
GO




/* ---------------------------------------------------------------------- */
/* Add table "dbo.HistoryState"                                           */
/* ---------------------------------------------------------------------- */

GO




CREATE TABLE [dbo].[HistoryState] (
    [HistoryStateId] INTEGER NOT NULL,
    [LastTemperatureReadingLsn] BINARY(10),
    [LastMoistureReadingLsn] BINARY(10),
    [LastSoilReadingLsn] BINARY(10),
    [LastStatusLsn] BINARY(10),
    CONSTRAINT [PK_HistoryState] PRIMARY KEY ([HistoryStateId])
)
GO




/* ---------------------------------------------------------------------- */
/* Add table "dbo.SentinelStatusHistory"                                  */
/* ---------------------------------------------------------------------- */

GO




CREATE TABLE [dbo].[SentinelStatusHistory] (
    [SentinelStatusHistoryId] INTEGER IDENTITY(1,1) NOT NULL,
    [SentinelId] INTEGER,
    [SentinelStatusCode] INTEGER NOT NULL,
    [SavedDate] DATETIME2 CONSTRAINT [DEF_SentinelStatusHistory_SavedDate] DEFAULT sysdatetime() NOT NULL,
    CONSTRAINT [PK_SentinelStatusHistory] PRIMARY KEY ([SentinelStatusHistoryId])
)
GO




/* ---------------------------------------------------------------------- */
/* Add foreign key constraints                                            */
/* ---------------------------------------------------------------------- */

GO


ALTER TABLE [dbo].[TemperatureReadingHistory] ADD CONSTRAINT [Sentinel_TemperatureReadingHistory] 
    FOREIGN KEY ([SentinelId]) REFERENCES [dbo].[Sentinel] ([SentinelId])
GO


ALTER TABLE [dbo].[SentinelStatusHistory] ADD CONSTRAINT [Sentinel_SentinelStatusHistory] 
    FOREIGN KEY ([SentinelId]) REFERENCES [dbo].[Sentinel] ([SentinelId])
GO


ALTER TABLE [dbo].[SentinelStatusHistory] ADD CONSTRAINT [SentinelStatus_SentinelStatusHistory] 
    FOREIGN KEY ([SentinelStatusCode]) REFERENCES [dbo].[SentinelStatus] ([SentinelStatusCode])
GO


ALTER TABLE [dbo].[SoilReadingHistory] ADD CONSTRAINT [Sentinel_SoilReadingHistory] 
    FOREIGN KEY ([SentinelId]) REFERENCES [dbo].[Sentinel] ([SentinelId])
GO


ALTER TABLE [dbo].[MoistureReadingHistory] ADD CONSTRAINT [Sentinel_MoistureReadingHistory] 
    FOREIGN KEY ([SentinelId]) REFERENCES [dbo].[Sentinel] ([SentinelId])
GO

