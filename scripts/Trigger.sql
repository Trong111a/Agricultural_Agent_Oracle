CREATE OR REPLACE TRIGGER tr_UpdateWarehouseAfterTransaction
AFTER INSERT ON Transactions
FOR EACH ROW
DECLARE
BEGIN
    -- Giảm tồn kho với hóa đơn bán (typeOfReceipt = 'Bán')
    UPDATE WarehouseInfo w
    SET w.quantity = w.quantity -
        (
            SELECT rd.quantity
            FROM ReceiptDetail rd
            JOIN Receipt r ON r.receiptId = rd.receiptId
            WHERE rd.productId = w.productId
              AND r.receiptId = :NEW.receiptId
              AND r.typeOfReceipt = 'Bán'
        )
    WHERE EXISTS (
        SELECT 1
        FROM ReceiptDetail rd
        JOIN Receipt r ON r.receiptId = rd.receiptId
        WHERE rd.productId = w.productId
          AND r.receiptId = :NEW.receiptId
          AND r.typeOfReceipt = 'Bán'
    );
END;
/
CREATE OR REPLACE TRIGGER TRG_ReceiptDetail_AfterInsert_Update_ValidateStock
AFTER INSERT OR UPDATE ON ReceiptDetail
FOR EACH ROW
DECLARE
    v_typeOfReceipt NVARCHAR2(10);
    v_stock NUMBER;
BEGIN
    -- Lấy loại hóa đơn của dòng chi tiết hiện tại
    SELECT r.typeOfReceipt
    INTO v_typeOfReceipt
    FROM Receipt r
    WHERE r.receiptId = :NEW.receiptId;

    -- Chỉ xử lý nếu là hóa đơn BÁN
    IF v_typeOfReceipt = 'Bán' THEN
        -- Lấy số lượng tồn kho hiện tại của sản phẩm
        SELECT w.quantity
        INTO v_stock
        FROM WarehouseInfo w
        WHERE w.productId = :NEW.productId;

        -- Kiểm tra tồn kho
        IF :NEW.quantity > v_stock THEN
            RAISE_APPLICATION_ERROR(-20001, 'Không đủ hàng trong kho để bán. Vui lòng kiểm tra tồn kho.');
        END IF;
    END IF;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20002, 'Không tìm thấy sản phẩm hoặc hóa đơn hợp lệ để kiểm tra tồn kho.');
END;
/


--CREATE OR REPLACE TRIGGER tr_check_before_delete_employee
--BEFORE DELETE ON Employee
--FOR EACH ROW
--DECLARE
--    v_has_transaction NUMBER;
--BEGIN
--    -- Kiểm tra xem nhân viên có giao dịch không
--    SELECT COUNT(*)
--    INTO v_has_transaction
--    FROM Transactions t
--    WHERE t.employeeId = :OLD.employeeId;
--
--    IF v_has_transaction > 0 THEN
--        -- Nếu có giao dịch, chỉ đánh dấu không hoạt động
--        UPDATE Employee
--        SET IsActive = 0
--        WHERE employeeId = :OLD.employeeId;
--
--        -- Ngăn chặn xóa
--        --RAISE_APPLICATION_ERROR(-20001, 'Không thể xoá nhân viên này.');
--    END IF;
--END;
/



