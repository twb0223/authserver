------DMS_TBM_Equipment----
alter table  dbo.DMS_TBM_Equipment  add EquipmentCPU  varchar(64) null
alter table  dbo.DMS_TBM_Equipment  add  EquipmentDisk varchar(64) null
alter table  dbo.DMS_TBM_Equipment  add  EquipmentWifiMac varchar(64) null
alter table  dbo.DMS_TBM_Equipment  add ClientChangeFlag  varchar(64) null
GO
 ----DMS_TBM_Station-----
alter table  dbo.DMS_TBM_Station  add  StationPorjectID  int null
alter table  dbo.DMS_TBM_Station  add  StationDepartmentID  int null
GO
 
-------------------新建表 DMS_TBM_StationDepartment---------------
/****** Object:  Table [dbo].[DMS_TBM_StationDepartment]    Script Date: 11/21/2014 14:42:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[DMS_TBM_StationDepartment](
	[StationDepartmentID] [int] IDENTITY(1,1) NOT NULL,
	[StationDepartmentName] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[StationDepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

---------------新建表[DMS_TBM_StationPorject]-----
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[DMS_TBM_StationPorject](
	[StationPorjectID] [int] IDENTITY(1,1) NOT NULL,
	[StationPorjectName] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[StationPorjectID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



 /****** Object:  View [dbo].[DMS_View_Equipment]    Script Date: 11/20/2014 10:51:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*修改设备视图*/
ALTER VIEW [dbo].[DMS_View_Equipment]
AS
SELECT     a.EquipmentID, a.EquipmentName, a.EquipmentDesc, a.InstitutionID, a.EquipmentTypeID, a.EquipmentCpuTypeID, a.ResolutionID, a.RegisterKey, a.EquipmentIP, 
                      a.AuthenticationState, a.EquipmentPort, a.EquipmentMac, a.EquipmentAdmin, a.EquipmentPassword, a.EquipmentSize, a.CreateAt, a.CreateBy, a.UpdateAt, 
                      a.UpdateBy, c.InstitutionName, c.InstitutionCode, ad.EquipmentCpuTypeContent, aw.ResolutionName, aw.ResolutionContent, i.StationName, 
                      (CASE WHEN i.StationName IS NULL THEN '无点位' ELSE i.StationName END) AS StationEquipmentName, c.ParentInstitutionID, a.OuterCode, 
                      a.OsID AS EquipmentOsTypeID, a.ProtocolID AS EquipmentProtocolID, a.StorageID AS EquipmentStorageID, a.CapacityID AS EquipmentCapacityID, a.CreateCode, 
                      ad.EquipmentCpuTypeName, at.EquipmentTypeName, ot.OsName, pt.ProtocolName, st.StorageName, ct.CapacitySize, a.EquipmentSecretKey, a.EquipmentCPU, 
                      a.EquipmentDisk, a.EquipmentWifiMac, a.ClientChangeFlag
FROM         dbo.DMS_TBM_Equipment AS a INNER JOIN
                      dbo.DMS_TBM_Institution AS c ON a.InstitutionID = c.InstitutionID LEFT OUTER JOIN
                      dbo.DMS_TSX_EquipmentCpuType AS ad ON a.EquipmentCpuTypeID = ad.EquipmentCpuTypeID LEFT OUTER JOIN
                      dbo.DMS_TSX_EquipmentType AS at ON a.EquipmentTypeID = at.EquipmentTypeID LEFT OUTER JOIN
                      dbo.DMS_TSX_Resolution AS aw ON a.ResolutionID = aw.ResolutionID LEFT OUTER JOIN
                      dbo.DMS_TGL_StationEquipment AS ai ON a.EquipmentID = ai.EquipmentID LEFT OUTER JOIN
                      dbo.DMS_TBM_Station AS i ON i.StationID = ai.StationID LEFT OUTER JOIN
                      dbo.DMS_TSX_OsType AS ot ON a.OsID = ot.OsID LEFT OUTER JOIN
                      dbo.DMS_TSX_ProtocolType AS pt ON a.ProtocolID = pt.ProtocolID LEFT OUTER JOIN
                      dbo.DMS_TSX_StorageType AS st ON a.StorageID = st.StorageID LEFT OUTER JOIN
                      dbo.DMS_TSX_CapacityType AS ct ON a.CapacityID = ct.CapacityID

GO
