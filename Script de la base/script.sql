USE [GED]
GO
/****** Object:  Table [dbo].[TB_Status]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TB_Status](
	[id_status] [int] NOT NULL,
	[libelle] [varchar](50) NULL,
 CONSTRAINT [PK_status] PRIMARY KEY CLUSTERED 
(
	[id_status] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [GED].[TB_SD_Manquent]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [GED].[TB_SD_Manquent](
	[NSDM] [int] NOT NULL,
	[id_DF] [int] NOT NULL,
	[libelle_SD] [varchar](50) NULL,
	[etat] [int] NULL,
 CONSTRAINT [PK_TB_SD_Manquent] PRIMARY KEY CLUSTERED 
(
	[NSDM] ASC,
	[id_DF] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TB_Phase]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TB_Phase](
	[id_phase] [int] NOT NULL,
	[libelle] [varchar](50) NULL,
 CONSTRAINT [PK_TB_Phase] PRIMARY KEY CLUSTERED 
(
	[id_phase] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TB_historique_Vues]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_historique_Vues](
	[id_vues] [int] NOT NULL,
	[id_DF] [int] NULL,
	[id_SDF] [int] NULL,
	[status] [int] NULL,
	[nature_orgine] [int] NULL,
	[numero_orgine] [int] NULL,
	[indice_orgine] [int] NULL,
	[indice_special_orgine] [int] NULL,
	[numero_titre] [int] NULL,
	[indice_titre] [int] NULL,
	[indice_speciale_titre] [int] NULL,
	[numero_sd] [int] NULL,
	[formalite] [int] NULL,
	[volume_depot] [int] NULL,
	[numero_depot] [int] NULL,
	[date_depot] [int] NULL,
	[nom_piece] [int] NULL,
	[nombre_page] [int] NULL,
	[num_page] [int] NULL,
	[qualite_image] [int] NULL,
	[taille_image] [int] NULL,
	[A_SUPPRIMER] [int] NULL,
	[nature_orgine_cr] [int] NULL,
	[numero_orgine_cr] [int] NULL,
	[indice_orgine_cr] [int] NULL,
	[indice_special_orgine_cr] [int] NULL,
	[numero_titre_cr] [int] NULL,
	[indice_titre_cr] [int] NULL,
	[indice_speciale_titre_cr] [int] NULL,
	[numero_sd_cr] [int] NULL,
	[formalite_cr] [int] NULL,
	[volume_depot_cr] [int] NULL,
	[numero_depot_cr] [int] NULL,
	[date_depot_cr] [int] NULL,
	[nom_piece_cr] [int] NULL,
	[nombre_page_cr] [int] NULL,
	[num_page_cr] [int] NULL,
	[qualite_image_cr] [int] NULL,
	[taille_image_cr] [int] NULL,
	[img_orgine_illisible_cr] [int] NULL,
	[sd_manquent_cr] [int] NULL,
	[piece_manquent_cr] [int] NULL,
	[user_controle] [int] NULL,
	[user_corriger] [int] NULL,
	[user_controle_corriger] [int] NULL,
	[etat_corriger] [int] NULL,
	[dateaction] [date] NOT NULL,
	[phase] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_historique_Dos]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_historique_Dos](
	[id_DF] [int] NOT NULL,
	[id_unite] [int] NULL,
	[status] [int] NULL,
	[phase] [int] NULL,
	[date_action] [date] NULL,
	[user_action] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [GED].[TB_View_Manquent]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [GED].[TB_View_Manquent](
	[N_view_m] [int] NOT NULL,
	[id_DF] [int] NOT NULL,
	[id_SDF] [int] NOT NULL,
	[libelle] [varchar](50) NULL,
	[etat] [int] NULL,
 CONSTRAINT [PK_TB_View_Manquent] PRIMARY KEY CLUSTERED 
(
	[N_view_m] ASC,
	[id_DF] ASC,
	[id_SDF] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TB_Tranche]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_Tranche](
	[id_Tranche] [int] NOT NULL,
	[status] [int] NULL,
	[date_reception_controle] [datetime] NULL,
	[date_reception_corection] [datetime] NULL,
 CONSTRAINT [PK_TB_Tranche] PRIMARY KEY CLUSTERED 
(
	[id_Tranche] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_Unite]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TB_Unite](
	[id_unite] [int] NOT NULL,
	[id_tranche] [int] NULL,
	[status] [int] NULL,
	[nom_unite] [varchar](50) NULL,
	[date_reception_controle] [datetime] NULL,
	[date_reception_corection] [datetime] NULL,
 CONSTRAINT [PK_TB_Unite] PRIMARY KEY CLUSTERED 
(
	[id_unite] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TB_DossierF]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TB_DossierF](
	[id_DF] [int] NOT NULL,
	[id_unite] [int] NULL,
	[status] [int] NULL,
	[date_reception_controle] [datetime] NULL,
	[date_correction] [datetime] NULL,
	[date_controle_correction] [datetime] NULL,
	[phase] [int] NULL,
	[idUser] [int] NULL,
	[username] [varchar](50) NULL,
 CONSTRAINT [PK_TB_DossierF] PRIMARY KEY CLUSTERED 
(
	[id_DF] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TB_Vues]    Script Date: 02/10/2016 22:39:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_Vues](
	[id_vues] [int] NOT NULL,
	[id_DF] [int] NULL,
	[id_SDF] [int] NULL,
	[status] [int] NULL,
	[nature_orgine] [int] NULL,
	[numero_orgine] [int] NULL,
	[indice_orgine] [int] NULL,
	[indice_special_orgine] [int] NULL,
	[numero_titre] [int] NULL,
	[indice_titre] [int] NULL,
	[indice_speciale_titre] [int] NULL,
	[numero_sd] [int] NULL,
	[formalite] [int] NULL,
	[volume_depot] [int] NULL,
	[numero_depot] [int] NULL,
	[date_depot] [int] NULL,
	[nom_piece] [int] NULL,
	[nombre_page] [int] NULL,
	[num_page] [int] NULL,
	[qualite_image] [int] NULL,
	[taille_image] [int] NULL,
	[A_SUPPRIMER] [int] NULL,
	[nature_orgine_cr] [int] NULL,
	[numero_orgine_cr] [int] NULL,
	[indice_orgine_cr] [int] NULL,
	[indice_special_orgine_cr] [int] NULL,
	[numero_titre_cr] [int] NULL,
	[indice_titre_cr] [int] NULL,
	[indice_speciale_titre_cr] [int] NULL,
	[numero_sd_cr] [int] NULL,
	[formalite_cr] [int] NULL,
	[volume_depot_cr] [int] NULL,
	[numero_depot_cr] [int] NULL,
	[date_depot_cr] [int] NULL,
	[nom_piece_cr] [int] NULL,
	[nombre_page_cr] [int] NULL,
	[num_page_cr] [int] NULL,
	[qualite_image_cr] [int] NULL,
	[taille_image_cr] [int] NULL,
	[img_orgine_illisible_cr] [int] NULL,
	[sd_manquent_cr] [int] NULL,
	[piece_manquent_cr] [int] NULL,
	[user_controle] [int] NULL,
	[user_corriger] [int] NULL,
	[user_controle_corriger] [int] NULL,
	[etat_corriger] [int] NULL,
 CONSTRAINT [PK_TB_Vues] PRIMARY KEY CLUSTERED 
(
	[id_vues] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_TB_DossierF_TB_Phase]    Script Date: 02/10/2016 22:39:07 ******/
