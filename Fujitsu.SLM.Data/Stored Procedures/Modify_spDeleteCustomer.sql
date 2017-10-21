-- =============================================
-- Author:		Matt Jordan
-- Create date: 26/10/2015
-- Description:	Customer deletion
-- =============================================
ALTER PROCEDURE [dbo].[spDeleteCustomer]
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

			EXEC [dbo].[spDeleteServiceDesk] @ServiceDeskId;

			FETCH NEXT FROM ServiceDesk_cursor INTO @ServiceDeskId
		END

	CLOSE ServiceDesk_cursor
	DEALLOCATE ServiceDesk_cursor

	DELETE FROM [dbo].[Contributor]
		WHERE CustomerId = @CustomerId

	DELETE FROM [dbo].[Diagram]
		WHERE CustomerId = @CustomerId

	DELETE FROM [dbo].[CustomerPack]
		WHERE CustomerId = @CustomerId

	DELETE FROM [dbo].[Audit]
		WHERE CustomerId = @CustomerId

	DELETE FROM [dbo].[Customer]
		WHERE Id = @CustomerId

	COMMIT TRAN
END

GO


