set ansi_nulls on
go
set ansi_padding on
go
set quoted_identifier on 
go

CREATE DATABASE Stock_TEST
GO

USE Stock_TEST
GO

CREATE TABLE [dbo].[Product]
(
	[ID_Product] INT NOT NULL IDENTITY(1,1),
	[Product_Name] VARCHAR(255) UNIQUE NOT NULL,
	CONSTRAINT PK_Product PRIMARY KEY CLUSTERED ([ID_Product] ASC) ON [PRIMARY]
)

CREATE TABLE [dbo].[Consumption]
(
	[ID_Consumption] INT NOT NULL IDENTITY(1,1),
	[Date_Consumption] DATE NOT NULL,
	[Quantity_Consumption] INT NOT NULL,
	[Product_ID] INT NOT NULL,
	CONSTRAINT PK_Consumption PRIMARY KEY CLUSTERED ([ID_Consumption] ASC) on [PRIMARY],
	CONSTRAINT FK_Consumption_Product FOREIGN KEY ([Product_ID]) REFERENCES Product([ID_Product])
)

CREATE TABLE [dbo].[Incoming]
(
	[ID_Incoming] INT NOT NULL IDENTITY(1,1),
	[Date_Incoming] DATE NOT NULL,
	[Quantity_Incoming] INT NOT NULL,
	[Product_ID] INT NOT NULL,
	CONSTRAINT PK_Incoming PRIMARY KEY CLUSTERED ([ID_Incoming] ASC) on [PRIMARY],
	CONSTRAINT FK_Incoming_Product FOREIGN KEY ([Product_ID]) REFERENCES Product([ID_Product])
)

CREATE OR ALTER VIEW ProductView
AS
	SELECT Product_Name as 'Название товара'
	FROM Product

CREATE OR ALTER VIEW ConsumptionView
AS
	SELECT Date_Consumption as 'Дата расхода',
			Quantity_Consumption as 'Кол-во товара',
			Product.Product_Name as 'Название товара'
	FROM Consumption INNER JOIN Product ON Product.ID_Product = Consumption.Product_ID


CREATE OR ALTER VIEW IncomingView
AS
	SELECT Date_Incoming as 'Дата прихода',
			Quantity_Incoming as 'Кол-во товара',
			Product.Product_Name as 'Название товара'
	FROM Incoming INNER JOIN Product ON Product.ID_Product = Incoming.Product_ID