ALTER TABLE [dbo].[TB_DossierF]  WITH CHECK ADD  CONSTRAINT [FK_TB_DossierF_TB_Phase] FOREIGN KEY([phase])
REFERENCES [dbo].[TB_Phase] ([id_phase])
GO
ALTER TABLE [dbo].[TB_DossierF] CHECK CONSTRAINT [FK_TB_DossierF_TB_Phase]
GO
/****** Object:  ForeignKey [FK_TB_DossierF_TB_Status]    Script Date: 02/10/2016 22:39:07 ******/
ALTER TABLE [dbo].[TB_DossierF]  WITH CHECK ADD  CONSTRAINT [FK_TB_DossierF_TB_Status] FOREIGN KEY([status])
REFERENCES [dbo].[TB_Status] ([id_status])
GO
ALTER TABLE [dbo].[TB_DossierF] CHECK CONSTRAINT [FK_TB_DossierF_TB_Status]
GO
/****** Object:  ForeignKey [FK_TB_DossierF_TB_Unite]    Script Date: 02/10/2016 22:39:07 ******/
ALTER TABLE [dbo].[TB_DossierF]  WITH CHECK ADD  CONSTRAINT [FK_TB_DossierF_TB_Unite] FOREIGN KEY([id_unite])
REFERENCES [dbo].[TB_Unite] ([id_unite])
GO
ALTER TABLE [dbo].[TB_DossierF] CHECK CONSTRAINT [FK_TB_DossierF_TB_Unite]
GO
/****** Object:  ForeignKey [FK_TB_Tranche_TB_Status]    Script Date: 02/10/2016 22:39:07 ******/
ALTER TABLE [dbo].[TB_Tranche]  WITH CHECK ADD  CONSTRAINT [FK_TB_Tranche_TB_Status] FOREIGN KEY([status])
REFERENCES [dbo].[TB_Status] ([id_status])
GO
ALTER TABLE [dbo].[TB_Tranche] CHECK CONSTRAINT [FK_TB_Tranche_TB_Status]
GO
/****** Object:  ForeignKey [FK_TB_Unite_TB_Status]    Script Date: 02/10/2016 22:39:07 ******/
ALTER TABLE [dbo].[TB_Unite]  WITH CHECK ADD  CONSTRAINT [FK_TB_Unite_TB_Status] FOREIGN KEY([status])
REFERENCES [dbo].[TB_Status] ([id_status])
GO
ALTER TABLE [dbo].[TB_Unite] CHECK CONSTRAINT [FK_TB_Unite_TB_Status]
GO
/****** Object:  ForeignKey [FK_TB_Unite_TB_Tranche]    Script Date: 02/10/2016 22:39:07 ******/
ALTER TABLE [dbo].[TB_Unite]  WITH CHECK ADD  CONSTRAINT [FK_TB_Unite_TB_Tranche] FOREIGN KEY([id_tranche])
REFERENCES [dbo].[TB_Tranche] ([id_Tranche])
GO
ALTER TABLE [dbo].[TB_Unite] CHECK CONSTRAINT [FK_TB_Unite_TB_Tranche]
GO
/****** Object:  ForeignKey [FK_TB_Vues_TB_DossierF]    Script Date: 02/10/2016 22:39:07 ******/
ALTER TABLE [dbo].[TB_Vues]  WITH CHECK ADD  CONSTRAINT [FK_TB_Vues_TB_DossierF] FOREIGN KEY([id_DF])
REFERENCES [dbo].[TB_DossierF] ([id_DF])
GO
ALTER TABLE [dbo].[TB_Vues] CHECK CONSTRAINT [FK_TB_Vues_TB_DossierF]
GO
/****** Object:  ForeignKey [FK_TB_Vues_TB_Status]    Script Date: 02/10/2016 22:39:07 ******/
ALTER TABLE [dbo].[TB_Vues]  WITH CHECK ADD  CONSTRAINT [FK_TB_Vues_TB_Status] FOREIGN KEY([status])
REFERENCES [dbo].[TB_Status] ([id_status])
GO
ALTER TABLE [dbo].[TB_Vues] CHECK CONSTRAINT [FK_TB_Vues_TB_Status]
GO
