CREATE OR REPLACE FUNCTION check_delete_product_policy (
    p_schema IN VARCHAR2,
    p_object IN VARCHAR2
) RETURN VARCHAR2 IS
    v_has_role NUMBER := 0;
    v_predicate VARCHAR2(4000);
BEGIN
    -- Kiểm tra xem user hiện tại có role CHUDAILY không
    SELECT COUNT(*) INTO v_has_role
    FROM USER_ROLE_PRIVS
    WHERE GRANTED_ROLE = 'CHUDAILY';

    IF v_has_role = 0 THEN
        RETURN '1=0';  -- Không có quyền
    END IF;

    -- Chỉ cho phép xóa nếu không có productId trong ReceiptDetail
    v_predicate := 'NOT EXISTS (SELECT 1 FROM ' || p_schema ||
                   '.ReceiptDetail rd WHERE rd.productId = PRODUCT.ProductId)';

    RETURN v_predicate;

EXCEPTION
    WHEN OTHERS THEN
        RETURN '1=0';
END;
/

BEGIN
   DBMS_RLS.ADD_POLICY(
      object_schema   => 'AGRICULTURAL_AGENT',          
      object_name     => 'PRODUCT',                      
      policy_name     => 'product_delete_policy',        
      function_schema => 'AGRICULTURAL_AGENT',          
      policy_function => 'check_delete_product_policy',  
      statement_types => 'DELETE',
      update_check    => TRUE     
   );
END;

BEGIN
   DBMS_RLS.DROP_POLICY(
      object_schema   => 'AGRICULTURAL_AGENT',          
      object_name     => 'PRODUCT',                      
      policy_name     => 'product_delete_policy'  
    );
END;

/
CREATE OR REPLACE FUNCTION manager_employee_access (
    schema_name  IN VARCHAR2,
    object_name  IN VARCHAR2
) RETURN VARCHAR2
AS
    v_username     VARCHAR2(50);
    v_is_admin     NUMBER;
    v_employee_id  NUMBER;
    v_role NUMBER;
BEGIN
    -- Lấy user đang login (phải là database user)
    v_username := SYS_CONTEXT('USERENV', 'SESSION_USER');
    BEGIN
    -- Lấy thông tin từ bảng Account
    SELECT isAdmin, id
    INTO v_is_admin, v_employee_id
    FROM Account
    WHERE UPPER(username) = UPPER(v_username);
    END;
    IF v_is_admin = 1 THEN
        RETURN '1=1';  -- admin thấy tất cả
    ELSE
            RETURN 'employeeId = ' || v_employee_id || ' OR position = 2';

    END IF;
EXCEPTION
    WHEN OTHERS THEN
        -- Log the error somewhere if possible or raise for debugging
        RETURN '1=0';
END;

/

BEGIN
   DBMS_RLS.ADD_POLICY(
      object_schema   => 'AGRICULTURAL_AGENT',          
      object_name     => 'EMPLOYEE',                      
      policy_name     => 'manager_staff_policy',        
      function_schema => 'AGRICULTURAL_AGENT',          -- Schema chứa Policy Function
      policy_function => 'manager_employee_access',     -- Tên Policy Function
      statement_types => 'SELECT,UPDATE'             -- Áp dụng cho xem và chỉnh sửa
   );
END;
/
BEGIN
   DBMS_RLS.DROP_POLICY(
      object_schema   => 'AGRICULTURAL_AGENT',          
      object_name     => 'EMPLOYEE',                      
      policy_name     => 'manager_staff_policy'
      );
      END;

/
CREATE OR REPLACE FUNCTION trans_policy_fn (
    schema_name  IN VARCHAR2,
    object_name  IN VARCHAR2
) RETURN VARCHAR2
AS
    v_username     VARCHAR2(50);
    v_is_admin     NUMBER;
    v_employee_id  NUMBER;
BEGIN
    -- Lấy user đang login (phải là database user)
    v_username := SYS_CONTEXT('USERENV', 'SESSION_USER');

    -- Lấy thông tin từ bảng Account
    SELECT isAdmin, id
    INTO v_is_admin, v_employee_id
    FROM Account
    WHERE UPPER(username) = UPPER(v_username);

    IF v_is_admin = 1 THEN
        RETURN '1=1';  -- admin thấy tất cả
    ELSE
        RETURN 'employeeId = ' || v_employee_id;
    END IF;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN '1=0'; -- user không hợp lệ, không được xem gì
END;
/
BEGIN
    DBMS_RLS.ADD_POLICY (
        object_schema   => 'AGRICULTURAL_AGENT',
        object_name     => 'TRANSACTIONS',
        policy_name     => 'transaction_policy',
        function_schema => 'AGRICULTURAL_AGENT',
        policy_function => 'trans_policy_fn',
        statement_types => 'SELECT',
        update_check    => FALSE
    );
END;
/

BEGIN
    DBMS_RLS.DROP_POLICY (
        object_schema   => 'AGRICULTURAL_AGENT',
        object_name     => 'TRANSACTIONS',
        policy_name     => 'transaction_policy'
        );
        END;
/

CREATE OR REPLACE FUNCTION check_confirm_policy ( 

    p_schema IN VARCHAR2, 

    p_object IN VARCHAR2 

) RETURN VARCHAR2 AUTHID CURRENT_USER IS 

    v_has_role NUMBER := 0; 

BEGIN 

    SELECT COUNT(*) INTO v_has_role 

    FROM USER_ROLE_PRIVS 

    WHERE GRANTED_ROLE = 'CHUDAILY'; 

 

    IF v_has_role = 0 THEN 

        RETURN '1=0';   

    ELSE  

        RETURN '1=1'; 

    END IF; 

 

EXCEPTION 

    WHEN OTHERS THEN 

        RETURN '1=0'; 

END; 

/ 
BEGIN 
   DBMS_RLS.ADD_POLICY( 

      object_schema   => 'AGRICULTURAL_AGENT',           

      object_name     => 'TRANSACTIONS',                       

      policy_name     => 'manager_trans_policy',         

      function_schema => 'AGRICULTURAL_AGENT',          -- Schema chứa Policy Function 

      policy_function => 'check_confirm_policy',     -- Tên Policy Function 

      statement_types => 'UPDATE'             -- Áp dụng cho xem và chỉnh sửa 

   ); 

END; 
/
--Chay trong admin
BEGIN
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'AGRICULTURAL_AGENT',  
        object_name     => 'PRODUCT',            
        policy_name     => 'PRODUCT_UPDATE_AUDIT', 
        audit_condition => NULL,                  
        audit_column    => 'PurchasePrice, SellingPrice', ProductName,QualityStandard', 
        statement_types => 'UPDATE',              
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED,
        enable          => TRUE                    
    );
END;
/
