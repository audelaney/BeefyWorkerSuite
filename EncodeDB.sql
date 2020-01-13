USE [master]
IF EXISTS(SELECT 1 FROM master.dbo.sysdatabases WHERE name = 'encodedb')
BEGIN
	DROP database [encodedb]
END
GO

CREATE database [encodedb]
GO

USE [encodedb]
GO

-- Need a queue table with procedures for adding to the queue, checking out of the queue, and removing from the queue.

CREATE TABLE [dbo].[ProdEncodeJob](
	[JobId] [INT] IDENTITY (10000, 1) NOT NULL,
	[ConfigPresetId] [VARCHAR] (255) NOT NULL,
	[Priority] [INT] DEFAULT 2, --Max priority?
	[MaxAttemptsAllowed] [INT] DEFAULT 3,
	[Accuracy] [VARCHAR] (63) NOT NULL,
	[MinPsnr] [DECIMAL](2,2) NOT NULL,
	[InProcess] [BIT] DEFAULT 0,
	[Completed] [BIT] DEFAULT 0,
	[FileName] [NVARCHAR] (255) NOT NULL,
	[SourceSize] [BIGINT] NOT NULL --source file size
	
	CONSTRAINT [pk_JobId] PRIMARY KEY([JobId])
)

CREATE TABLE [dbo].[ProdEncodeRecord](
	[JobId] [INT] NOT NULL, --Foreign key to ProdEncodeJob table
	[AttemptNumber] [INT] NOT NULL,
	[OverallYPsnr] [DECIMAL] (2,2) NOT NULL,
	[AverageYPsnr] [DECIMAL] (2,2) NOT NULL,
	[ExecutionTimeMs] [INT] NOT NULL,
	[EncodeDate] [DATE] NOT NULL,
	[ConfigVariationFromJob] [NVARCHAR] (255),
	[CommitHash] [NVARCHAR] (255) NOT NULL,

	CONSTRAINT [pk_JobId_AttemptNumber] PRIMARY KEY ([JobId],[AttemptNumber]),
	CONSTRAINT [fk_JobId_ProdEncodeJob] FOREIGN KEY ([JobId]) REFERENCES [ProdEncodeJob]([JobId])
)

CREATE TABLE [dbo].[TestEncodeRecord](
	[RecordId] [INT] IDENTITY (40000, 1) NOT NULL,
	[ClipNum] [INT] NOT NULL,
	[AvgQp] [FLOAT] NOT NULL,
	[AvgYPsnr] [FLOAT] NOT NULL,
	[OverallYPsnr] [FLOAT] NOT NULL,
	[AverageSpeed] [FLOAT] NOT NULL,
	[OutputFileSize] [BIGINT] NOT NULL,
	[EncodingTimeMs] [INT] NOT NULL,
	[ExecutionTimeMs] [INT] NOT NULL,
	[EncodeDate] [DATE] NOT NULL,
	[ConfigFile] [NVARCHAR](255) NOT NULL,
	[CommandVariation] [NVARCHAR](255) NOT NULL,
	[Remarks] [NVARCHAR](255) NOT NULL,
	[CommitHash] [NVARCHAR](255) NOT NULL,
	[Machine] [NVARCHAR](255) NOT NULL,
	
	CONSTRAINT [pk_RecordId] PRIMARY KEY([RecordId])
)

GO
CREATE PROCEDURE [dbo].[add_test_encode_record]
	(
		@ClipNum [INT],
		@AvgQp [FLOAT],
		@AvgYPsnr [FLOAT],
		@OverallYPsnr [FLOAT],
		@AverageSpeed [FLOAT],
		@OutputFileSize [BIGINT],
		@EncodingTimeMs [VARCHAR](63),
		@ExecutionTimeMs [VARCHAR](63),
		@EncodeDate [DATE],
		@ConfigFile [NVARCHAR](255),
		@CommandVariation [NVARCHAR](255),
		@CommitHash [NVARCHAR](255),
		@Machine [NVARCHAR](255)
	)
AS
	BEGIN
		INSERT INTO [dbo].[TestEncodeRecord]
			   ([ClipNum]
			   ,[AvgQp]
			   ,[AvgYPsnr]
			   ,[OverallYPsnr]
			   ,[AverageSpeed]
			   ,[OutputFileSize]
			   ,[EncodingTimeMs]
			   ,[ExecutionTimeMs]
			   ,[EncodeDate]
			   ,[ConfigFile]
			   ,[CommandVariation]
			   ,[CommitHash]
			   ,[Machine])
		 VALUES
			   (@ClipNum
			   ,@AvgQp
			   ,@AvgYPsnr
			   ,@OverallYPsnr
			   ,@AverageSpeed
			   ,@OutputFileSize
			   ,@EncodingTimeMs
			   ,@ExecutionTimeMs
			   ,@EncodeDate
			   ,@ConfigFile
			   ,@CommandVariation
			   ,@CommitHash
			   ,@Machine)
	END
GO
CREATE PROCEDURE [dbo].[add_test_encode_remarks]
	(
		@RecordId [INT],--encode id
		@Remarks [NVARCHAR] (255)--remarks
	)
AS
	BEGIN
		UPDATE [TestEncodeRecord]
		SET [Remarks] = @Remarks
		WHERE [RecordId] = @RecordId
	END
GO
CREATE PROCEDURE [dbo].[get_test_encode_records]
AS
	BEGIN
		SELECT [RecordId]
				,[ClipNum]
				,[AvgQp]
				,[AvgYPsnr]
				,[OverallYPsnr]
				,[AverageSpeed]
				,[OutputFileSize]
				,[EncodingTimeMs]
				,[ExecutionTimeMs]
				,[EncodeDate]
				,[ConfigFile]
				,[CommandVariation]
				,[Remarks]
				,[CommitHash]
				,[Machine]
		  FROM [dbo].[TestEncodeRecord]
	END
GO