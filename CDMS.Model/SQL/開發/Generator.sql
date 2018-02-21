--SELECT COLUMN_Name ,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH ,IS_NULLABLE , COLUMN_DEFAULT
--FROM INFORMATION_SCHEMA.Columns 
--WHERE Table_Name = 'Company'

DECLARE @TableName NVARCHAR(30), @ColumnName NVARCHAR(30), @DataType NVARCHAR(30), @DataLength INT
DECLARE @IsRequire INT, @IsPhone INT, @IsMail INT
DECLARE @RangeText NVARCHAR(50)
DECLARE @PropertyType NVARCHAR(30)
DECLARE @OriginalTable NVARCHAR(30)

SET NOCOUNT ON 
SET @OriginalTable = ''

DECLARE MyCursor CURSOR FAST_FORWARD FOR
SELECT a.DataTable, a.DataColumn, a.DataType, 
	   CAST( a.[DataLength] AS INT) AS [DataLength], 
	   ISNULL(c.ProprtyType,'string') AS ProprtyType,
	   CASE 
		WHEN b.IsRequired IS NULL THEN 0
	   ELSE b.IsRequired 
	   END AS IsRequire,
	   CASE 
		WHEN b.IsPhone IS NULL THEN 0
	   ELSE b.IsPhone 
	   END AS IsPhone,
	   CASE 
		WHEN b.IsMail IS NULL THEN 0
	   ELSE b.IsMail 
	   END AS IsMail,
	   b.RangeText
FROM [dbo].[__all_] a LEFT JOIN _require b
ON (a.DataTable = b.DataTable AND a.DataColumn = b.DataColumn)
LEFT JOIN _mapping c
ON (a.DataType = c.DataType AND a.AllowNulls = c.AllowNulls)
WHERE a.DataTable NOT IN('__all_','_require', 'InvoiceStock', '_mapping', 'Menu','MenuPermission','Log','sysdiagrams','_GZ$')
AND a.DataTable NOT IN('Alternative','BankAccount')
AND a.DataTable NOT IN('Brand','Code','News','ProductImage')
AND a.DataTable NOT IN('Country','Feedback','Inspection','Inspection_Image', 'Observation','OverSea','OverType','Store','Track', 'Workplace')
AND a.DataColumn NOT IN('LastPerson', 'LastUpdate')
ORDER BY a.DataTable, a.DataColumn
 
OPEN MyCursor;
FETCH NEXT FROM MyCursor INTO @TableName, @ColumnName, @DataType, @DataLength, @PropertyType, 
							  @IsRequire, @IsPhone, @IsMail, @RangeText;
 
WHILE @@FETCH_STATUS = 0
BEGIN   
  IF (@TableName <> @OriginalTable) 
  BEGIN
	IF (@OriginalTable <> '')
	BEGIN
		PRINT '}';
		PRINT '#endregion';
		PRINT '';
	END

	PRINT '#region Base' + @TableName ;
	PRINT 'public class Base' + @TableName + ': ' + @TableName ;
	PRINT '{' ;
  END 
 
  -- 文字類型或必填欄位才需處理
  IF ( @DataType = 'nvarchar' OR @DataType = 'nchar' OR @IsRequire = 1 )
  BEGIN

	PRINT ''
	-- Required
	IF (@IsRequire = 1) PRINT '[Required]';

	-- IsPhone
	IF (@IsPhone = 1) PRINT '[Phone]';
		
	-- IsMail
	IF (@IsMail = 1) PRINT '[EmailAddress]'
	
	--Range
	IF (@RangeText IS NOT NULL) PRINT '[' + @RangeText +']'

	-- 最小長度
	IF (@DataType = 'nchar') PRINT '[MinLength('+ CAST(@DataLength/2 AS NVARCHAR(30))  +')]';
	
	-- 最大長度
	IF (@DataType = 'nchar' OR @DataType = 'nvarchar')
	PRINT '[MaxLength('+ CAST(@DataLength/2 AS NVARCHAR(30)) +')]';
	PRINT 'new public '+ @PropertyType +' '+ @ColumnName + '{ get; set; }';	
  END
	--[Required]
	--[MaxLength(4)]
	--[MinLength(4)]
	--new public string BrandID { get; set; }
  SET @OriginalTable = @TableName;
  FETCH NEXT FROM MyCursor INTO @TableName, @ColumnName, @DataType, @DataLength, @PropertyType, 
							  @IsRequire, @IsPhone, @IsMail, @RangeText;
END

PRINT '}' ;
PRINT '#endregion';

CLOSE MyCursor;
DEALLOCATE MyCursor;