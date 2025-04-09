CREATE TABLE Users (
    Username NVARCHAR(450) NOT NULL PRIMARY KEY,
    EncryptedApiKey NVARCHAR(MAX) NULL,
    ApiKeyGeneratedDate DATETIME NULL,
    ExampleDataField NVARCHAR(4000) NULL,
    IsPayingCustomer BIT NOT NULL DEFAULT 0,
    RequestCount INT NOT NULL DEFAULT 0,
    LastRequestDate DATETIME NULL,
    StripeCustomerId NVARCHAR(450) NULL,
    RowVersion ROWVERSION NOT NULL
);
