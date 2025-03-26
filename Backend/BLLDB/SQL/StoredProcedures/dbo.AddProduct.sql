CREATE PROCEDURE AddProduct
    @Name NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @GroupId INT
AS
BEGIN
    INSERT INTO Products (Name, Price, GroupID, IsActive)
    VALUES (@Name, @Price, @GroupId, 1)

    SELECT 
        p.ID,
        p.Name,
        p.Price,
        pg.Name AS GroupName,
        p.IsActive
    FROM 
        Products p
    LEFT JOIN 
        ProductGroups pg ON p.GroupID = pg.ID
    WHERE 
        p.ID = SCOPE_IDENTITY()
END