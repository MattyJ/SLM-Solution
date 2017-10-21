-- =============================================
-- Author:		Matt Jordan
-- Create date: 25/10/2016
-- Description:	Resolver Deletion
-- =============================================
CREATE PROCEDURE [dbo].[spDeleteResolver]
	-- Add the parameters for the stored procedure here
	@ResolverId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN

	DELETE FROM [dbo].[OperationalProcessType]
		WHERE Resolver_Id = @ResolverId

	UPDATE [dbo].[ServiceComponent]
		SET [ResolverId] = NULL
		WHERE ResolverId = @ResolverId

	DELETE FROM [dbo].[Resolver]
		WHERE Id = @ResolverId

	END
END
