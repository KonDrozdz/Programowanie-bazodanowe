CREATE PROCEDURE GenerateOrder
    @UserId INT
AS
BEGIN
    DECLARE @OrderId INT;

    INSERT INTO Orders (UserID, Date, IsPaid)
    VALUES (@UserId, GETUTCDATE(), 0);

    SET @OrderId = SCOPE_IDENTITY();

     INSERT INTO OrderPositions (OrderID, ProductId, Amount, Price)
     SELECT @OrderID, bp.ProductID, bp.Amount, p.Price
     FROM BasketPositions bp
     JOIN Products p ON p.ID = bp.ProductID
     WHERE bp.UserID = @UserId;

    DELETE FROM BasketPositions
    WHERE UserID = @UserId;

    SELECT 
        o.ID,
        o.Date,
        SUM(op.Price * op.Amount) AS TotalAmount,
        o.IsPaid
    FROM 
        Orders o
    INNER JOIN 
        OrderPositions op ON o.ID = op.OrderID
    WHERE 
        o.ID = @OrderId
    GROUP BY 
        o.ID, o.Date, o.IsPaid;
END