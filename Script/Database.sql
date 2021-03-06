USE [teamx_x_sms]
GO
/****** Object:  Schema [Detail]    Script Date: 6/24/2018 12:24:36 AM ******/
CREATE SCHEMA [Detail]
GO
/****** Object:  Schema [Game]    Script Date: 6/24/2018 12:24:36 AM ******/
CREATE SCHEMA [Game]
GO
/****** Object:  Schema [Master]    Script Date: 6/24/2018 12:24:37 AM ******/
CREATE SCHEMA [Master]
GO
/****** Object:  Schema [Transcation]    Script Date: 6/24/2018 12:24:37 AM ******/
CREATE SCHEMA [Transcation]
GO
/****** Object:  Table [Master].[Sector]    Script Date: 6/24/2018 12:24:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Master].[Sector](
	[SectorId] [int] IDENTITY(1,1) NOT NULL,
	[SectorName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_master_Sector] PRIMARY KEY CLUSTERED 
(
	[SectorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Master].[Stock]    Script Date: 6/24/2018 12:24:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Master].[Stock](
	[StockId] [int] IDENTITY(1,1) NOT NULL,
	[SectorId] [int] NOT NULL,
	[StockName] [nvarchar](100) NOT NULL,
	[StartingPrice] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_master_Stock] PRIMARY KEY CLUSTERED 
(
	[StockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Game].[Player]    Script Date: 6/24/2018 12:24:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Game].[Player](
	[PlayerId] [int] IDENTITY(1,1) NOT NULL,
	[PlayerName] [nvarchar](100) NOT NULL,
	[SecondaryName] [nvarchar](100) NULL,
	[ConnectionId] [nvarchar](100) NOT NULL,
	[GameId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_detail_Player] PRIMARY KEY CLUSTERED 
(
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Detail].[Transcation]    Script Date: 6/24/2018 12:24:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Detail].[Transcation](
	[TransactionId] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[IsWithdraw] [bit] NOT NULL,
	[IsDeposit] [bit] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_trns_Transaction] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Detail].[PlayerStock]    Script Date: 6/24/2018 12:24:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Detail].[PlayerStock](
	[PlayerStockId] [int] IDENTITY(1,1) NOT NULL,
	[StockId] [int] NOT NULL,
	[TransactionId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Detail.PlayerStock] PRIMARY KEY CLUSTERED 
(
	[PlayerStockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [Detail].[ViewPlayerPortfolio]    Script Date: 6/24/2018 12:24:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [Detail].[ViewPlayerPortfolio]
AS
SELECT Game.Player.GameId, Detail.PlayerStock.PlayerId, Game.Player.PlayerName, Game.Player.IsActive, Detail.PlayerStock.StockId, Master.Stock.StockName,Master.Sector.SectorId,Master.Sector.SectorName,Master.Stock.StartingPrice, Detail.Transcation.IsWithdraw, 
Detail.Transcation.IsDeposit, Detail.PlayerStock.Quantity, Detail.PlayerStock.UnitPrice, Detail.Transcation.Amount
FROM Detail.Transcation LEFT OUTER JOIN
    Master.Stock RIGHT OUTER JOIN
    Detail.PlayerStock ON Master.Stock.StockId = Detail.PlayerStock.StockId
	RIGHT OUTER JOIN Master.Sector ON Master.Stock.SectorId = Master.Sector.SectorId 
	LEFT OUTER JOIN
    Game.Player ON Detail.PlayerStock.PlayerId = Game.Player.PlayerId
	 ON Detail.Transcation.TransactionId = Detail.PlayerStock.TransactionId
GO
/****** Object:  Table [Detail].[BankAccount]    Script Date: 6/24/2018 12:24:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Detail].[BankAccount](
	[AccountId] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[AccountName] [nvarchar](100) NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_detail_Bank_1] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Detail].[GameDetails]    Script Date: 6/24/2018 12:24:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Detail].[GameDetails](
	[GameId] [int] NOT NULL,
	[Details] [nvarchar](max) NULL,
 CONSTRAINT [PK_GameDetails] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Game].[Game]    Script Date: 6/24/2018 12:24:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Game].[Game](
	[GameId] [int] IDENTITY(1,1) NOT NULL,
	[GameCode] [nvarchar](50) NOT NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[CreatedPlayer] [nvarchar](50) NOT NULL,
	[PlayersCount] [int] NOT NULL,
	[IsPublic] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsStarted] [bit] NOT NULL,
	[IsCanceled] [bit] NOT NULL,
	[Winner] [varchar](50) NULL,
 CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Master].[Event]    Script Date: 6/24/2018 12:24:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Master].[Event](
	[EventId] [int] IDENTITY(1,1) NOT NULL,
	[EventName] [nvarchar](100) NOT NULL,
	[MinEffect] [int] NOT NULL,
	[MaxEffect] [int] NOT NULL,
	[IsSector] [bit] NOT NULL,
	[IsStock] [bit] NOT NULL,
	[MinDuration] [int] NOT NULL,
	[MaxDuration] [int] NOT NULL,
	[Probability] [float] NOT NULL,
	[Chance] [int] NOT NULL,
 CONSTRAINT [PK_master_Event] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Master].[Trend]    Script Date: 6/24/2018 12:24:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Master].[Trend](
	[TrendId] [int] IDENTITY(1,1) NOT NULL,
	[TrendName] [nvarchar](100) NOT NULL,
	[MaxEffect] [int] NOT NULL,
	[MinEffect] [int] NOT NULL,
 CONSTRAINT [PK_master_Trend] PRIMARY KEY CLUSTERED 
(
	[TrendId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Detail].[Transcation] ADD  CONSTRAINT [DF_Transcation_IsWithdraw]  DEFAULT ((0)) FOR [IsWithdraw]
GO
ALTER TABLE [Detail].[Transcation] ADD  CONSTRAINT [DF_Transcation_IsDeposit]  DEFAULT ((0)) FOR [IsDeposit]
GO
ALTER TABLE [Game].[Game] ADD  CONSTRAINT [DF_Game_PlayersCount]  DEFAULT ((0)) FOR [PlayersCount]
GO
ALTER TABLE [Game].[Game] ADD  CONSTRAINT [DF_Game_IsPublic]  DEFAULT ((0)) FOR [IsPublic]
GO
ALTER TABLE [Game].[Game] ADD  CONSTRAINT [DF_Game_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [Game].[Game] ADD  CONSTRAINT [DF_Game_IsStarted]  DEFAULT ((0)) FOR [IsStarted]
GO
ALTER TABLE [Game].[Game] ADD  CONSTRAINT [DF_Game_IsCanceled]  DEFAULT ((0)) FOR [IsCanceled]
GO
ALTER TABLE [Master].[Event] ADD  DEFAULT ((0)) FOR [Chance]
GO
ALTER TABLE [Detail].[BankAccount]  WITH CHECK ADD  CONSTRAINT [FK_BankAccount_Player] FOREIGN KEY([PlayerId])
REFERENCES [Game].[Player] ([PlayerId])
GO
ALTER TABLE [Detail].[BankAccount] CHECK CONSTRAINT [FK_BankAccount_Player]
GO
ALTER TABLE [Detail].[PlayerStock]  WITH CHECK ADD  CONSTRAINT [FK_Detail.PlayerStock_Player] FOREIGN KEY([PlayerId])
REFERENCES [Game].[Player] ([PlayerId])
GO
ALTER TABLE [Detail].[PlayerStock] CHECK CONSTRAINT [FK_Detail.PlayerStock_Player]
GO
ALTER TABLE [Detail].[PlayerStock]  WITH CHECK ADD  CONSTRAINT [FK_Detail.PlayerStock_Stock] FOREIGN KEY([StockId])
REFERENCES [Master].[Stock] ([StockId])
GO
ALTER TABLE [Detail].[PlayerStock] CHECK CONSTRAINT [FK_Detail.PlayerStock_Stock]
GO
ALTER TABLE [Detail].[PlayerStock]  WITH CHECK ADD  CONSTRAINT [FK_Detail.PlayerStock_Transcation] FOREIGN KEY([TransactionId])
REFERENCES [Detail].[Transcation] ([TransactionId])
GO
ALTER TABLE [Detail].[PlayerStock] CHECK CONSTRAINT [FK_Detail.PlayerStock_Transcation]
GO
ALTER TABLE [Detail].[Transcation]  WITH CHECK ADD  CONSTRAINT [FK_Account] FOREIGN KEY([AccountId])
REFERENCES [Detail].[BankAccount] ([AccountId])
GO
ALTER TABLE [Detail].[Transcation] CHECK CONSTRAINT [FK_Account]
GO
ALTER TABLE [Game].[Player]  WITH CHECK ADD  CONSTRAINT [FK_Game] FOREIGN KEY([GameId])
REFERENCES [Game].[Game] ([GameId])
GO
ALTER TABLE [Game].[Player] CHECK CONSTRAINT [FK_Game]
GO
ALTER TABLE [Master].[Stock]  WITH CHECK ADD  CONSTRAINT [FK_Sector] FOREIGN KEY([SectorId])
REFERENCES [Master].[Sector] ([SectorId])
GO
ALTER TABLE [Master].[Stock] CHECK CONSTRAINT [FK_Sector]
GO
/****** Object:  StoredProcedure [dbo].[BuyStocks]    Script Date: 6/24/2018 12:24:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BuyStocks]
	@playerID int,
	@playerAccID int,
	@quantity int,
	@stockID int,
	@price decimal(18,2)
AS
BEGIN
	SET NOCOUNT ON;

    UPDATE Detail.BankAccount SET Balance = Balance - (@price * @quantity) WHERE AccountId = @playerAccID;
	------------------------------------------------------------------------------------------------------------
	DECLARE @IDsTable1 TABLE(ID int);								--buyer

	INSERT INTO Detail.Transcation									--insert to trans table
		OUTPUT INSERTED.TransactionId INTO @IDsTable1(ID)			--get the auto incremented id
	VALUES (@playerAccID, 1, 0, (@price * @quantity));
														--insert into playerstocks using that trans id
	INSERT INTO Detail.PlayerStock VALUES (@stockID, (SELECT ID FROM @IDsTable1), @playerID, @quantity, @price);

	DELETE FROM @IDsTable1;

END
GO
/****** Object:  StoredProcedure [dbo].[SellStocks]    Script Date: 6/24/2018 12:24:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SellStocks]
	@playerID int,
	@playerAccID int,
	@quantity int,
	@stockID int,
	@price decimal(18,2)
AS
BEGIN
	SET NOCOUNT ON;

    UPDATE Detail.BankAccount SET Balance = Balance + (@price * @quantity) WHERE AccountId = @playerAccID;
	------------------------------------------------------------------------------------------------------------
	DEClare @IDsTable2 TABLE(ID int);								--seller

	INSERT INTO Detail.Transcation									--insert to trans table
		OUTPUT INSERTED.TransactionId INTO @IDsTable2(ID)			--get the auto incremented id
	VALUES (@playerAccID, 0, 1, (@price * @quantity));
																	--insert into playerstocks using that trans id
	INSERT INTO Detail.PlayerStock VALUES (@stockID, (SELECT ID FROM @IDsTable2), @playerID, -1*@quantity, @price);

	DELETE FROM @IDsTable2;

END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[47] 2[4] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Transcation (Detail)"
            Begin Extent = 
               Top = 11
               Left = 485
               Bottom = 141
               Right = 655
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "Stock (Master)"
            Begin Extent = 
               Top = 23
               Left = 6
               Bottom = 153
               Right = 176
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PlayerStock (Detail)"
            Begin Extent = 
               Top = 16
               Left = 225
               Bottom = 146
               Right = 395
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "Player (Game)"
            Begin Extent = 
               Top = 155
               Left = 451
               Bottom = 285
               Right = 627
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 13
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 2010
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrde' , @level0type=N'SCHEMA',@level0name=N'Detail', @level1type=N'VIEW',@level1name=N'ViewPlayerPortfolio'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'r = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'Detail', @level1type=N'VIEW',@level1name=N'ViewPlayerPortfolio'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'Detail', @level1type=N'VIEW',@level1name=N'ViewPlayerPortfolio'
GO
