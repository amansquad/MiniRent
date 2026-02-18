-- Update PropertyImages.Url column to support Base64 images
ALTER TABLE PropertyImages ALTER COLUMN Url NVARCHAR(MAX) NOT NULL;
GO

-- Verify the change
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PropertyImages' AND COLUMN_NAME = 'Url';
GO
