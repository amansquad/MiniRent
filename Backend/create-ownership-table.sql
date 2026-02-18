-- Manual script to create UserPropertyOwnership table
-- Run this if the migration doesn't work

-- Drop table if it exists
IF OBJECT_ID('UserPropertyOwnership', 'U') IS NOT NULL
BEGIN
    DROP TABLE UserPropertyOwnership;
    PRINT 'Dropped existing UserPropertyOwnership table';
END

-- Create the table
CREATE TABLE UserPropertyOwnership (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    PropertyId UNIQUEIDENTIFIER NOT NULL,
    OwnershipStartDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    Notes NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    
    -- Foreign Keys
    CONSTRAINT FK_UserPropertyOwnership_Users 
        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    CONSTRAINT FK_UserPropertyOwnership_Properties 
        FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE NO ACTION,
    
    -- Unique constraint to prevent duplicate ownership records
    CONSTRAINT UQ_UserPropertyOwnership_UserProperty 
        UNIQUE (UserId, PropertyId)
);

-- Create indexes for better query performance
CREATE INDEX IX_UserPropertyOwnership_UserId 
    ON UserPropertyOwnership(UserId);

CREATE INDEX IX_UserPropertyOwnership_PropertyId 
    ON UserPropertyOwnership(PropertyId);

CREATE INDEX IX_UserPropertyOwnership_IsActive 
    ON UserPropertyOwnership(IsActive);

PRINT 'UserPropertyOwnership table created successfully';

-- Verify the table was created
SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'UserPropertyOwnership';

-- Show table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserPropertyOwnership'
ORDER BY ORDINAL_POSITION;

-- Optional: Populate table with existing ownership data
-- This creates records for all existing properties and their owners
INSERT INTO UserPropertyOwnership (UserId, PropertyId, OwnershipStartDate, IsActive, CreatedAt)
SELECT 
    p.CreatedById,
    p.Id,
    p.CreatedAt,
    1, -- IsActive
    GETUTCDATE()
FROM Properties p
WHERE p.CreatedById IS NOT NULL 
  AND p.IsDeleted = 0
  AND NOT EXISTS (
      SELECT 1 FROM UserPropertyOwnership upo 
      WHERE upo.UserId = p.CreatedById AND upo.PropertyId = p.Id
  );

PRINT 'Populated UserPropertyOwnership with existing data';

-- Show count of records
SELECT COUNT(*) AS TotalOwnershipRecords FROM UserPropertyOwnership;

-- Show sample data
SELECT TOP 5
    upo.Id,
    u.Username AS Owner,
    p.Address AS Property,
    upo.OwnershipStartDate,
    upo.IsActive
FROM UserPropertyOwnership upo
INNER JOIN Users u ON upo.UserId = u.Id
INNER JOIN Properties p ON upo.PropertyId = p.Id
ORDER BY upo.CreatedAt DESC;
