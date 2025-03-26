CREATE PROCEDURE PayForOrder
    @OrderId INT,
    @AmountPaid DECIMAL(18, 2)
AS
BEGIN
    DECLARE @TotalAmount DECIMAL(18, 2);

    SELECT @TotalAmount = SUM(op.Price * op.Amount)
    FROM OrderPositions op
    WHERE op.OrderID = @OrderId;

    IF @TotalAmount <= @AmountPaid
    BEGIN
        UPDATE Orders
        SET IsPaid = 1
        WHERE ID = @OrderId;
    END
    ELSE
    BEGIN
        THROW 50000, 'Insufficient payment', 1;
    END
END