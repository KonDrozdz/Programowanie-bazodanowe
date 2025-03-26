CREATE PROCEDURE DeleteProduct
    @ProductId INT
AS
BEGIN
    DELETE FROM Products
    WHERE ID = @ProductId
END