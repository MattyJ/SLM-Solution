-- =============================================
-- Author:		Matt Jordan
-- Create date: 25/10/2016
-- Description:	Service Domain Deletion
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteServiceDomain]
	-- Add the parameters for the stored procedure here
	@ServiceDomainId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		DECLARE @FunctionId int
		DECLARE Function_cursor CURSOR FOR SELECT Id FROM ServiceFunction
			WHERE ServiceDomainId = @ServiceDomainId

		OPEN Function_cursor
		FETCH NEXT FROM Function_cursor INTO @FunctionId

		WHILE @@FETCH_STATUS = 0
		BEGIN

			EXEC [dbo].[spDeleteServiceFunction] @FunctionId

			FETCH NEXT FROM Function_cursor INTO @FunctionId
		END

		CLOSE Function_cursor
		DEALLOCATE Function_cursor

		DELETE FROM [dbo].[ServiceDomain]
			WHERE Id = @ServiceDomainId
	END
END
GO