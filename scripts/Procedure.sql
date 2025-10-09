create or replace PROCEDURE proc_AddProduct(
    p_productName     IN  NVARCHAR2,
    p_purchasePrice   IN  NUMBER,
    p_sellingPrice    IN  NUMBER,
    p_qualityStandard IN  NVARCHAR2,
    p_quantityInStock IN  NUMBER,
    p_photo           IN  BLOB,
    p_measurementUnit IN  NVARCHAR2,
    p_newProductId    OUT NUMBER
)
IS
    v_name    NVARCHAR2(100) := TRIM(p_productName);
    v_qs      NVARCHAR2(50)  := TRIM(p_qualityStandard);
    v_unit    NVARCHAR2(30)  := TRIM(p_measurementUnit);
BEGIN

    IF v_name IS NULL OR LENGTH(v_name) = 0 THEN
        RAISE_APPLICATION_ERROR(-20001, 'Vui lòng nhập tên sản phẩm.');
    END IF;

    IF v_qs IS NULL OR LENGTH(v_qs) = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Vui lòng nhập tiêu chuẩn chất lượng.');
    END IF;

    IF v_unit IS NULL OR LENGTH(v_unit) = 0 THEN
        RAISE_APPLICATION_ERROR(-20003, 'Vui lòng nhập đơn vị đo lường.');
    END IF;

    IF p_purchasePrice IS NULL OR p_sellingPrice IS NULL OR p_quantityInStock IS NULL THEN
        RAISE_APPLICATION_ERROR(-20004, 'Vui lòng nhập đầy đủ thông tin số liệu (giá, số lượng).');
    END IF;

    IF p_purchasePrice < 0 OR p_sellingPrice < 0 THEN
        RAISE_APPLICATION_ERROR(-20005, 'Giá không được âm.');
    END IF;

    IF p_quantityInStock < 0 THEN
        RAISE_APPLICATION_ERROR(-20006, 'Số lượng không hợp lệ (phải >= 0).');
    END IF;

--    INSERT INTO PRODUCT (PRODUCTNAME, PURCHASEPRICE, SELLINGPRICE, QUALITYSTANDARD, PHOTO, ISACTIVE)
--    VALUES (v_name, p_purchasePrice, p_sellingPrice, v_qs, p_photo, 1)
--    RETURNING PRODUCTID INTO p_newProductId;
    IF p_photo IS NULL THEN
        INSERT INTO PRODUCT (PRODUCTNAME, PURCHASEPRICE, SELLINGPRICE, QUALITYSTANDARD, ISACTIVE)
        VALUES (v_name, p_purchasePrice, p_sellingPrice, v_qs, 1)
        RETURNING PRODUCTID INTO p_newProductId;
    ELSE
        INSERT INTO PRODUCT (PRODUCTNAME, PURCHASEPRICE, SELLINGPRICE, QUALITYSTANDARD, PHOTO, ISACTIVE)
        VALUES (v_name, p_purchasePrice, p_sellingPrice, v_qs, p_photo, 1)
        RETURNING PRODUCTID INTO p_newProductId;
    END IF;


    INSERT INTO WAREHOUSEINFO(PRODUCTID, QUANTITY, MEASUREMENTUNIT)
    VALUES (p_newProductId, p_quantityInStock, v_unit);

    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20010, 'Lỗi khi thêm sản phẩm: ' || SUBSTR(SQLERRM, 1, 150));
END proc_AddProduct;
/

create or replace PROCEDURE proc_GetProductById(   
    in_productId IN NUMBER
)
IS
    v_ProductId NUMBER(10);
    v_productName NVARCHAR2(100);
    v_qualityStandard NVARCHAR2(100);
    v_purchasePrice FLOAT;
    v_sellingPrice FLOAT;
    v_photo BLOB;
    v_quantity NUMBER(10);
    v_measurementUnit NVARCHAR2(30);
BEGIN
    SELECT 
        p.ProductId,
        p.productName, 
        p.qualityStandard, 
        p.purchasePrice, 
        p.sellingPrice, 
        p.photo, 
        w.quantity, 
        w.measurementUnit
    INTO 
        v_ProductId,
        v_productName, 
        v_qualityStandard, 
        v_purchasePrice, 
        v_sellingPrice, 
        v_photo, 
        v_quantity, 
        v_measurementUnit
    FROM Product p
    JOIN WarehouseInfo w ON p.ProductId = w.productId
    WHERE p.ProductId = in_productId AND p.IsActive = 1;
