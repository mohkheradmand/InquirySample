--select * from ErrorLogs

SELECT 
		pa.Id, 
		InquiryId, 
		SharedInquiryId, 
		p.[Name],
		CASE pa.RequestStatus 
			WHEN -1 THEN 'Pending'
			WHEN 0 THEN 'Success'
			WHEN 1 THEN 'BussinessFailed'
			WHEN 2 THEN 'TechnicalError'
			WHEN 3 THEN 'Timeout'
		END AS RequestStatus,
		pa.CreatedAt,
		pa.CompletedAt
FROM ProviderAttempts as pa
JOIN Providers AS p ON pa.ProviderId = p.Id
ORDER BY 1 DESC

