-- Diagnostic script to check UserPropertyOwnership table status
-- Run this to see what's happening

PRINT '========================================';
PRINT 'UserPropertyOwnership Table Diagnostics';
PRINT '========================================';
PRINT '';

-- 1. Check if table exists
PRINT '1. Checking if table exists...';
IF OBJECT_ID('UserPropertyOwnership', 'U') IS NOT NULL
BEGIN
    PRINT '   ✓ UserPropertyOwnership table EXISTS';
    PRINT '';
    
    -- 2. Show table structure
    PRINT '2. Table Structure:';
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        CHARACTER_MAXIMUM_LENGTH,
        IS_NULLABLE,
        COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'UserPropertyOwnership'
    ORDER BY ORDINAL_POSITION;
    PRINT '';
    
    -- 3. Show constraints
    PRINT '3. Table Constraints:';
    SELECT 
        CONSTRAINT_NAME,
        CONSTRAINT_TYPE
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'UserPropertyOwnership';
    PRINT '';
    
    -- 4. Show indexes
    PRINT '4. Table Indexes:';
    SELECT 
        i.name AS IndexName,
        i.type_desc AS IndexType,
        COL_NAME(ic.object_id, ic.column_id) AS ColumnName
    FROM sys.indexes i
    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    WHERE i.object_id = OBJECT_ID('UserPropertyOwnership')
    ORDER BY i.name, ic.key_ordinal;
    PRINT '';
    
    -- 5. Check record count
    PRINT '5. Record Count:';
    DECLARE @RecordCount INT;
    SELECT @RecordCount = COUNT(*) FROM UserPropertyOwnership;
    PRINT '   Total records: ' + CAST(@RecordCount AS VARCHAR(10));
    
    IF @RecordCount = 0
    BEGIN
        PRINT '   ⚠ WARNING: Table is EMPTY!';
        PRINT '';
        PRINT '   To populate the table, run: populate-ownership-data.sql';
    END
    ELSE
    BEGIN
        PRINT '   ✓ Table has data';
        PRINT '';
        
        -- 6. Show sample data
        PRINT '6. Sample Data (First 5 records):';
        SELECT TOP 5
            upo.Id,
            upo.UserId,
            u.Username AS OwnerUsername,
            upo.PropertyId,
            p.Address AS PropertyAddress,
            upo.OwnershipStartDate,
            upo.IsActive,
            upo.CreatedAt
        FROM UserPropertyOwnership upo
        LEFT JOIN Users u ON upo.UserId = u.Id
        LEFT JOIN Properties p ON upo.PropertyId = p.Id
        ORDER BY upo.CreatedAt DESC;
        PRINT '';
        
        -- 7. Show statistics
        PRINT '7. Ownership Statistics:';
        SELECT 
            COUNT(*) AS TotalRecords,
            COUNT(DISTINCT UserId) AS UniqueOwners,
            COUNT(DISTINCT PropertyId) AS UniqueProperties,
            COUNT(CASE WHEN IsActive = 1 THEN 1 END) AS ActiveOwnerships,
            COUNT(CASE WHEN IsActive = 0 THEN 1 END) AS InactiveOwnerships
        FROM UserPropertyOwnership;
    END
END
ELSE
BEGIN
    PRINT '   ✗ UserPropertyOwnership table DOES NOT EXIST!';
    PRINT '';
    PRINT '   To create the table, run: create-ownership-table.sql';
    PRINT '   Or apply migration: dotnet ef database update --project Backend';
END

PRINT '';
PRINT '========================================';
PRINT '8. Available Properties (not in ownership table):';
PRINT '========================================';

IF OBJECT_ID('UserPropertyOwnership', 'U') IS NOT NULL
BEGIN
    SELECT 
        p.Id AS PropertyId,
        p.Title,
        p.Address,
        p.Status,
        p.CreatedById AS OwnerId,
        u.Username AS OwnerUsername,
        u.FullName AS OwnerName,
        p.CreatedAt AS PropertyCreatedAt,
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM UserPropertyOwnership upo 
                WHERE upo.PropertyId = p.Id AND upo.UserId = p.CreatedById
            ) THEN 'Yes'
            ELSE 'No'
        END AS InOwnershipTable
    FROM Properties p
    LEFT JOIN Users u ON p.CreatedById = u.Id
    WHERE p.IsDeleted = 0 AND p.CreatedById IS NOT NULL
    ORDER BY p.CreatedAt DESC;
END
ELSE
BEGIN
    -- Just show properties if table doesn't exist
    SELECT 
        p.Id AS PropertyId,
        p.Title,
        p.Address,
        p.Status,
        p.CreatedById AS OwnerId,
        u.Username AS OwnerUsername,
        u.FullName AS OwnerName,
        p.CreatedAt AS PropertyCreatedAt
    FROM Properties p
    LEFT JOIN Users u ON p.CreatedById = u.Id
    WHERE p.IsDeleted = 0 AND p.CreatedById IS NOT NULL
    ORDER BY p.CreatedAt DESC;
END

PRINT '';
PRINT '========================================';
PRINT 'Diagnostics Complete';
PRINT '========================================';
