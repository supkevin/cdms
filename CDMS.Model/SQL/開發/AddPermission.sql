INSERT INTO MenuPermission(PermissionID,MenuID)
SELECT 1 , MenuID
FROM Menu 
WHERE MenuPath IS NOT NULL
AND NOT EXISTS(
SELECT *
FROM MenuPermission 
WHERE PermissionID = 1 
AND MenuID = Menu.MenuID
)