USE [Nsna2016Directory]
GO

/****** Object:  Table [dbo].[KidsInfo]    Script Date: 12/5/2016 9:28:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[KidsInfo](
	[KidsInfoGuid] [nvarchar](128) NOT NULL,
	[FamilyContactGuid] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[Age] [smallint] NOT NULL,
	[Gender] [varchar](1) NOT NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedOn] [datetime] NULL,
	[RowVersion] [timestamp] NOT NULL,
	 CONSTRAINT [PK_KidsInfo] PRIMARY KEY CLUSTERED 
	(
		[KidsInfoGuid] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[KidsInfo]  WITH CHECK ADD  CONSTRAINT [FK_KidsInfo_FamilyContact] FOREIGN KEY([FamilyContactGuid])
REFERENCES [dbo].[FamilyContact] ([FamilyContactGuid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[KidsInfo] CHECK CONSTRAINT [FK_KidsInfo_FamilyContact]
GO


