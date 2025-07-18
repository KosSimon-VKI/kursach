USE [БД_Агеенков]
GO
/****** Object:  Table [dbo].[Договоры]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Договоры](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[заказчик_id] [int] NULL,
	[номер_договора] [varchar](255) NOT NULL,
	[дата_заключения] [date] NULL,
	[описание] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Заказчики]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Заказчики](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[название] [varchar](255) NOT NULL,
	[адрес] [text] NULL,
	[контактное_лицо] [varchar](255) NULL,
	[телефон] [varchar](20) NULL,
	[email] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Измерения]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Измерения](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[пикет_id] [int] NULL,
	[оператор_id] [int] NULL,
	[дата_время] [datetime] NULL,
	[тип_измерения] [varchar](50) NULL,
	[результат] [float] NULL,
	[примечания] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Координаты]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Координаты](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[широта] [decimal](9, 6) NOT NULL,
	[долгота] [decimal](9, 6) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Оборудование]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Оборудование](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[проект_id] [int] NOT NULL,
	[название] [varchar](255) NOT NULL,
	[тип] [varchar](100) NULL,
	[серийный_номер] [varchar](100) NULL,
	[характеристики] [text] NULL,
	[дата_добавления] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Операторы]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Операторы](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[фио] [varchar](255) NOT NULL,
	[должность] [varchar](255) NULL,
	[контактный_телефон] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Отчёт_об_измерениях]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Отчёт_об_измерениях](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[измерения_id] [int] NOT NULL,
	[проект_id] [int] NOT NULL,
	[дата_создания] [datetime] NULL,
	[описание] [text] NULL,
	[графики] [varchar](255) NULL,
	[файл_отчета] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Пикеты]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Пикеты](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[профиль_id] [int] NULL,
	[номер] [int] NOT NULL,
	[координата_id] [int] NOT NULL,
	[высота] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Площади]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Площади](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[проект_id] [int] NULL,
	[название] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Площади_Координаты]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Площади_Координаты](
	[площадь_id] [int] NOT NULL,
	[координата_id] [int] NOT NULL,
	[порядковый_номер] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[площадь_id] ASC,
	[координата_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Пользователи]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Пользователи](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[логин] [varchar](50) NOT NULL,
	[пароль] [varchar](255) NOT NULL,
	[тип_id] [int] NOT NULL,
	[проект_id] [int] NULL,
	[фио] [varchar](255) NULL,
	[email] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Пользователи_Логин] UNIQUE NONCLUSTERED 
(
	[логин] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Проекты]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Проекты](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[договор_id] [int] NULL,
	[название] [varchar](255) NOT NULL,
	[описание] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Проекты_Договор] UNIQUE NONCLUSTERED 
(
	[договор_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Профили]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Профили](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[площадь_id] [int] NULL,
	[название] [varchar](255) NOT NULL,
	[тип] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Профили_Координаты]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Профили_Координаты](
	[профиль_id] [int] NOT NULL,
	[координата_id] [int] NOT NULL,
	[порядковый_номер] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[профиль_id] ASC,
	[координата_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ТипыПользователей]    Script Date: 21.04.2025 19:15:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ТипыПользователей](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[название] [varchar](50) NOT NULL,
	[описание] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Оборудование] ADD  DEFAULT (getdate()) FOR [дата_добавления]
GO
ALTER TABLE [dbo].[Отчёт_об_измерениях] ADD  DEFAULT (getdate()) FOR [дата_создания]
GO
ALTER TABLE [dbo].[Договоры]  WITH NOCHECK ADD FOREIGN KEY([заказчик_id])
REFERENCES [dbo].[Заказчики] ([id])
GO
ALTER TABLE [dbo].[Измерения]  WITH NOCHECK ADD FOREIGN KEY([оператор_id])
REFERENCES [dbo].[Операторы] ([id])
GO
ALTER TABLE [dbo].[Измерения]  WITH NOCHECK ADD FOREIGN KEY([пикет_id])
REFERENCES [dbo].[Пикеты] ([id])
GO
ALTER TABLE [dbo].[Оборудование]  WITH NOCHECK ADD FOREIGN KEY([проект_id])
REFERENCES [dbo].[Проекты] ([id])
GO
ALTER TABLE [dbo].[Отчёт_об_измерениях]  WITH NOCHECK ADD FOREIGN KEY([измерения_id])
REFERENCES [dbo].[Измерения] ([id])
GO
ALTER TABLE [dbo].[Отчёт_об_измерениях]  WITH NOCHECK ADD FOREIGN KEY([проект_id])
REFERENCES [dbo].[Проекты] ([id])
GO
ALTER TABLE [dbo].[Пикеты]  WITH NOCHECK ADD FOREIGN KEY([координата_id])
REFERENCES [dbo].[Координаты] ([id])
GO
ALTER TABLE [dbo].[Пикеты]  WITH NOCHECK ADD FOREIGN KEY([профиль_id])
REFERENCES [dbo].[Профили] ([id])
GO
ALTER TABLE [dbo].[Площади]  WITH NOCHECK ADD FOREIGN KEY([проект_id])
REFERENCES [dbo].[Проекты] ([id])
GO
ALTER TABLE [dbo].[Площади_Координаты]  WITH NOCHECK ADD FOREIGN KEY([координата_id])
REFERENCES [dbo].[Координаты] ([id])
GO
ALTER TABLE [dbo].[Площади_Координаты]  WITH NOCHECK ADD FOREIGN KEY([площадь_id])
REFERENCES [dbo].[Площади] ([id])
GO
ALTER TABLE [dbo].[Пользователи]  WITH NOCHECK ADD FOREIGN KEY([проект_id])
REFERENCES [dbo].[Проекты] ([id])
GO
ALTER TABLE [dbo].[Пользователи]  WITH NOCHECK ADD FOREIGN KEY([тип_id])
REFERENCES [dbo].[ТипыПользователей] ([id])
GO
ALTER TABLE [dbo].[Проекты]  WITH NOCHECK ADD FOREIGN KEY([договор_id])
REFERENCES [dbo].[Договоры] ([id])
GO
ALTER TABLE [dbo].[Профили]  WITH NOCHECK ADD FOREIGN KEY([площадь_id])
REFERENCES [dbo].[Площади] ([id])
GO
ALTER TABLE [dbo].[Профили_Координаты]  WITH NOCHECK ADD FOREIGN KEY([координата_id])
REFERENCES [dbo].[Координаты] ([id])
GO
ALTER TABLE [dbo].[Профили_Координаты]  WITH NOCHECK ADD FOREIGN KEY([профиль_id])
REFERENCES [dbo].[Профили] ([id])
GO
