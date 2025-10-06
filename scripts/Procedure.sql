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

    INSERT INTO PRODUCT (PRODUCTNAME, PURCHASEPRICE, SELLINGPRICE, QUALITYSTANDARD, PHOTO, ISACTIVE)
    VALUES (v_name, p_purchasePrice, p_sellingPrice, v_qs, p_photo, 1)
    RETURNING PRODUCTID INTO p_newProductId;

    INSERT INTO WAREHOUSEINFO(PRODUCTID, QUANTITY, MEASUREMENTUNIT)
    VALUES (p_newProductId, p_quantityInStock, v_unit);

    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20010, 'Lỗi khi thêm sản phẩm: ' || SUBSTR(SQLERRM, 1, 150));
END proc_AddProduct;
