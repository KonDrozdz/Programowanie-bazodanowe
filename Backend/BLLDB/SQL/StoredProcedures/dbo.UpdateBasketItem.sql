CREATE PROCEDURE UpdateBasketItem
    @UserId INT,
    @ProductId INT,
    @Amount INT
AS
BEGIN
    UPDATE BasketPositions
    SET Amount = @Amount
    WHERE UserID = @UserId AND ProductID = @ProductId
END