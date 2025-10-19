create or replace PROCEDURE AGRICULTURAL_AGENT.proc_AddProduct(
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
        INSERT INTO AGRICULTURAL_AGENT.PRODUCT (PRODUCTNAME, PURCHASEPRICE, SELLINGPRICE, QUALITYSTANDARD, ISACTIVE)
        VALUES (v_name, p_purchasePrice, p_sellingPrice, v_qs, 1)
        RETURNING PRODUCTID INTO p_newProductId;
    ELSE
        INSERT INTO AGRICULTURAL_AGENT.PRODUCT (PRODUCTNAME, PURCHASEPRICE, SELLINGPRICE, QUALITYSTANDARD, PHOTO, ISACTIVE)
        VALUES (v_name, p_purchasePrice, p_sellingPrice, v_qs, p_photo, 1)
        RETURNING PRODUCTID INTO p_newProductId;
    END IF;


    INSERT INTO AGRICULTURAL_AGENT.WAREHOUSEINFO(PRODUCTID, QUANTITY, MEASUREMENTUNIT)
    VALUES (p_newProductId, p_quantityInStock, v_unit);

    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20010, 'Lỗi khi thêm sản phẩm: ' || SUBSTR(SQLERRM, 1, 150));
END proc_AddProduct;
/

create or replace PROCEDURE AGRICULTURAL_AGENT.proc_GetProductById(   
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
    FROM AGRICULTURAL_AGENT.Product p
    JOIN AGRICULTURAL_AGENT.WarehouseInfo w ON p.ProductId = w.productId
    WHERE p.ProductId = in_productId AND p.IsActive = 1;
END;
/
create or replace PROCEDURE AGRICULTURAL_AGENT.proc_UpdateProduct(
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
    UPDATE AGRICULTURAL_AGENT.Product
    SET productName = p_productName,
        purchasePrice = p_purchasePrice,
        sellingPrice = p_sellingPrice,
        qualityStandard = p_qualityStandard,
        photo = NVL(p_photo, photo)
    WHERE ProductId = p_productId;


    -- Update kho
    UPDATE AGRICULTURAL_AGENT.WarehouseInfo
    SET quantity = p_quantityInStock,
        measurementUnit = p_measurementUnit
    WHERE productId = p_productId;
    COMMIT;
END;
/

CREATE OR REPLACE PROCEDURE AGRICULTURAL_AGENT.proc_UpdateQuanPurPriceProduct(
    p_ProductId IN NUMBER,
    p_purchasePrice IN NUMBER,
    p_quantityInStock IN NUMBER
)
IS
BEGIN
    UPDATE AGRICULTURAL_AGENT.Product
    SET purchasePrice = p_purchasePrice   
    WHERE ProductId = p_ProductId;
    
    UPDATE AGRICULTURAL_AGENT.WarehouseInfo
    SET quantity = p_quantityInStock
    WHERE productId = p_ProductId;
    
    COMMIT;
END;
/

CREATE OR REPLACE PROCEDURE AGRICULTURAL_AGENT.proc_CreateOrder(
    p_priceTotal IN FLOAT,
    p_typeOfReceipt IN NVARCHAR2,
    p_discount IN FLOAT,
    p_note IN NVARCHAR2,
    p_receiptId OUT NUMBER
) IS
BEGIN
  
    INSERT INTO AGRICULTURAL_AGENT.Receipt (typeOfReceipt, priceTotal, discount, note)
    VALUES (p_typeOfReceipt, p_priceTotal, p_discount, p_note)
    RETURNING receiptId INTO p_receiptId;

    COMMIT;  
END;
/
CREATE OR REPLACE PROCEDURE proc_DeleteProduct(
    p_productId IN NUMBER,
    p_result OUT VARCHAR2
)
AUTHID CURRENT_USER
IS
    v_exists NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_exists
    FROM AGRICULTURAL_AGENT.ReceiptDetail
    WHERE productId = p_productId;

    IF v_exists > 0 THEN
        UPDATE AGRICULTURAL_AGENT.Product
        SET IsActive = 0
        WHERE ProductId = p_productId;
    ELSE
        DELETE FROM AGRICULTURAL_AGENT.Product
        WHERE ProductId = p_productId; 
    END IF;
    p_result := 'Xoá sản phẩm thành công.';

EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE = -1031 THEN
            p_result := 'Bạn không thể xoá sản phẩm này vì không có quyền.';
        ELSE
            p_result := 'Lỗi khi xử lý xoá sản phẩm: ' || SQLERRM;
        END IF;
END;
/

CREATE OR REPLACE PROCEDURE AGRICULTURAL_AGENT.proc_DailyRevenueReport (
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
    FROM AGRICULTURAL_AGENT.Receipt R
    JOIN AGRICULTURAL_AGENT.Transactions T ON R.receiptId = T.receiptId
    WHERE 
        R.typeOfReceipt = N'Bán'
        AND TRUNC(T.DateOfImplementation) = TRUNC(p_ReportDate);
END;
/
CREATE OR REPLACE PROCEDURE AGRICULTURAL_AGENT.proc_DailyDebtReport (
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
    FROM AGRICULTURAL_AGENT.Receipt R
    JOIN AGRICULTURAL_AGENT.Transactions T ON R.receiptId = T.receiptId
    WHERE 
        TRUNC(T.DateOfImplementation) = TRUNC(p_ReportDate)
        AND T.repayment < (R.priceTotal - NVL(R.discount, 0));
END;
/
CREATE OR REPLACE PROCEDURE AGRICULTURAL_AGENT.proc_DailyExpenseReport (
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
    FROM AGRICULTURAL_AGENT.Receipt R
    JOIN AGRICULTURAL_AGENT.Transactions T ON R.receiptId = T.receiptId
    WHERE 
        R.typeOfReceipt = N'Mua'
        AND TRUNC(T.DateOfImplementation) = TRUNC(p_ReportDate);
END;

/
CREATE OR REPLACE PROCEDURE AGRICULTURAL_AGENT.proc_InventoryReport
(
    p_report_cursor OUT SYS_REFCURSOR 
)
AS
BEGIN
    OPEN p_report_cursor FOR
        SELECT  
            P.PRODUCTID, 
            P.PRODUCTNAME,
            W.QUANTITY,
            P.SELLINGPRICE,
            (W.QUANTITY * P.SELLINGPRICE) AS INVENTORYVALUE
        FROM 
            AGRICULTURAL_AGENT.PRODUCT P
        JOIN 
            AGRICULTURAL_AGENT.WAREHOUSEINFO W ON P.PRODUCTID = W.PRODUCTID
        WHERE 
            P.ISACTIVE = 1; 
END;
/
create or replace PROCEDURE AGRICULTURAL_AGENT.LockUserAccount (
    p_username IN VARCHAR2
)
AUTHID DEFINER 
IS
    v_user_exists NUMBER;
BEGIN
    -- Kiểm tra xem user có tồn tại hay không
    SELECT COUNT(1) INTO v_user_exists
    FROM ALL_USERS
    WHERE USERNAME = UPPER(p_username);

    IF v_user_exists > 0 THEN
        -- Khóa tài khoản
        EXECUTE IMMEDIATE 'ALTER USER ' || p_username || ' ACCOUNT LOCK';

        DBMS_OUTPUT.PUT_LINE('Tài khoản ' || UPPER(p_username) || ' đã được khóa (LOCK) thành công.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Lỗi: Tài khoản ' || UPPER(p_username) || ' không tồn tại.');
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Lỗi khóa tài khoản ' || UPPER(p_username) || ': ' || SQLERRM);
END LockUserAccount;
/

create or replace PROCEDURE AGRICULTURAL_AGENT.UnlockUserAccount (
    p_username IN VARCHAR2
)
AUTHID DEFINER 
IS
    v_user_exists NUMBER;
BEGIN

    SELECT COUNT(1) INTO v_user_exists
    FROM ALL_USERS
    WHERE USERNAME = UPPER(p_username);

    IF v_user_exists > 0 THEN
        -- Mở khóa tài khoản
        EXECUTE IMMEDIATE 'ALTER USER ' || p_username || ' ACCOUNT UNLOCK';

        DBMS_OUTPUT.PUT_LINE('Tài khoản ' || UPPER(p_username) || ' đã được mở khóa (UNLOCK) thành công.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Lỗi: Tài khoản ' || UPPER(p_username) || ' không tồn tại.');
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Lỗi mở khóa tài khoản ' || UPPER(p_username) || ': ' || SQLERRM);
END UnlockUserAccount;
/
create or replace PROCEDURE                    AGRICULTURAL_AGENT.CreateUsersFromAccount AUTHID DEFINER IS v_role_to_grant VARCHAR2(30);  

v_profile_to_apply VARCHAR2(30); 

 v_user_count NUMBER;  

v_current_profile VARCHAR2(30); 

 v_sql_check_profile VARCHAR2(255); 

v_user_to_grant  VARCHAR2(30); 


BEGIN DBMS_OUTPUT.ENABLE(NULL); 

FOR rec IN ( 
    SELECT a.username, a.pass, a.ID, a.email, a.IsAdmin 
    FROM AGRICULTURAL_AGENT.Account a 
) LOOP 
    v_user_to_grant := UPPER(rec.username); 

    -- Xác định role và profile 
    IF rec.IsAdmin = 1 THEN 
        v_role_to_grant := 'CHUDAILY'; 
        v_profile_to_apply := NULL; -- không gán profile 
    ELSE 
        v_role_to_grant := 'QUANLY'; 
        v_profile_to_apply := 'QUANLY_PROFILE'; 
    END IF; 

    -- Kiểm tra user đã tồn tại chưa 
    SELECT COUNT(1) INTO v_user_count 
    FROM ALL_USERS 
    WHERE USERNAME = v_user_to_grant; 

    IF v_user_count = 0 THEN 
        -- Tạo user mới 
        BEGIN 
            EXECUTE IMMEDIATE 'CREATE USER ' || v_user_to_grant || ' IDENTIFIED BY "' || rec.pass || '"'; 

            BEGIN 
                EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || v_user_to_grant; 
            EXCEPTION 
                WHEN OTHERS THEN 
                    DBMS_OUTPUT.PUT_LINE('Không grant CREATE SESSION cho ' || v_user_to_grant || ': ' || SQLERRM); 
            END; 

            BEGIN 
                EXECUTE IMMEDIATE 'GRANT ' || v_role_to_grant || ' TO ' || v_user_to_grant; 
            EXCEPTION 
                WHEN OTHERS THEN 
                    DBMS_OUTPUT.PUT_LINE('Không grant role ' || v_role_to_grant || ' cho ' || v_user_to_grant || ': ' || SQLERRM); 
            END; 

            IF v_profile_to_apply IS NOT NULL THEN 
                BEGIN 
                    EXECUTE IMMEDIATE 'ALTER USER ' || v_user_to_grant || ' PROFILE ' || v_profile_to_apply; 
                EXCEPTION 
                    WHEN OTHERS THEN 
                        DBMS_OUTPUT.PUT_LINE('Không gán profile ' || v_profile_to_apply || ' cho ' || v_user_to_grant || ': ' || SQLERRM); 
                END; 
            END IF; 

            DBMS_OUTPUT.PUT_LINE('Tạo mới thành công: ' || v_user_to_grant); 
        EXCEPTION 
            WHEN OTHERS THEN 
                DBMS_OUTPUT.PUT_LINE('Lỗi tạo user ' || v_user_to_grant || ': ' || SQLERRM); 
        END; 
    ELSE 
        -- User đã tồn tại: grant CREATE SESSION và cập nhật profile nếu cần 
        BEGIN 
            EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || v_user_to_grant; 
        EXCEPTION 
            WHEN OTHERS THEN 
                NULL; -- nếu không đủ quyền grant 
        END; 

        IF v_profile_to_apply IS NOT NULL THEN 
            BEGIN 
                v_sql_check_profile := 'SELECT PROFILE FROM ALL_USERS WHERE USERNAME = ''' || v_user_to_grant || ''''; 
                EXECUTE IMMEDIATE v_sql_check_profile INTO v_current_profile; 

                IF v_current_profile <> v_profile_to_apply THEN 
                    BEGIN 
                        EXECUTE IMMEDIATE 'ALTER USER ' || v_user_to_grant || ' PROFILE ' || v_profile_to_apply; 
                        DBMS_OUTPUT.PUT_LINE('Cập nhật profile ' || v_profile_to_apply || ' cho: ' || v_user_to_grant); 
                    EXCEPTION 
                        WHEN OTHERS THEN 
                            DBMS_OUTPUT.PUT_LINE('Không cập nhật profile cho ' || v_user_to_grant || ': ' || SQLERRM); 
                    END; 
                END IF; 
            EXCEPTION 
                WHEN OTHERS THEN 
                    DBMS_OUTPUT.PUT_LINE('Không kiểm tra profile cho ' || v_user_to_grant || ': ' || SQLERRM); 
            END; 
        END IF; 
    END IF; 
END LOOP; 


END; 