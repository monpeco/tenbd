

if exists (select * from dbo.sysobjects where [name] = 'test_Test')
BEGIN
PRINT 'Elimina procedure [test_Test]'
drop procedure [dbo].[test_Test]
END
GO

PRINT 'Creando procedure [test_Test]'
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
		print '1 - entrando a test_Test'
		  begin
			 set  @hay_err = '0'
			 set  @hay_era = '0'
			 set  @mensaje2 =' '
			 set @codi_empr = 1
			 set @tipo_docu = 39
			 set @foli_docu = 101
		  end

	print '@codi_empr: ' + @codi_empr + ', @tipo_docu: ' + @tipo_docu  + ', @foli_docu: ' + @foli_docu 

	print 'execute PARA_GET_VAL EGATE_TIPO_VALI, @p_salida output, @p_existe output, @p_mensaje output'
	execute PARA_GET_VAL 'EGATE_TIPO_VALI', @p_salida output, @p_existe output, @p_mensaje output
	
	print '@p_salida: ' + @p_salida + ', @p_existe: ' + @p_existe + ', @p_mensaje: ' + @p_mensaje


	execute @res = 	dte_chec_rang_foli @codi_empr,@tipo_docu,
					@corr_rafo output, @foli_docu, @ult_foli output, 
					@cod_error output, @mensaje output 
	--+  ', @ult_foli: ' + @ult_foli + ', @cod_error: ' + @cod_error
	print '@corr_rafo: [' + @corr_rafo + ']'
	print '@foli_docu: [' + @foli_docu + ']'
	print '@ult_foli: [' + @ult_foli + ']'
	print '@cod_error: [' + @cod_error + ']'
	print '@mensaje: [' + @mensaje + ']'
	end
 
 --- 2
 
	print '2 - validacion cod_error'
 	if @cod_error = 'S' 
		begin 
			set @mensaje2 =@mensaje2+ ' -20006 - FOLI_DOCU -'+ @mensaje
			print '---> @mensaje2: [' + @mensaje2 + ']' --TODO QUITAR
			
			if  @p_salida = 'TOTAL' 
				begin
					print '---> @p_salida if : [' + @p_salida + ']' --TODO QUITAR
					set  @hay_err = '1'
					raiserror (@mensaje2,16,1)
				end
			else 
				if  @p_salida  = 'PARCIAL'
					begin
						print '---> @p_salida else : [' + @p_salida + ']' --TODO QUITAR
						set @p_codi ='20001'
						execute @res = 	dte_chec_erro  @p_codi,
										@p_desc_erro output, @p_tipo_erro output,
										@p_indi_dnte output, @p_existe output, @p_mensaje output
						
						print '---> @res: [' + @res + ']' --TODO QUITAR
						print '---> @p_desc_erro: [' + @p_desc_erro + ']' --TODO QUITAR
						print '---> @p_tipo_erro: [' + @p_tipo_erro + ']' --TODO QUITAR
						print '---> @p_indi_dnte: [' + @p_indi_dnte + ']' --TODO QUITAR
						print '---> @p_existe: ['    + @p_existe + ']' --TODO QUITAR
						print '---> @p_mensaje: ['   + @p_mensaje + ']' --TODO QUITAR
						
						print '------------------------]' --TODO QUITAR
						print '---> @@p_tipo_dnte: ['   + @p_tipo_dnte + ']' --TODO QUITAR
						print '---> @@p_tipo_dnte: ['   + @p_tipo_dnte + ']' --TODO QUITAR
						print '---> @@p_indi_dnte: ['   + @p_indi_dnte + ']' --TODO QUITAR
						
						if (@p_tipo_dnte = 'N') OR (@p_tipo_dnte = 'S' AND @p_indi_dnte = 'S')
							begin
								print '---> @p_tipo_dnte = N: ' --TODO QUITAR
								if @p_tipo_erro ='ERR'   
									begin
										set  @hay_err = '1'
										print '---> @hay_err = 1' --TODO QUITAR
										raiserror (@mensaje2,16,1)
									end
								else
									begin
										set  @hay_era = '1'
										print '---> @hay_era = 1' --TODO QUITAR
									end
							end 
					end
		end
 
 
 ---
 
END

GO