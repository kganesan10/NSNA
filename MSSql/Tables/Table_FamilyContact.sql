USE [Nsna2016Directory]
GO

/****** Object:  Table [dbo].[FamilyContact]    Script Date: 12/5/2016 9:27:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[FamilyContact](
	[FamilyContactGuid] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Gender] [varchar](1) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[HomePhone] [nvarchar](25) NOT NULL,
	[MobilePhone] [nvarchar](25) NULL,
	[Address] [nvarchar](128) NULL,
	[City] [nvarchar](128) NOT NULL,
	[State] [nvarchar](128) NOT NULL,
	[ZipCode] [nvarchar](25) NULL,
	[Kovil] [nvarchar](50) NOT NULL,
	[KovilPirivu] [nvarchar](50) NOT NULL,
	[NativePlace] [nvarchar](128) NOT NULL,
	[MaritalStatus] [varchar](1) NOT NULL,
	[SpouseFirstName] [nvarchar](50) NULL,
	[SpouseLastName] [nvarchar](50) NULL,
	[SpouseEmail] [nvarchar](128) NULL,
	[SpouseMobilePhone] [nvarchar](25) NULL,
	[SpouseKovil] [nvarchar](50) NULL,
	[SpouseKovilPirivu] [nvarchar](50) NULL,
	[SpouseNativePlace] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[LastModifiedOn] [datetime] NULL,
	[RowVersion] [timestamp] NOT NULL,
	 CONSTRAINT [PK_FamilyContact] PRIMARY KEY CLUSTERED 
	(
		[FamilyContactGuid] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

SET ANSI_PADDING OFF
GO


