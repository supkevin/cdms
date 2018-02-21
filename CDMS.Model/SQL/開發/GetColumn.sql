use CDMS ;     -- 更換成你要查詢的資料庫
DECLARE @tablename NVARCHAR(50)
DECLARE @cloumnname NVARCHAR(50)
DECLARE @store TINYINT
SET @tablename=''   -- 輸入要查詢的資料表，留下空的表示查全部
SET @cloumnname=''  -- 輸入要查詢的欄位名稱，留下空的表示查全部
SET @store =2       -- 設定store != 1 會將結果暫存在 [__all_]  資料表，
                       -- 請注意是否跟既有資料表同名
IF @store =1        -- store=1 只會顯示、傳回結果～不會儲存
BEGIN
SELECT so.name 'DataTable',sc.name 'DataColumn',st.name 'DataType', sc.length 'DataLength', st.allownulls 'AllowNulls'
FROM sysobjects so
INNER JOIN syscolumns sc ON so.id =sc.id 
INNER JOIN systypes st ON st.xtype=sc.xtype
WHERE (so.type='U'AND st.name <> 'sysname') AND so.name LIKE '%'+@tablename+'%' AND  sc.name LIKE '%'+@cloumnname+'%'
ORDER BY 1
END
ELSE   -- 不成立則存在[__all_]  資料表
BEGIN
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[__all_]') AND TYPE in (N'U'))
DROP TABLE [__all_]
SELECT so.name 'DataTable',sc.name 'DataColumn',st.name 'DataType', sc.length 'DataLength', st.allownulls 'AllowNulls'
INTO __all_ 
FROM sysobjects so
INNER JOIN syscolumns sc ON so.id =sc.id 
INNER JOIN systypes st ON st.xtype=sc.xtype
WHERE (so.type='U'AND st.name <> 'sysname') AND so.name LIKE '%'+@tablename+'%' AND  sc.name LIKE '%'+@cloumnname+'%'
ORDER BY 1
END