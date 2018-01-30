

if exists (select * from dbo.sysobjects where [name] = 'test_Test')
BEGIN
PRINT 'Elimina trigger [test_Test]'
drop procedure [dbo].[test_Test]
END
GO

PRINT 'Creando trigger [test_Test]'
GO

create  procedure test_Test 
           @p_tipo   varchar(3), 
           @p_erro varchar(1) output, 
           @p_mens  varchar(80) output
AS
	declare @hay   numeric(1),
	
	@hay_err	varchar(3),    	
	@hay_era	varchar(3),
	@res		varchar(10),
	@codi_empr 	varchar(20),
	@tipo_docu	varchar(20),
	@corr_rafo 	varchar(20), 
	@foli_docu 	varchar(20),
	@ult_foli 	varchar(10),
	@cod_error 	varchar(10), 
	@mensaje 	varchar(80),
	@mensaje2 	varchar(2000),
	@p_salida 	VARCHAR(80),
	@p_existe 	varchar(1),
	@p_mensaje 	varchar(80),
	@p_codi_erro 	varchar(12),
	@p_codi 	 	varchar(5),
	@p_desc_erro 	varchar(50),
	@p_indi_dnte 	varchar(1),
	@p_tipo_erro 	varchar(12),
	@p_tipo_dnte 	varchar(1),
	@esta_docu 	 	varchar(3),
	@rutt_rece 	 	varchar(20),
	@p_aux_esta_defi varchar(3)
BEGIN
	begin

		print 'entrando a test_Test'

	end
 
END

GO