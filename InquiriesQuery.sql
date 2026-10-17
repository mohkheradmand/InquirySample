SELECT
      Products.HsCode
      ,[CreatedAt]
      ,[CompletedAt]
      ,[Output]
      ,Providers.[Name]
      ,[InquiryStatus]
      ,CASE [InquiryStatus]
            WHEN -1 THEN 'Pending'
            WHEN 0 THEN 'Success'
            WHEN 1 THEN 'Failed'
       END AS [InquiryStatus]
  FROM Inquiries
  JOIN Products ON Products.Id = Inquiries.ProductId
  JOIN Providers ON Providers.Id = Inquiries.ProviderId
  ORDER BY Inquiries.Id DESC


--SELECT
--      Products.HsCode,
--      Count(*)
--  FROM Inquiries
--  JOIN Products ON Products.Id = Inquiries.ProductId
--  group by Products.HsCode