-- Script to populate UserPropertyOwnership table with existing data
-- Run this after the table has been created

-- First, check if table exists
IF OBJECT_ID('UserPropertyOwnership', 'U') IS NULL
BEGIN
    PRINT 'ERROR: UserPropertyOwnership table does not exist!';
    PRINT 'Please run create-ownership-table.sql first';
    RETURN;
END

PRINT 'UserPropertyOwnership table exists. Proceeding with data population...';
PRINT '';

-- Show current record count
DECLARE @CurrentCount INT;
SELECT @CurrentCount = COUNT(*) FROM UserPropertyOwnership;
PRINT 'Current records in UserPropertyOwnership: ' + CAST(@CurrentCount AS VARCHAR(10));
PRINT '';

-- Show available properties to populate
PRINT 'Properties available for population:';
SELECT 
    COUNT(*) AS TotalProperties,
    COUNT(DISTINCT CreatedById) AS UniqueOwners
FROM Properties 
WHERE CreatedById IS NOT NULL AND IsDeleted = 0;
PRINT '';

-- Insert ownership records for all existing properties
PRINT 'Inserting ownership records...';

INSERT INTO UserPropertyOwnership (
    Id,
    UserId, 
    PropertyId, 
    OwnershipStartDate, 
    IsActive, 
    Notes,
    CreatedAt
)
SELECT 
    NEWID() AS Id,
    p.CreatedById AS UserId,
    p.Id AS PropertyId,
    p.CreatedAt AS OwnershipStartDate,
    1 AS IsActive,
    'Auto-populated from existing property data' AS Notes,
    GETUTCDATE() AS CreatedAt
FROM Properties p
WHERE p.CreatedById IS NOT NULL 
  AND p.IsDeleted = 0
  AND NOT EXISTS (
      SELECT 1 
      FROM UserPropertyOwnership upo 
      WHERE upo.UserId = p.CreatedById 
        AND upo.PropertyId = p.Id
  );

DECLARE @InsertedCount INT = @@ROWCOUNT;
PRINT 'Inserted ' + CAST(@InsertedCount AS VARCHAR(10)) + ' new ownership records';
PRINT '';

-- Show new record count
SELECT @CurrentCount = COUNT(*) FROM UserPropertyOwnership;
PRINT 'Total records in UserPropertyOwnership: ' + CAST(@CurrentCount AS VARCHAR(10));
PRINT '';

-- Show sample of inserted data
PRINT 'Sample of ownership records:';
SELECT TOP 10
    upo.Id,
    u.Username AS OwnerUsername,
    u.FullName AS OwnerName,
    p.Title AS PropertyTitle,
    p.Address AS PropertyAddress,
    p.Status AS PropertyStatus,
    upo.OwnershipStartDate,
    upo.IsActive,
    upo.CreatedAt
FROM UserPropertyOwnership upo
INNER JOIN Users u ON upo.UserId = u.Id
INNER JOIN Properties p ON upo.PropertyId = p.Id
ORDER BY upo.CreatedAt DESC;

PRINT '';
PRINT 'Summary by owner:';
SELECT 
    u.Username,
    u.FullName,
    COUNT(upo.PropertyId) AS TotalProperties,
    COUNT(CASE WHEN upo.IsActive = 1 THEN 1 END) AS ActiveOwnerships
FROM Users u
LEFT JOIN UserPropertyOwnership upo ON u.Id = upo.UserId
GROUP BY u.Id, u.Username, u.FullName
HAVING COUNT(upo.PropertyId) > 0
ORDER BY TotalProperties DESC;

PRINT '';
PRINT 'Data population complete!';
