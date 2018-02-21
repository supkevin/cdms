/*
-- ��Ҧ��i�઺�������
INSERT INTO _require(DataTable,DataColumn)
SELECT DataTable,DataColumn
FROM [dbo].[__all_]
WHERE 1 = 1
AND DataColumn NOT IN ('SeqNo','LastPerson','LastUpdate')
--AND [DataType] IN ('nchar','nvarchar')
AND DataTable NOT IN('__all_','_require','sysdiagrams','Menu','MenuPermission')
AND DataTable NOT IN('Alternative','BankAccount')
AND DataTable NOT IN('Brand','Code','Company','News','Product','ProductImage','User')
AND DataTable NOT IN('Country','Feedback','Inspection','Observation','OverSea','OverType','Store','Track', 'Workplace')
ORDER BY [DataTable],[DataColumn]
*/
/*�u��ӧO��ƪ�g�J*/
INSERT INTO _require(DataTable,DataColumn)
SELECT DataTable,DataColumn
FROM [dbo].[__all_]
WHERE 1 = 1
AND DataColumn NOT IN ('SeqNo','LastPerson','LastUpdate')
AND DataTable IN('SalesInvoice','SalesInvoiceDetail')
ORDER BY [DataTable],[DataColumn]

-- ��ƫ��O���� 
/*
INSERT INTO _mapping(DataType,AllowNulls)
SELECT Distinct DataType, AllowNulls
FROM __all_

SELECT *
FROM __all_
WHERE DataType  = 'varchar'
*/