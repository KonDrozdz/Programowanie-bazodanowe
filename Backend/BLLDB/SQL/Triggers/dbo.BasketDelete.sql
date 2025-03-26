CREATE TRIGGER BasketDelete
ON BasketPositions
AFTER DELETE
AS
BEGIN
    INSERT INTO BasketLog (UserID, ProductID, ActionTime, Action)
    SELECT d.UserID, d.ProductID, GETDATE(), 'Removed from basket'
    FROM DELETED d;
END