use CDMS ;     -- �󴫦��A�n�d�ߪ���Ʈw
DECLARE @tablename NVARCHAR(50)
DECLARE @cloumnname NVARCHAR(50)
DECLARE @store TINYINT
SET @tablename=''   -- ��J�n�d�ߪ���ƪ�A�d�U�Ū���ܬd����
SET @cloumnname=''  -- ��J�n�d�ߪ����W�١A�d�U�Ū���ܬd����
SET @store =2       -- �]�wstore != 1 �|�N���G�Ȧs�b [__all_]  ��ƪ�A
                       -- �Ъ`�N�O�_��J����ƪ�P�W
IF @store =1        -- store=1 �u�|��ܡB�Ǧ^���G�㤣�|�x�s
BEGIN
SELECT so.name 'DataTable',sc.name 'DataColumn',st.name 'DataType', sc.length 'DataLength', st.allownulls 'AllowNulls'
FROM sysobjects so
INNER JOIN syscolumns sc ON so.id =sc.id 
INNER JOIN systypes st ON st.xtype=sc.xtype
WHERE (so.type='U'AND st.name <> 'sysname') AND so.name LIKE '%'+@tablename+'%' AND  sc.name LIKE '%'+@cloumnname+'%'
ORDER BY 1
END
ELSE   -- �����߫h�s�b[__all_]  ��ƪ�
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