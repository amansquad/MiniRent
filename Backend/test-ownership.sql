-- Test script to verify ownership views and tables

-- Check if UserPropertyOwnership table exists
SELECT 
    CASE WHEN EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.TABLES 
        WHERE TABLE_NAME = 'UserPropertyOwnership'
    ) THEN 'UserPropertyOwnership table EXISTS' 
    ELSE 'UserPropertyOwnership table DOES NOT EXIST' 
    END AS TableStatus;

-- Check if views exist
SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN (
    'vw_UserPropertyOwnership',
    'vw_UserPropertiesSummary', 
    'vw_PropertyOwnershipHistory'
)
ORDER BY TABLE_NAME;

-- If table exists, check its structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserPropertyOwnership')
BEGIN
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'UserPropertyOwnership'
    ORDER BY ORDINAL_POSITION;
END

-- Test basic ownership query (using existing relationship)
SELECT TOP 5
    u.Id AS UserId,
    u.Username,
    u.FullName AS OwnerName,
    p.Id AS PropertyId,
    p.Address AS PropertyAddress,
    p.Status AS PropertyStatus
FROM Users u
INNER JOIN Properties p ON u.Id = p.CreatedById
WHERE p.IsDeleted = 0
ORDER BY u.Username, p.CreatedAt DESC;

-- Count properties per user
SELECT 
    u.Username,
    u.FullName,
    COUNT(p.Id) AS PropertyCount
FROM Users u
LEFT JOIN Properties p ON u.Id = p.CreatedById AND p.IsDeleted = 0
GROUP BY u.Id, u.Username, u.FullName
ORDER BY PropertyCount DESC;
