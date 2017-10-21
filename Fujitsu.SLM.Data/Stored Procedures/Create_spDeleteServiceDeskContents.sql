-- =============================================
-- Author:		Matt Jordan
-- Create date: 25/10/2016
-- Description:	Service Desk Content Deletion
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteServiceDeskContents]
	-- Add the parameters for the stored procedure here
	@ServiceDeskId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN

		DECLARE @ResolverId int
		DECLARE Resolver_cursor CURSOR FOR SELECT Id FROM Resolver
			WHERE ServiceDeskId = @ServiceDeskId

		UPDATE [dbo].[Resolver]
		SET [ServiceComponent_Id] = NULL
 		WHERE ServiceDeskId = @ServiceDeskId

		OPEN Resolver_cursor
		FETCH NEXT FROM Resolver_cursor INTO @ResolverId

		WHILE @@FETCH_STATUS = 0
		BEGIN
				DELETE FROM [dbo].[OperationalProcessType]
				FROM [dbo].[OperationalProcessType] opt
				WHERE opt.Resolver_Id = @ResolverId

				FETCH NEXT FROM Resolver_cursor INTO @ResolverId
		END

		CLOSE Resolver_cursor
		DEALLOCATE Resolver_cursor

		DECLARE @DomainId int
		DECLARE Domain_cursor CURSOR FOR SELECT Id FROM ServiceDomain
			WHERE ServiceDeskId = @ServiceDeskId

		OPEN Domain_cursor
		FETCH NEXT FROM Domain_cursor INTO @DomainId

		WHILE @@FETCH_STATUS = 0
		BEGIN

			EXEC [dbo].[spDeleteServiceDomain] @DomainId

			FETCH NEXT FROM Domain_cursor INTO @DomainId
		END

		CLOSE Domain_cursor
		DEALLOCATE Domain_cursor

		DELETE FROM [dbo].[ServiceDomain]
			WHERE ServiceDeskId = @ServiceDeskId
	END

	DELETE FROM [dbo].[Resolver]
		WHERE ServiceDeskId = @ServiceDeskId
END
GO


