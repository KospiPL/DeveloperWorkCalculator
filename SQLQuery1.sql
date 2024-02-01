CREATE TABLE ITEM_HIS (
    Id INT PRIMARY KEY,
    Rev INT,
    System_ChangedDate_OldValue DATETIME,
    System_ChangedDate_NewValue DATETIME,
    System_BoardColumn_OldValue VARCHAR(255),
    System_BoardColumn_NewValue VARCHAR(255)
);
CREATE TABLE ITEM_DET (
    Id INT,
    Rev INT,
    AreaPath VARCHAR(255),
    TeamProject VARCHAR(255),
    IterationPath VARCHAR(255),
    WorkItemType VARCHAR(255),
    State VARCHAR(255),
    AssignedTo_DisplayName VARCHAR(255),
    CreatedDate DATETIME,
    Title VARCHAR(255),
    BoardColumn VARCHAR(255),
    ActivatedDate DATETIME,
    ResolvedDate DATETIME,
    PRIMARY KEY (Id)
);