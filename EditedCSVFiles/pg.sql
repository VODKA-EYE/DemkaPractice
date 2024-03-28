SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------------------------------------------------------
-- Table Manufacturer
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Manufacturer (
  ID SERIAL,
  Name VARCHAR(100)  NOT NULL,
  StartDate DATE NULL,
  PRIMARY KEY (ID));

-- ----------------------------------------------------------------------------
-- Table Product
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Product (
  ID SERIAL,
  Title VARCHAR(100)  NOT NULL,
  Cost DECIMAL(19,4) NOT NULL,
  Description TEXT  NULL,
  MainImagePath VARCHAR(1000)  NULL,
  IsActive BOOL NOT NULL,
  ManufacturerID INT NULL,
  PRIMARY KEY (ID),
  CONSTRAINT FK_Product_Manufacturer
    FOREIGN KEY (ManufacturerID)
    REFERENCES Manufacturer (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table Gender
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Gender (
  Code SERIAL  NOT NULL,
  Name VARCHAR(16)  NULL,
  PRIMARY KEY (Code));

-- ----------------------------------------------------------------------------
-- Table Client
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Client (
  ID SERIAL,
  FirstName VARCHAR(50)  NOT NULL,
  LastName VARCHAR(50)  NOT NULL,
  Patronymic VARCHAR(50)  NULL,
  Birthday DATE NULL,
  RegistrationDate DATE NOT NULL,
  Email VARCHAR(255)  NULL,
  Phone VARCHAR(20)  NOT NULL,
  GenderCode SERIAL  NOT NULL,
  PhotoPath VARCHAR(1000)  NULL,
  PRIMARY KEY (ID),
  CONSTRAINT FK_Client_Gender
    FOREIGN KEY (GenderCode)
    REFERENCES Gender (Code)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table TagOfClient
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS TagOfClient (
  ClientID INT NOT NULL,
  TagID INT NOT NULL,
  PRIMARY KEY (ClientID, TagID),
  CONSTRAINT FK_TagOfClient_Client
    FOREIGN KEY (ClientID)
    REFERENCES Client (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT FK_TagOfClient_Tag
    FOREIGN KEY (TagID)
    REFERENCES Tag (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table Tag
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Tag (
  ID SERIAL,
  Title VARCHAR(30)  NOT NULL,
  Color CHAR(6)  NOT NULL,
  PRIMARY KEY (ID));

-- ----------------------------------------------------------------------------
-- Table Service
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Service (
  ID SERIAL,
  Title VARCHAR(100)  NOT NULL,
  Cost DECIMAL(19,4) NOT NULL,
  DurationInSeconds INT NOT NULL,
  Description TEXT  NULL,
  Discount FLOAT NULL,
  MainImagePath VARCHAR(1000)  NULL,
  PRIMARY KEY (ID));

-- ----------------------------------------------------------------------------
-- Table ServicePhoto
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ServicePhoto (
  ID SERIAL,
  ServiceID INT NOT NULL,
  PhotoPath VARCHAR(1000)  NOT NULL,
  PRIMARY KEY (ID),
  CONSTRAINT FK_ServicePhoto_Service
    FOREIGN KEY (ServiceID)
    REFERENCES Service (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table ProductSale
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ProductSale (
  ID SERIAL,
  SaleDate DATE NOT NULL,
  ProductID INT NOT NULL,
  Quantity INT NOT NULL,
  ClientServiceID INT NULL,
  PRIMARY KEY (ID),
  CONSTRAINT FK_ProductSale_Product
    FOREIGN KEY (ProductID)
    REFERENCES Product (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT FK_ProductSale_ClientService
    FOREIGN KEY (ClientServiceID)
    REFERENCES ClientService (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table ClientService
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ClientService (
  ID SERIAL,
  ClientID INT NOT NULL,
  ServiceID INT NOT NULL,
  StartTime DATE NOT NULL,
  Comment TEXT  NULL,
  PRIMARY KEY (ID),
  CONSTRAINT FK_ClientService_Client
    FOREIGN KEY (ClientID)
    REFERENCES Client (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT FK_ClientService_Service
    FOREIGN KEY (ServiceID)
    REFERENCES Service (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table ProductPhoto
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ProductPhoto (
  ID SERIAL,
  ProductID INT NOT NULL,
  PhotoPath VARCHAR(1000)  NOT NULL,
  PRIMARY KEY (ID),
  CONSTRAINT FK_ProductPhoto_Product
    FOREIGN KEY (ProductID)
    REFERENCES Product (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table DocumentByService
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS DocumentByService (
  ID SERIAL,
  ClientServiceID INT NOT NULL,
  DocumentPath VARCHAR(1000)  NOT NULL,
  PRIMARY KEY (ID),
  CONSTRAINT FK_DocumentByService_ClientService
    FOREIGN KEY (ClientServiceID)
    REFERENCES ClientService (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

-- ----------------------------------------------------------------------------
-- Table AttachedProduct
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS AttachedProduct (
  MainProductID INT NOT NULL,
  AttachedProductID INT NOT NULL,
  PRIMARY KEY (MainProductID, AttachedProductID),
  CONSTRAINT FK_AttachedProduct_Product1
    FOREIGN KEY (AttachedProductID)
    REFERENCES Product (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT FK_AttachedProduct_Product
    FOREIGN KEY (MainProductID)
    REFERENCES Product (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
    
SET FOREIGN_KEY_CHECKS = 1;