-- =============================================
-- Author:		Matt Jordan
-- Create date: 26/02/2015
-- Description:	Customer Deletion
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteCustomer]
	-- Add the parameters for the stored procedure here
	@CustomerId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRAN

		DECLARE @ServiceDeskId int
		DECLARE ServiceDesk_cursor CURSOR FOR SELECT Id FROM ServiceDesk
			WHERE CustomerId = @CustomerId

		OPEN ServiceDesk_cursor
		FETCH NEXT FROM ServiceDesk_cursor INTO @ServiceDeskId

		WHILE @@FETCH_STATUS = 0
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

				DECLARE @FunctionId int
				DECLARE Function_cursor CURSOR FOR SELECT Id FROM ServiceFunction
					WHERE ServiceDomainId = @DomainId

				OPEN Function_cursor
				FETCH NEXT FROM Function_cursor INTO @FunctionId

					WHILE @@FETCH_STATUS = 0
					BEGIN

						DELETE FROM [dbo].[ServiceComponent]
							WHERE ServiceFunctionId = @FunctionId

						FETCH NEXT FROM Function_cursor INTO @FunctionId
					END

				CLOSE Function_cursor
				DEALLOCATE Function_cursor

				DELETE FROM [dbo].[ServiceFunction]
					WHERE ServiceDomainId = @DomainId

				FETCH NEXT FROM Domain_cursor INTO @DomainId
			END

			CLOSE Domain_cursor
			DEALLOCATE Domain_cursor

			DELETE FROM [dbo].[ServiceDomain]
				WHERE ServiceDeskId = @ServiceDeskId

			DELETE FROM [dbo].[DeskInputType]
				WHERE ServiceDeskId = @ServiceDeskId

			FETCH NEXT FROM ServiceDesk_cursor INTO @ServiceDeskId
		END

	CLOSE ServiceDesk_cursor
	DEALLOCATE ServiceDesk_cursor

	DELETE FROM [dbo].[Resolver]
		WHERE ServiceDeskId = @ServiceDeskId

	DELETE FROM [dbo].[Contributor]
		WHERE CustomerId = @CustomerId

	DELETE FROM [dbo].[ServiceDesk]
		WHERE CustomerId = @CustomerId

	DELETE FROM [dbo].[Diagram]
		WHERE CustomerId = @CustomerId

	DELETE FROM [dbo].[CustomerPack]
		WHERE CustomerId = @CustomerId

	DELETE FROM [dbo].[Customer]
		WHERE Id = @CustomerId

	COMMIT TRAN
END

GO


