/*

values(1, 'empresa', 23, 5455,'54545','lote1',getdate(), getdate(),23)


exec gsWMSProduccion_PendientesRecibir

sp_helptext gsWMSProduccion_PendientesRecibir

*/

alter procedure Cargar_Produccion_WMS  
as  
begin
declare @Tabla table (
Op	numeric	,
Kardex	varchar(20)	,
Codigo	varchar(100)	,
Descripcion	varchar(500)	,
Presentacion	varchar(200)	,
DescripcionDetallada	varchar(500)	,
NroParte	varchar(100)	,
Lote	varchar(100)	,
FechaFabricacion	datetime	,
FechaVencimiento	datetime	,
Estado	varchar(100)	,
Calidad	varchar(100)	,
Caracteristica	varchar(100)	,
Unidad	varchar(100)	,
Cantidad	int	,
Observaciones	varchar(500)	,
Nro_OrdenProduccion	varchar(100)	,
ClienteOC	varchar(500)	,
ClienteCodigo	varchar(500)	,
ClienteNombre	varchar(500)	,
ClienteSucursal	varchar(500)	,
OrigenTabla	varchar(200)	,
OrigenLinea	varchar(200)	,
OrigenOp	varchar(200)	,
OrigenFecha	datetime	,
OrigenTransaccion	varchar(200)	,
OrigenDespacho	varchar(200)	,
Clase1	varchar(100)	,
Clase2	varchar(100)	,
Clase3	varchar(100)	,
Clase4	varchar(100)	,
FKNiv01	int	,
FKNiv01_Nombre	varchar(500)	,
FKNiv02	int	,
FKNiv02_Nombre	varchar(500)	,
FKNiv03	int	,
FKNiv03_Nombre	varchar(500)	,
FKNiv04	int	,
FKNiv04_Nombre	varchar(500)	,
G1	varchar(500)	,
G2	varchar(500)	,
G3	varchar(500)	,
G4	varchar(500)	,
G5	varchar(500)	,
G6	varchar(500)	,
G7	varchar(500)	,
G8	varchar(500)	,
ModeloCodigo	varchar(500)	,
ModeloDescripcion	varchar(500)	
)

insert @Tabla
exec VBG01941 NULL,NULL,NULL,NULL,NULL,NULL,default,default,default,default,default,default,default,default,NULL,default,default,default,default,default,default,default,default,default


declare @RucEmpresa varchar(20)  
set @RucEmpresa = (select top 1 RUC from DatosEmpresa)  
  
declare @ID int  
  
set @ID =    
isnull((  
select T2.ID from (  
select isnull(T1.ID ,0) as ID  from (  
(select Top 1 CONVERT(int, isnull(id,0)) as ID  from APPIAGSILV..genesys_ingresos order by id desc  )  
) T1  ) T2  
),0)  
set @ID = (@ID + 1)  

insert APPIAGSILV..genesys_ingresos 
select  
@ID as ID,
@RucEmpresa as ID_Empresa,
T100.Op as NroPedido,
T100.Kardex as ID_Kardex,
T100.Codigo as ID_Item,
--T1.Descripcion,
T100.Lote as ID_Lote,
T100.FechaFabricacion,
T100.FechaVencimiento,
--T1.Unidad,
T100.Cantidad

from @Tabla T100

end
