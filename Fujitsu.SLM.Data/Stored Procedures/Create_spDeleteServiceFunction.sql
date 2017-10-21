-- =============================================
-- Author:		Matt Jordan
-- Create date: 25/10/2016
-- Description:	Service Function Deletion
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteServiceFunction]
	-- Add the parameters for the stored procedure here
	@ServiceFunctionId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		DECLARE @ComponentId int
		DECLARE Component_cursor CURSOR FOR SELECT Id FROM ServiceComponent
			WHERE ServiceFunctionId = @ServiceFunctionId
			ORDER BY ComponentLevel DESC

		OPEN Component_cursor
		FETCH NEXT FROM Component_cursor INTO @ComponentId

		WHILE @@FETCH_STATUS = 0
		BEGIN

			EXEC [dbo].[spDeleteServiceComponent] @ComponentId

			FETCH NEXT FROM Component_cursor INTO @ComponentId
		END

		CLOSE Component_cursor
		DEALLOCATE Component_cursor

		DELETE FROM [dbo].[ServiceFunction]
			WHERE Id = @ServiceFunctionId
	END
END
GO