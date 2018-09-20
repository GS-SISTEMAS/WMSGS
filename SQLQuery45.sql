

alter procedure Cargar_Pedidos_WMS
as
begin
declare @fecha1 datetime
declare @fecha2 datetime 
set @fecha1 = getdate()  
set @fecha2 = getdate()  

insert APPIAGSILV..genesys_pedidosventa 
exec VBG00518_WMS null,@fecha1,@fecha2,null,null,null,null,null,null,null,0,'' 
end




