if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[test_Test]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
PRINT 'Elimina trigger [test_Test]'
drop procedure [dbo].[test_Test]
GO

PRINT 'Creando trigger [test_Test]'
GO

create  procedure test_Test 
           @p_tipo   varchar(3), 
           @p_erro varchar(1) output, 
           @p_mens  varchar(80) output
AS
	declare @hay   numeric(1)
BEGIN
	begin

		print 'entrando a test_Test'

	end
 
END

GO