-- =============================================
-- Author:		Matt Jordan
-- Create date: 25/10/2016
-- Description:	Service Component Deletion
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteServiceComponent]
	-- Add the parameters for the stored procedure here
	@ServiceComponentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		DECLARE @ComponentId int
		DECLARE Service_component_cursor CURSOR FOR SELECT Id FROM ServiceComponent
			WHERE Id = @ServiceComponentId OR ParentServiceComponentId = @ServiceComponentId
			ORDER BY ComponentLevel DESC

		OPEN Service_component_cursor
		FETCH NEXT FROM Service_component_cursor INTO @ComponentId

		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @ResolverId int
			DECLARE Component_resolver_cursor CURSOR FOR SELECT Id FROM Resolver
				WHERE ServiceComponent_Id = @ComponentId

			OPEN Component_resolver_cursor
			FETCH NEXT FROM Component_resolver_cursor INTO @ResolverId
			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC [dbo].[spDeleteResolver] @ResolverId

				FETCH NEXT FROM Component_resolver_cursor INTO @ResolverId
			END

			CLOSE Component_resolver_cursor
			DEALLOCATE Component_resolver_cursor

			DELETE FROM [dbo].[ServiceComponent]
			WHERE Id = @ComponentId

			FETCH NEXT FROM Service_component_cursor INTO @ComponentId
		END

		CLOSE Service_component_cursor
		DEALLOCATE Service_component_cursor
	END
END
GO
