USE [master]
GO
/****** Object:  Database [TestingSystem]    Script Date: 20.02.2014 11:21:18 ******/
CREATE DATABASE [TestingSystem]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TestingSystem', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\TestingSystem.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TestingSystem_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\TestingSystem_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [TestingSystem] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TestingSystem].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TestingSystem] SET ANSI_NULL_DEFAULT ON 
GO
ALTER DATABASE [TestingSystem] SET ANSI_NULLS ON 
GO
ALTER DATABASE [TestingSystem] SET ANSI_PADDING ON 
GO
ALTER DATABASE [TestingSystem] SET ANSI_WARNINGS ON 
GO
ALTER DATABASE [TestingSystem] SET ARITHABORT ON 
GO
ALTER DATABASE [TestingSystem] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TestingSystem] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [TestingSystem] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TestingSystem] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TestingSystem] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TestingSystem] SET CURSOR_DEFAULT  LOCAL 
GO
ALTER DATABASE [TestingSystem] SET CONCAT_NULL_YIELDS_NULL ON 
GO
ALTER DATABASE [TestingSystem] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TestingSystem] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [TestingSystem] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TestingSystem] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TestingSystem] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TestingSystem] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TestingSystem] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TestingSystem] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TestingSystem] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TestingSystem] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TestingSystem] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TestingSystem] SET RECOVERY FULL 
GO
ALTER DATABASE [TestingSystem] SET  MULTI_USER 
GO
ALTER DATABASE [TestingSystem] SET PAGE_VERIFY NONE  
GO
ALTER DATABASE [TestingSystem] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TestingSystem] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TestingSystem] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [TestingSystem]
GO
/****** Object:  UserDefinedTableType [dbo].[ResultsRow]    Script Date: 20.02.2014 11:21:18 ******/
CREATE TYPE [dbo].[ResultsRow] AS TABLE(
	[UserID] [int] NOT NULL,
	[TestID] [int] NOT NULL,
	[QuestionID] [int] NOT NULL,
	[Passed] [bit] NOT NULL DEFAULT ((0))
)
GO
/****** Object:  StoredProcedure [dbo].[GetOrCreateUser]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetOrCreateUser]
	@userName nvarchar(100)
AS
BEGIN
	DECLARE @user TABLE(
		UserID int NOT NULL,
		Name nvarchar(100) NOT NULL
	);
	INSERT INTO @user
	SELECT TOP(1) UserID, Name
	FROM [dbo].Users
	WHERE Name = @userName;


	IF (SELECT 1 FROM @user) IS NULL
		INSERT INTO [dbo].Users 
			OUTPUT INSERTED.UserID, INSERTED.Name
				INTO @user
		VALUES (@userName);

	SELECT TOP(1) UserID FROM @user;
END
GO
/****** Object:  StoredProcedure [dbo].[GetTestData]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetTestData]
	@testID int = 1
AS
BEGIN
	SELECT Test.TestID,
	Test.Title as TestTitle,
	Quest.QuestionID,
	Quest.[Text] as QuestionText,
	QuestType.Name as QuestionType,
	Optn.OptionID,
	Optn.[Text] as OptionText,
	Q_O.IsAnswer as [IsAsnwer]
	FROM [dbo].[Tests] as Test
	INNER JOIN [dbo].[Tests_Questions] as T_Q
	ON T_Q.TestID = Test.TestID
	INNER JOIN [dbo].Questions as Quest
	ON T_Q.QuestionID = Quest.QuestionID
	INNER JOIN [dbo].QuestionTypes as QuestType
	ON Quest.QuestionTypeID = QuestType.QuestionTypeID
	INNER JOIN [dbo].Questions_Options as Q_O
	ON Q_O.QuestionID = Quest.QuestionID
	INNER JOIN [dbo].Options as Optn
	ON Q_O.OptionID = Optn.OptionID
	WHERE Test.TestID = @testID
END
GO
/****** Object:  StoredProcedure [dbo].[GetTestingStats]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetTestingStats]
AS
BEGIN
	SELECT  Test.TestID,
	Test.Title,
	RightAnswrs.Answrs,
	Count(1) as RightAnswrsCount
	FROM [dbo].Tests as Test
	INNER JOIN (
		SELECT Res.TestID,
		Res.UserID,
		ROUND(SUM(CAST(Res.Passed AS INT)) / CAST(COUNT(1) as REAL) * 100, 0) as Answrs
		FROM [dbo].Results as Res
		GROUP BY Res.TestID, 
		Res.UserID
		) as RightAnswrs
	ON RightAnswrs.TestID = Test.TestID
	GROUP BY Test.TestID, 
	Test.Title,
	RightAnswrs.Answrs;
END
GO
/****** Object:  StoredProcedure [dbo].[GetTests]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetTests]
AS
BEGIN
	SELECT TestID,
	Title
	FROM [dbo].[Tests]
END
GO
/****** Object:  StoredProcedure [dbo].[GetUserTestingResults]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserTestingResults]
	@userID int,
	@testID int
AS
BEGIN
	declare @userResults TABLE (
		Passed bit NOT NULL
	);		
	INSERT INTO @userResults
	SELECT Result.Passed
	FROM [dbo].Results as Result
	INNER JOIN [dbo].Tests as Test
	ON Test.TestID = Result.TestID
	WHERE Result.UserID = @userID AND
	Result.TestID = @testID;

	SELECT (
		SELECT COUNT(1) 
		FROM @userResults
		WHERE Passed = 1
		) as Passed,
		(
		SELECT COUNT(1)
		FROM @userResults
		) as [Count]
END
GO
/****** Object:  StoredProcedure [dbo].[SaveUserResults]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaveUserResults]
	@userID int,
	@testID int,
	@resultRows [dbo].[ResultsRow] READONLY
AS
BEGIN
	
	DELETE FROM [dbo].Results
	WHERE UserID = @userID AND TestID = @testID;
	
	INSERT INTO [dbo].[Results]
	SELECT UserID, TestID, QuestionID, Passed
	FROM @resultRows;
END
GO
/****** Object:  Table [dbo].[Options]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Options](
	[OptionID] [int] IDENTITY(1,1) NOT NULL,
	[Text] [ntext] NOT NULL,
 CONSTRAINT [PK_Options] PRIMARY KEY CLUSTERED 
(
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Questions]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Questions](
	[QuestionID] [int] IDENTITY(1,1) NOT NULL,
	[Text] [ntext] NOT NULL,
	[QuestionTypeID] [int] NOT NULL,
 CONSTRAINT [PK__Question__0DC06F8CEABD90A7] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Questions_Options]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Questions_Options](
	[QuestionID] [int] NOT NULL,
	[OptionID] [int] NOT NULL,
	[IsAnswer] [bit] NOT NULL,
 CONSTRAINT [PK_Questions_Options] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC,
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestionTypes]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionTypes](
	[QuestionTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Question__7EDFF91151DED2ED] PRIMARY KEY CLUSTERED 
(
	[QuestionTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Results]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Results](
	[UserID] [int] NOT NULL,
	[TestID] [int] NOT NULL,
	[QuestionID] [int] NOT NULL,
	[Passed] [bit] NOT NULL,
 CONSTRAINT [PK_Results] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[TestID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tests]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tests](
	[TestID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NOT NULL,
 CONSTRAINT [PK__Tests__8CC33100A7EE11CD] PRIMARY KEY CLUSTERED 
(
	[TestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [dbo].[Tests_Questions]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tests_Questions](
	[TestID] [int] NOT NULL,
	[QuestionID] [int] NOT NULL,
 CONSTRAINT [PK_Tests_Questions] PRIMARY KEY CLUSTERED 
(
	[TestID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 20.02.2014 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK__Users__1788CCACA6ADBF80] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Options] ON 

INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (1, N'2')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (2, N'4')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (3, N'8')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (4, N'16')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (5, N'64')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (6, N'2 + 2 = 5')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (7, N'2 + 2 = 4')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (8, N'Order of matrix multiplication does not make a sense.')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (9, N'NOT true AND NOT FALSE equals false')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (10, N'device management')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (11, N'process management')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (12, N'memory management')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (13, N'creation of text files.')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (14, N'programming.')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (15, N'C:\\my_folder\document.txt')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (16, N'document.txt')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (17, N'documents\document.txt')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (18, N'4')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (19, N'bits per second')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (20, N'bytes per minute')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (21, N'words per minute')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (22, N'symbols per second')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (23, N'8')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (24, N'7')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (25, N'6')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (26, N'5')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (27, N'an IP adrress')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (28, N'a WEB page')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (29, N'home WEB page')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (30, N'domain name')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (31, N'local, regional, global')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (32, N'terminal, administrative, mixed')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (33, N'wired, wireless')
INSERT [dbo].[Options] ([OptionID], [Text]) VALUES (34, N'digital, commercial, corporative')
SET IDENTITY_INSERT [dbo].[Options] OFF
SET IDENTITY_INSERT [dbo].[Questions] ON 

INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (1, N'Consider the following recursive function,<br/><code>int Fun(int n)<br/><br/>&nbsp;if (n == 4)<br/>&nbsp;&nbsp;return 2;<br/>&nbsp;else<br/>&nbsp;&nbsp;return 2 * Fun(n + 1);<br/>}<br/></code>What is the value returned by the function call <code>Fun(2)</code> ?', 1)
INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (2, N'Which of the following statements are true?', 2)
INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (3, N'What is the value of the equation <code> (101011 ^ 11) >> 3 </code> in the decimal numeral system?', 3)
INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (4, N'An operating system performs:', 2)
INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (5, N'Which of the folowing file paths is absolute?', 1)
INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (6, N'The speed of information transfer over network is measured in...', 1)
INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (7, N'In the OSI model all network functions are divided in ... levels.', 1)
INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (8, N'Connected to the Internet network computer always has ...', 1)
INSERT [dbo].[Questions] ([QuestionID], [Text], [QuestionTypeID]) VALUES (9, N'Computer networks by their sizes are divided in...', 1)
SET IDENTITY_INSERT [dbo].[Questions] OFF
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (1, 1, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (1, 2, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (1, 3, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (1, 4, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (1, 5, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (2, 6, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (2, 7, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (2, 8, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (2, 9, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (3, 18, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (4, 10, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (4, 11, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (4, 12, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (4, 13, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (4, 14, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (5, 15, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (5, 16, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (5, 17, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (6, 19, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (6, 20, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (6, 21, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (6, 22, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (7, 23, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (7, 24, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (7, 25, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (7, 26, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (8, 27, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (8, 28, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (8, 29, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (8, 30, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (9, 31, 1)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (9, 32, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (9, 33, 0)
INSERT [dbo].[Questions_Options] ([QuestionID], [OptionID], [IsAnswer]) VALUES (9, 34, 0)
SET IDENTITY_INSERT [dbo].[QuestionTypes] ON 

INSERT [dbo].[QuestionTypes] ([QuestionTypeID], [Name]) VALUES (1, N'Radio')
INSERT [dbo].[QuestionTypes] ([QuestionTypeID], [Name]) VALUES (2, N'Select')
INSERT [dbo].[QuestionTypes] ([QuestionTypeID], [Name]) VALUES (3, N'Text')
SET IDENTITY_INSERT [dbo].[QuestionTypes] OFF
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 1, 1, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 1, 2, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 1, 3, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 1, 4, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 1, 5, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 2, 6, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 2, 7, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 2, 8, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (1, 2, 9, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 1, 1, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 1, 2, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 1, 3, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 1, 4, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 1, 5, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 2, 6, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 2, 7, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 2, 8, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (2, 2, 9, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 1, 1, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 1, 2, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 1, 3, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 1, 4, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 1, 5, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 2, 6, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 2, 7, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 2, 8, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (3, 2, 9, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 1, 1, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 1, 2, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 1, 3, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 1, 4, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 1, 5, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 2, 6, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 2, 7, 1)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 2, 8, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (4, 2, 9, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (5, 2, 6, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (5, 2, 7, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (5, 2, 8, 0)
INSERT [dbo].[Results] ([UserID], [TestID], [QuestionID], [Passed]) VALUES (5, 2, 9, 0)
SET IDENTITY_INSERT [dbo].[Tests] ON 

INSERT [dbo].[Tests] ([TestID], [Title]) VALUES (1, N'Informatics')
INSERT [dbo].[Tests] ([TestID], [Title]) VALUES (2, N'Networks')
SET IDENTITY_INSERT [dbo].[Tests] OFF
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (1, 1)
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (1, 2)
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (1, 3)
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (1, 4)
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (1, 5)
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (2, 6)
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (2, 7)
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (2, 8)
INSERT [dbo].[Tests_Questions] ([TestID], [QuestionID]) VALUES (2, 9)
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserID], [Name]) VALUES (1, N'User')
INSERT [dbo].[Users] ([UserID], [Name]) VALUES (2, N'User2')
INSERT [dbo].[Users] ([UserID], [Name]) VALUES (3, N'User3')
INSERT [dbo].[Users] ([UserID], [Name]) VALUES (4, N'user4')
INSERT [dbo].[Users] ([UserID], [Name]) VALUES (5, N'user5')
SET IDENTITY_INSERT [dbo].[Users] OFF
ALTER TABLE [dbo].[Options] ADD  CONSTRAINT [DF_Options_Text]  DEFAULT (N'New option') FOR [Text]
GO
ALTER TABLE [dbo].[Questions] ADD  CONSTRAINT [DF__Questions__Text__2B3F6F97]  DEFAULT ('Question text') FOR [Text]
GO
ALTER TABLE [dbo].[Questions_Options] ADD  CONSTRAINT [DF_Questions_Options_IsAnswer]  DEFAULT ((0)) FOR [IsAnswer]
GO
ALTER TABLE [dbo].[QuestionTypes] ADD  CONSTRAINT [DF__QuestionTy__Name__2F10007B]  DEFAULT (N'New type') FOR [Name]
GO
ALTER TABLE [dbo].[Results] ADD  CONSTRAINT [DF_Results_Passed]  DEFAULT ((0)) FOR [Passed]
GO
ALTER TABLE [dbo].[Tests] ADD  CONSTRAINT [DF__Tests__Title__25869641]  DEFAULT (N'Test title') FOR [Title]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__Name__286302EC]  DEFAULT (N'New user') FOR [Name]
GO
ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_QuestionTypes] FOREIGN KEY([QuestionTypeID])
REFERENCES [dbo].[QuestionTypes] ([QuestionTypeID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_QuestionTypes]
GO
ALTER TABLE [dbo].[Questions_Options]  WITH CHECK ADD  CONSTRAINT [FK_Questions_Options_Options] FOREIGN KEY([OptionID])
REFERENCES [dbo].[Options] ([OptionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Questions_Options] CHECK CONSTRAINT [FK_Questions_Options_Options]
GO
ALTER TABLE [dbo].[Questions_Options]  WITH CHECK ADD  CONSTRAINT [FK_Questions_Options_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Questions_Options] CHECK CONSTRAINT [FK_Questions_Options_Questions]
GO
ALTER TABLE [dbo].[Results]  WITH CHECK ADD  CONSTRAINT [FK_Results_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Results] CHECK CONSTRAINT [FK_Results_Questions]
GO
ALTER TABLE [dbo].[Results]  WITH CHECK ADD  CONSTRAINT [FK_Results_Tests] FOREIGN KEY([TestID])
REFERENCES [dbo].[Tests] ([TestID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Results] CHECK CONSTRAINT [FK_Results_Tests]
GO
ALTER TABLE [dbo].[Results]  WITH CHECK ADD  CONSTRAINT [FK_Results_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Results] CHECK CONSTRAINT [FK_Results_Users]
GO
ALTER TABLE [dbo].[Tests_Questions]  WITH CHECK ADD  CONSTRAINT [FK_Tests_Questions_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Tests_Questions] CHECK CONSTRAINT [FK_Tests_Questions_Questions]
GO
ALTER TABLE [dbo].[Tests_Questions]  WITH CHECK ADD  CONSTRAINT [FK_Tests_Questions_Tests] FOREIGN KEY([TestID])
REFERENCES [dbo].[Tests] ([TestID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Tests_Questions] CHECK CONSTRAINT [FK_Tests_Questions_Tests]
GO
USE [master]
GO
ALTER DATABASE [TestingSystem] SET  READ_WRITE 
GO
