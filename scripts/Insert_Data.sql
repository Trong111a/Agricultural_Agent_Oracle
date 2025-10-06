SET SERVEROUTPUT ON;

DECLARE
    v_new_id          NUMBER;
    v_photo_blob      BLOB := NULL;
    
    TYPE Product_Rec IS RECORD (
        p_name      NVARCHAR2(100),
        p_pprice    NUMBER,
        p_sprice    NUMBER,
        p_qstandard NVARCHAR2(50),
        p_qty       NUMBER,
        p_unit      NVARCHAR2(30)
    );
    TYPE Product_Table IS TABLE OF Product_Rec INDEX BY PLS_INTEGER;
    
    products Product_Table;
    
BEGIN
    products(1).p_name := N'Gạo ST25'; products(1).p_pprice := 12000; products(1).p_sprice := 16000; products(1).p_qstandard := N'VietGAP'; products(1).p_qty := 100; products(1).p_unit := N'kg';
    products(2).p_name := N'Gạo Nàng Hương'; products(2).p_pprice := 15000; products(2).p_sprice := 19000; products(2).p_qstandard := N'Hữu cơ'; products(2).p_qty := 120; products(2).p_unit := N'kg';
    products(3).p_name := N'Khoai lang tím'; products(3).p_pprice := 8000; products(3).p_sprice := 11000; products(3).p_qstandard := N'VietGAP'; products(3).p_qty := 80; products(3).p_unit := N'kg';
    products(4).p_name := N'Sầu riêng Ri6'; products(4).p_pprice := 70000; products(4).p_sprice := 85000; products(4).p_qstandard := N'VietGAP'; products(4).p_qty := 50; products(4).p_unit := N'kg';
    products(5).p_name := N'Mít Thái'; products(5).p_pprice := 20000; products(5).p_sprice := 27000; products(5).p_qstandard := N'Hữu cơ'; products(5).p_qty := 60; products(5).p_unit := N'kg';
    products(6).p_name := N'Cam sành'; products(6).p_pprice := 18000; products(6).p_sprice := 25000; products(6).p_qstandard := N'VietGAP'; products(6).p_qty := 90; products(6).p_unit := N'kg';
    products(7).p_name := N'Xoài Cát Hòa Lộc'; products(7).p_pprice := 30000; products(7).p_sprice := 40000; products(7).p_qstandard := N'GlobalGAP'; products(7).p_qty := 70; products(7).p_unit := N'kg';
    products(8).p_name := N'Chôm chôm nhãn'; products(8).p_pprice := 22000; products(8).p_sprice := 28000; products(8).p_qstandard := N'VietGAP'; products(8).p_qty := 85; products(8).p_unit := N'kg';
    products(9).p_name := N'Bưởi da xanh'; products(9).p_pprice := 35000; products(9).p_sprice := 45000; products(9).p_qstandard := N'Hữu cơ'; products(9).p_qty := 40; products(9).p_unit := N'kg';
    products(10).p_name := N'Thanh long ruột đỏ'; products(10).p_pprice := 17000; products(10).p_sprice := 23000; products(10).p_qstandard := N'VietGAP'; products(10).p_qty := 95; products(10).p_unit := N'kg';
    products(11).p_name := N'Dưa hấu không hạt'; products(11).p_pprice := 9000; products(11).p_sprice := 13000; products(11).p_qstandard := N'VietGAP'; products(11).p_qty := 100; products(11).p_unit := N'kg';
    products(12).p_name := N'Dưa lưới Nhật'; products(12).p_pprice := 40000; products(12).p_sprice := 55000; products(12).p_qstandard := N'Hữu cơ'; products(12).p_qty := 35; products(12).p_unit := N'kg';
    products(13).p_name := N'Rau muống sạch'; products(13).p_pprice := 6000; products(13).p_sprice := 9000; products(13).p_qstandard := N'VietGAP'; products(13).p_qty := 200; products(13).p_unit := N'bó';
    products(14).p_name := N'Cải bó xôi'; products(14).p_pprice := 7000; products(14).p_sprice := 10000; products(14).p_qstandard := N'GlobalGAP'; products(14).p_qty := 150; products(14).p_unit := N'bó';
    products(15).p_name := N'Cà chua Đà Lạt'; products(15).p_pprice := 10000; products(15).p_sprice := 14000; products(15).p_qstandard := N'Hữu cơ'; products(15).p_qty := 110; products(15).p_unit := N'kg';
    products(16).p_name := N'Cà rốt'; products(16).p_pprice := 9000; products(16).p_sprice := 12000; products(16).p_qstandard := N'VietGAP'; products(16).p_qty := 100; products(16).p_unit := N'kg';
    products(17).p_name := N'Tỏi Lý Sơn'; products(17).p_pprice := 25000; products(17).p_sprice := 32000; products(17).p_qstandard := N'Hữu cơ'; products(17).p_qty := 45; products(17).p_unit := N'kg';
    products(18).p_name := N'Hành tím Vĩnh Châu'; products(18).p_pprice := 20000; products(18).p_sprice := 26000; products(18).p_qstandard := N'VietGAP'; products(18).p_qty := 60; products(18).p_unit := N'kg';
    products(19).p_name := N'Ớt hiểm đỏ'; products(19).p_pprice := 15000; products(19).p_sprice := 20000; products(19).p_qstandard := N'VietGAP'; products(19).p_qty := 75; products(19).p_unit := N'kg';
    products(20).p_name := N'Dưa leo'; products(20).p_pprice := 7000; products(20).p_sprice := 10000; products(20).p_qstandard := N'VietGAP'; products(20).p_qty := 90; products(20).p_unit := N'kg';
    products(21).p_name := N'Củ cải trắng'; products(21).p_pprice := 8000; products(21).p_sprice := 11000; products(21).p_qstandard := N'Hữu cơ'; products(21).p_qty := 85; products(21).p_unit := N'kg';
    products(22).p_name := N'Súp lơ xanh'; products(22).p_pprice := 16000; products(22).p_sprice := 22000; products(22).p_qstandard := N'VietGAP'; products(22).p_qty := 55; products(22).p_unit := N'bó';
    products(23).p_name := N'Bắp cải tím'; products(23).p_pprice := 10000; products(23).p_sprice := 15000; products(23).p_qstandard := N'GlobalGAP'; products(23).p_qty := 65; products(23).p_unit := N'kg';
    products(24).p_name := N'Dưa gang'; products(24).p_pprice := 8500; products(24).p_sprice := 12000; products(24).p_qstandard := N'VietGAP'; products(24).p_qty := 100; products(24).p_unit := N'kg';
    products(25).p_name := N'Nấm rơm'; products(25).p_pprice := 30000; products(25).p_sprice := 40000; products(25).p_qstandard := N'Hữu cơ'; products(25).p_qty := 30; products(25).p_unit := N'kg';
    products(26).p_name := N'Nấm bào ngư'; products(26).p_pprice := 28000; products(26).p_sprice := 37000; products(26).p_qstandard := N'VietGAP'; products(26).p_qty := 40; products(26).p_unit := N'kg';
    products(27).p_name := N'Củ dền đỏ'; products(27).p_pprice := 10000; products(27).p_sprice := 13000; products(27).p_qstandard := N'GlobalGAP'; products(27).p_qty := 70; products(27).p_unit := N'kg';
    products(28).p_name := N'Sắn dây'; products(28).p_pprice := 15000; products(28).p_sprice := 20000; products(28).p_qstandard := N'Hữu cơ'; products(28).p_qty := 55; products(28).p_unit := N'kg';
    products(29).p_name := N'Lá lốt'; products(29).p_pprice := 4000; products(29).p_sprice := 7000; products(29).p_qstandard := N'VietGAP'; products(29).p_qty := 150; products(29).p_unit := N'bó';
    products(30).p_name := N'Diếp cá'; products(30).p_pprice := 5000; products(30).p_sprice := 8000; products(30).p_qstandard := N'Hữu cơ'; products(30).p_qty := 140; products(30).p_unit := N'bó';
    products(31).p_name := N'Rau dền cơm'; products(31).p_pprice := 6000; products(31).p_sprice := 9000; products(31).p_qstandard := N'VietGAP'; products(31).p_qty := 120; products(31).p_unit := N'bó';
    products(32).p_name := N'Cải thìa'; products(32).p_pprice := 7000; products(32).p_sprice := 10000; products(32).p_qstandard := N'Hữu cơ'; products(32).p_qty := 130; products(32).p_unit := N'bó';
    products(33).p_name := N'Rau ngót'; products(33).p_pprice := 5000; products(33).p_sprice := 8000; products(33).p_qstandard := N'VietGAP'; products(33).p_qty := 140; products(33).p_unit := N'bó';
    products(34).p_name := N'Rau mồng tơi'; products(34).p_pprice := 6000; products(34).p_sprice := 9000; products(34).p_qstandard := N'Hữu cơ'; products(34).p_qty := 110; products(34).p_unit := N'bó';
    products(35).p_name := N'Cải bẹ xanh'; products(35).p_pprice := 7000; products(35).p_sprice := 10000; products(35).p_qstandard := N'GlobalGAP'; products(35).p_qty := 100; products(35).p_unit := N'bó';
    products(36).p_name := N'Đậu bắp'; products(36).p_pprice := 8000; products(36).p_sprice := 11000; products(36).p_qstandard := N'VietGAP'; products(36).p_qty := 95; products(36).p_unit := N'kg';
    products(37).p_name := N'Bí đỏ'; products(37).p_pprice := 9000; products(37).p_sprice := 12000; products(37).p_qstandard := N'Hữu cơ'; products(37).p_qty := 90; products(37).p_unit := N'kg';
    products(38).p_name := N'Bí xanh'; products(38).p_pprice := 10000; products(38).p_sprice := 13000; products(38).p_qstandard := N'VietGAP'; products(38).p_qty := 85; products(38).p_unit := N'kg';
    products(39).p_name := N'Dưa chuột'; products(39).p_pprice := 7500; products(39).p_sprice := 10000; products(39).p_qstandard := N'VietGAP'; products(39).p_qty := 90; products(39).p_unit := N'kg';
    products(40).p_name := N'Bầu sao'; products(40).p_pprice := 8000; products(40).p_sprice := 11000; products(40).p_qstandard := N'Hữu cơ'; products(40).p_qty := 80; products(40).p_unit := N'kg';
    products(41).p_name := N'Khổ qua'; products(41).p_pprice := 8500; products(41).p_sprice := 11500; products(41).p_qstandard := N'GlobalGAP'; products(41).p_qty := 70; products(41).p_unit := N'kg';
    products(42).p_name := N'Cà tím'; products(42).p_pprice := 9000; products(42).p_sprice := 12000; products(42).p_qstandard := N'VietGAP'; products(42).p_qty := 75; products(42).p_unit := N'kg';
    products(43).p_name := N'Củ hành tây'; products(43).p_pprice := 10000; products(43).p_sprice := 14000; products(43).p_qstandard := N'Hữu cơ'; products(43).p_qty := 65; products(43).p_unit := N'kg';
    products(44).p_name := N'Củ gừng'; products(44).p_pprice := 12000; products(44).p_sprice := 16000; products(44).p_qstandard := N'VietGAP'; products(44).p_qty := 60; products(44).p_unit := N'kg';
    products(45).p_name := N'Củ nghệ'; products(45).p_pprice := 13000; products(45).p_sprice := 17000; products(45).p_qstandard := N'Hữu cơ'; products(45).p_qty := 55; products(45).p_unit := N'kg';
    products(46).p_name := N'Nghệ tươi'; products(46).p_pprice := 12000; products(46).p_sprice := 15000; products(46).p_qstandard := N'VietGAP'; products(46).p_qty := 70; products(46).p_unit := N'kg';
    products(47).p_name := N'Củ sắn'; products(47).p_pprice := 6000; products(47).p_sprice := 9000; products(47).p_qstandard := N'VietGAP'; products(47).p_qty := 100; products(47).p_unit := N'kg';
    products(48).p_name := N'Chuối xiêm'; products(48).p_pprice := 9000; products(48).p_sprice := 12000; products(48).p_qstandard := N'Hữu cơ'; products(48).p_qty := 80; products(48).p_unit := N'nải';
    products(49).p_name := N'Chuối cau'; products(49).p_pprice := 10000; products(49).p_sprice := 13000; products(49).p_qstandard := N'VietGAP'; products(49).p_qty := 85; products(49).p_unit := N'nải';
    products(50).p_name := N'Táo Mỹ'; products(50).p_pprice := 30000; products(50).p_sprice := 40000; products(50).p_qstandard := N'GlobalGAP'; products(50).p_qty := 60; products(50).p_unit := N'kg';
    products(51).p_name := N'Nho đen không hạt'; products(51).p_pprice := 35000; products(51).p_sprice := 45000; products(51).p_qstandard := N'GlobalGAP'; products(51).p_qty := 70; products(51).p_unit := N'kg';
    products(52).p_name := N'Kiwi New Zealand'; products(52).p_pprice := 40000; products(52).p_sprice := 50000; products(52).p_qstandard := N'Hữu cơ'; products(52).p_qty := 55; products(52).p_unit := N'kg';
    products(53).p_name := N'Cam Mỹ'; products(53).p_pprice := 28000; products(53).p_sprice := 35000; products(53).p_qstandard := N'GlobalGAP'; products(53).p_qty := 65; products(53).p_unit := N'kg';
    products(54).p_name := N'Xoài keo'; products(54).p_pprice := 20000; products(54).p_sprice := 25000; products(54).p_qstandard := N'VietGAP'; products(54).p_qty := 90; products(54).p_unit := N'kg';
    products(55).p_name := N'Ổi lê'; products(55).p_pprice := 15000; products(55).p_sprice := 20000; products(55).p_qstandard := N'Hữu cơ'; products(55).p_qty := 100; products(55).p_unit := N'kg';
    products(56).p_name := N'Ổi ruột đỏ'; products(56).p_pprice := 16000; products(56).p_sprice := 21000; products(56).p_qstandard := N'VietGAP'; products(56).p_qty := 110; products(56).p_unit := N'kg';
    products(57).p_name := N'Quýt đường'; products(57).p_pprice := 18000; products(57).p_sprice := 24000; products(57).p_qstandard := N'GlobalGAP'; products(57).p_qty := 95; products(57).p_unit := N'kg';
    products(58).p_name := N'Lê Hàn Quốc'; products(58).p_pprice := 35000; products(58).p_sprice := 45000; products(58).p_qstandard := N'Hữu cơ'; products(58).p_qty := 50; products(58).p_unit := N'kg';
    products(59).p_name := N'Mận hậu'; products(59).p_pprice := 20000; products(59).p_sprice := 27000; products(59).p_qstandard := N'VietGAP'; products(59).p_qty := 80; products(59).p_unit := N'kg';
    products(60).p_name := N'Dâu tây Đà Lạt'; products(60).p_pprice := 40000; products(60).p_sprice := 55000; products(60).p_qstandard := N'GlobalGAP'; products(60).p_qty := 40; products(60).p_unit := N'kg';
    products(61).p_name := N'Măng cụt'; products(61).p_pprice := 30000; products(61).p_sprice := 42000; products(61).p_qstandard := N'Hữu cơ'; products(61).p_qty := 50; products(61).p_unit := N'kg';
    products(62).p_name := N'Vú sữa Lò Rèn'; products(62).p_pprice := 28000; products(62).p_sprice := 35000; products(62).p_qstandard := N'VietGAP'; products(62).p_qty := 60; products(62).p_unit := N'kg';
    products(63).p_name := N'Mít tố nữ'; products(63).p_pprice := 24000; products(63).p_sprice := 30000; products(63).p_qstandard := N'Hữu cơ'; products(63).p_qty := 70; products(63).p_unit := N'kg';
    products(64).p_name := N'Mãng cầu ta'; products(64).p_pprice := 26000; products(64).p_sprice := 33000; products(64).p_qstandard := N'VietGAP'; products(64).p_qty := 75; products(64).p_unit := N'kg';
    products(65).p_name := N'Mãng cầu xiêm'; products(65).p_pprice := 27000; products(65).p_sprice := 34000; products(65).p_qstandard := N'Hữu cơ'; products(65).p_qty := 65; products(65).p_unit := N'kg';
    products(66).p_name := N'Dưa hấu ruột vàng'; products(66).p_pprice := 9500; products(66).p_sprice := 13000; products(66).p_qstandard := N'VietGAP'; products(66).p_qty := 90; products(66).p_unit := N'kg';
    products(67).p_name := N'Chanh dây'; products(67).p_pprice := 10000; products(67).p_sprice := 14000; products(67).p_qstandard := N'Hữu cơ'; products(67).p_qty := 100; products(67).p_unit := N'kg';
    products(68).p_name := N'Bơ Booth'; products(68).p_pprice := 35000; products(68).p_sprice := 45000; products(68).p_qstandard := N'GlobalGAP'; products(68).p_qty := 45; products(68).p_unit := N'kg';
    products(69).p_name := N'Bơ sáp'; products(69).p_pprice := 30000; products(69).p_sprice := 40000; products(69).p_qstandard := N'VietGAP'; products(69).p_qty := 50; products(69).p_unit := N'kg';
    products(70).p_name := N'Hồng giòn'; products(70).p_pprice := 25000; products(70).p_sprice := 33000; products(70).p_qstandard := N'Hữu cơ'; products(70).p_qty := 60; products(70).p_unit := N'kg';
    products(71).p_name := N'Chôm chôm Java'; products(71).p_pprice := 23000; products(71).p_sprice := 29000; products(71).p_qstandard := N'VietGAP'; products(71).p_qty := 85; products(71).p_unit := N'kg';
    products(72).p_name := N'Thơm mật'; products(72).p_pprice := 15000; products(72).p_sprice := 20000; products(72).p_qstandard := N'Hữu cơ'; products(72).p_qty := 70; products(72).p_unit := N'kg';
    products(73).p_name := N'Đu đủ ruột đỏ'; products(73).p_pprice := 9000; products(73).p_sprice := 12000; products(73).p_qstandard := N'VietGAP'; products(73).p_qty := 90; products(73).p_unit := N'kg';
    products(74).p_name := N'Quả sấu tươi'; products(74).p_pprice := 20000; products(74).p_sprice := 26000; products(74).p_qstandard := N'Hữu cơ'; products(74).p_qty := 40; products(74).p_unit := N'kg';
    products(75).p_name := N'Táo ta'; products(75).p_pprice := 17000; products(75).p_sprice := 23000; products(75).p_qstandard := N'VietGAP'; products(75).p_qty := 65; products(75).p_unit := N'kg';
    products(76).p_name := N'Mơ vàng'; products(76).p_pprice := 25000; products(76).p_sprice := 32000; products(76).p_qstandard := N'Hữu cơ'; products(76).p_qty := 55; products(76).p_unit := N'kg';
    products(77).p_name := N'Rau tía tô'; products(77).p_pprice := 5000; products(77).p_sprice := 8000; products(77).p_qstandard := N'VietGAP'; products(77).p_qty := 150; products(77).p_unit := N'bó';
    products(78).p_name := N'Rau răm'; products(78).p_pprice := 4000; products(78).p_sprice := 7000; products(78).p_qstandard := N'Hữu cơ'; products(78).p_qty := 160; products(78).p_unit := N'bó';
    products(79).p_name := N'Lá chanh'; products(79).p_pprice := 6000; products(79).p_sprice := 9000; products(79).p_qstandard := N'VietGAP'; products(79).p_qty := 130; products(79).p_unit := N'bó';
    
    FOR i IN products.FIRST .. products.LAST LOOP
        BEGIN
            proc_AddProduct(
                p_productName     => products(i).p_name,
                p_purchasePrice   => products(i).p_pprice,
                p_sellingPrice    => products(i).p_sprice,
                p_qualityStandard => products(i).p_qstandard,
                p_quantityInStock => products(i).p_qty,
                p_photo           => v_photo_blob,  -- NULL
                p_measurementUnit => products(i).p_unit,
                p_newProductId    => v_new_id
            );
            
            DBMS_OUTPUT.PUT_LINE('Đã thêm sản phẩm: ' || products(i).p_name || ' (ID: ' || v_new_id || ')');
            
        EXCEPTION
            WHEN OTHERS THEN
        
                DBMS_OUTPUT.PUT_LINE('LỖI khi thêm sản phẩm ' || products(i).p_name || ': ' || SQLERRM);
        END;
    END LOOP;
    
END;
/