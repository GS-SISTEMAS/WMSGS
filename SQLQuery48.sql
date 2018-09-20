

alter procedure Cargar_Produccion_WMS
as
begin
 declare @table table
 (
 	[id] [varchar](10) NOT NULL,
	[ID_Empresa] [varchar](20) NULL,
	[NroPedido] [numeric](18, 0) NULL,
	[ID_Kardex] [varchar](20) NULL,
	[ID_Item] [varchar](20) NULL,
	[ID_Lote] [varchar](500) NULL,
	[FecManufactura] [datetime] NULL,
	[FecVencimiento] [datetime] NULL,
	[Cantidad] [money] NULL

 )

--select * from APPIAGSILV..genesys_ingresos 

insert @table 
exec gsWMSProduccion_PendientesRecibir


--select * from @table

end


exec Cargar_Produccion_WMS

