CREATE OR REPLACE VIEW View_NewOracleUsers AS
SELECT a.username, a.pass, a.ID, a.email
FROM Account a
WHERE NOT EXISTS (
    SELECT 1 FROM all_users u WHERE LOWER(u.username) = LOWER(a.username)
);
/
CREATE OR REPLACE PROCEDURE CreateUsersFromAccount IS
BEGIN
    FOR rec IN (SELECT * FROM View_NewOracleUsers) LOOP
        BEGIN
            -- Tạo Oracle user
            EXECUTE IMMEDIATE 'CREATE USER ' || rec.username || ' IDENTIFIED BY "' || rec.pass || '"';
            
        EXCEPTION
            WHEN OTHERS THEN
                DBMS_OUTPUT.PUT_LINE('❌ Lỗi tạo user ' || rec.username || ': ' || SQLERRM);
        END;
    END LOOP;
END;
/

BEGIN
    DBMS_SCHEDULER.create_job (
        job_name        => 'JOBCREATEUSERS',
        job_type        => 'PLSQL_BLOCK',
        job_action      => 'BEGIN CreateUsersFromAccount; END;',
        start_date      => SYSTIMESTAMP,
        repeat_interval => 'FREQ=MINUTELY; INTERVAL=1',
        enabled         => TRUE,
        comments        => 'Tạo Oracle users từ bảng Account'
    );
END;
/

