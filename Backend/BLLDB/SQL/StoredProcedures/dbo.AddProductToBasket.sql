CREATE PROCEDURE AddProductToBasket
    @UserId INT,
    @ProductId INT,
    @Amount INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM BasketPositions WHERE UserID = @UserId AND ProductID = @ProductId)
    BEGIN
        UPDATE BasketPositions
        SET Amount = Amount + @Amount
        WHERE UserID = @UserId AND ProductID = @ProductId
    END
    ELSE
    BEGIN
        INSERT INTO BasketPositions (UserID, ProductID, Amount)
        VALUES (@UserId, @ProductId, @Amount)
    END
END