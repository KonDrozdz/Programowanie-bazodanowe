CREATE PROCEDURE AddProductGroup
    @Name NVARCHAR(100),
    @ParentId INT = NULL
AS
BEGIN
    INSERT INTO ProductGroups (Name, ParentID)
    VALUES (@Name, @ParentId)

    SELECT 
        ID,
        Name,
        ParentID
    FROM 
        ProductGroups
    WHERE 
        ID = SCOPE_IDENTITY()
END