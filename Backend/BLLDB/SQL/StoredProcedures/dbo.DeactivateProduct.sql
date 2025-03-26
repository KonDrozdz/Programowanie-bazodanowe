CREATE PROCEDURE DeactivateProduct
    @ProductId INT
AS
BEGIN
    UPDATE Products
    SET IsActive = 0
    WHERE ID = @ProductId
END