END;
/
create or replace PROCEDURE proc_UpdateProduct(
    p_productId IN NUMBER,
    p_productName IN NVARCHAR2,
    p_purchasePrice IN NUMBER,
    p_sellingPrice IN NUMBER,
    p_qualityStandard IN NVARCHAR2,
    p_quantityInStock IN NUMBER,
    p_photo IN BLOB DEFAULT NULL,
    p_measurementUnit IN NVARCHAR2
)
IS
BEGIN
    -- Update thông tin sản phẩm (không cập nhật ảnh nếu null)
    UPDATE Product
    SET productName = p_productName,
        purchasePrice = p_purchasePrice,
        sellingPrice = p_sellingPrice,
        qualityStandard = p_qualityStandard,
        photo = NVL(p_photo, photo)
    WHERE ProductId = p_productId;


    -- Update kho
    UPDATE WarehouseInfo
    SET quantity = p_quantityInStock,
        measurementUnit = p_measurementUnit
    WHERE productId = p_productId;
    COMMIT;
END;
/

CREATE OR REPLACE PROCEDURE proc_UpdateQuanPurPriceProduct(
    p_ProductId IN NUMBER,
    p_purchasePrice IN NUMBER,
    p_quantityInStock IN NUMBER
)
IS
BEGIN
    UPDATE Product
    SET purchasePrice = p_purchasePrice   
    WHERE ProductId = p_ProductId;
    
    UPDATE WarehouseInfo
    SET quantity = p_quantityInStock
    WHERE productId = p_ProductId;
    
    COMMIT;
END;
/

CREATE OR REPLACE PROCEDURE proc_CreateOrder(
    p_priceTotal IN FLOAT,
    p_typeOfReceipt IN NVARCHAR2,
    p_discount IN FLOAT,
    p_note IN NVARCHAR2,
    p_receiptId OUT NUMBER
) IS
BEGIN
  
    INSERT INTO Receipt (typeOfReceipt, priceTotal, discount, note)
    VALUES (p_typeOfReceipt, p_priceTotal, p_discount, p_note)
    RETURNING receiptId INTO p_receiptId;

    COMMIT;  
END;
/
CREATE OR REPLACE PROCEDURE proc_DeleteProduct(p_productId IN NUMBER)
IS
    v_exists NUMBER;
BEGIN
    -- Kiểm tra xem sản phẩm đã từng xuất hiện trong hóa đơn chưa
    SELECT COUNT(*) INTO v_exists
    FROM ReceiptDetail
    WHERE productId = p_productId;

    IF v_exists > 0 THEN
        -- Nếu có: Không xoá, chỉ đánh dấu không hoạt động
        UPDATE Product
        SET IsActive = 0
        WHERE ProductId = p_productId;
    ELSE
        -- Nếu không có: Xoá sản phẩm
        DELETE FROM Product
        WHERE ProductId = p_productId;
    END IF;
END;
/

CREATE OR REPLACE PROCEDURE proc_DailyRevenueReport (
    p_ReportDate IN DATE,
    result_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN result_cursor FOR
    SELECT 
        R.receiptId,
        R.priceTotal,
        R.discount,
        (R.priceTotal - NVL(R.discount, 0)) AS FinalAmount
    FROM Receipt R
    JOIN Transactions T ON R.receiptId = T.receiptId
    WHERE 
        R.typeOfReceipt = N'Bán'
        AND TRUNC(T.DateOfImplementation) = TRUNC(p_ReportDate);
END;
/
CREATE OR REPLACE PROCEDURE proc_DailyDebtReport (
    p_ReportDate IN DATE,
    result_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN result_cursor FOR
    SELECT 
        R.receiptId,
        R.priceTotal,
        NVL(R.discount, 0) AS Discount,
        (R.priceTotal - NVL(R.discount, 0)) AS FinalAmount,
        T.repayment,
        ((R.priceTotal - NVL(R.discount, 0)) - T.repayment) AS RemainingDebt
    FROM Receipt R
    JOIN Transactions T ON R.receiptId = T.receiptId
    WHERE 
        TRUNC(T.DateOfImplementation) = TRUNC(p_ReportDate)
        AND T.repayment < (R.priceTotal - NVL(R.discount, 0));
END;
/
CREATE OR REPLACE PROCEDURE proc_DailyExpenseReport (
    p_ReportDate IN DATE,
    result_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN result_cursor FOR
    SELECT 
        R.receiptId,
        R.priceTotal,
        R.discount,
        (R.priceTotal - NVL(R.discount, 0)) AS FinalAmount
    FROM Receipt R
    JOIN Transactions T ON R.receiptId = T.receiptId
    WHERE 
        R.typeOfReceipt = N'Mua'
        AND TRUNC(T.DateOfImplementation) = TRUNC(p_ReportDate);
END;


