-- TRIGGERS PARA DB_TIENDA_PC

USE DB_TIENDA_PC

--TRIGGER PARA controlar y actualizar Stock--
CREATE TRIGGER dis_updStock
ON DETALLES_PEDIDOS
AFTER Insert
AS
BEGIN
	IF EXISTS (SELECT 1 FROM inserted I		
				JOIN Componentes C ON C.id_componente = I.id_componente
				WHERE I.cantidad > C.stock
				)
			BEGIN
		       --Si la cantidad que se pide supera el stock disponible, revertir transaccion
				ROLLBACK TRANSACTION;
				THROW 50000, 'No hay suficiente stock para la venta.', 1;
				 RETURN;--termina el proceso
			END

	Update C
	SET stock = C.stock - I.cantidad
	FROM Componentes C 
	JOIN inserted I ON C.id_componente = I.id_componente;

END;

-- TRIGGER PARA CUANDO UN PEDIDO SE CANCELA RESTAURAR EL STOCK
CREATE TRIGGER trg_reestablecerStock
ON PEDIDOS
AFTER UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN deleted D ON D.id_pedido = I.id_pedido
        WHERE I.estado = 'Cancelado'
        AND D.estado = 'Pendiente'
    )
    BEGIN
        -- Actualiza el stock sumando la cantidad de los detalles del pedido
        UPDATE C
        SET C.stock = C.stock + DP.cantidad
        FROM DETALLES_PEDIDOS DP
        JOIN inserted I ON I.id_pedido = DP.id_pedido
        JOIN Componentes C ON C.id_componente = DP.id_componente;
    END
END;


-- PROCEDURE QUE SE ENCARGA DE LA CANCELACIÓN DE UN PEDIDO 
CREATE PROCEDURE SP_LOW_ORDER
(
	@id_pedido int,
	@motivoBaja varchar(100)
)
AS
BEGIN
	UPDATE PEDIDOS SET estado = 'Cancelado', @motivoBaja = @motivoBaja, fechaCancelacion = GETDATE() WHERE id_pedido = @id_pedido
END


SELECT * FROM ESPECIFICACIONES

-- PROCEDURE QUE PERMITE LA CREACIÓN DE UNA NUEVA ESPECIFICACIÓN
CREATE PROCEDURE SP_NUEVA_ESPECIFICACION
(
	@especificacion varchar(100)
)
AS
BEGIN
	INSERT INTO ESPECIFICACIONES (especificacion) VALUES (@especificacion)
END;

EXEC SP_NUEVA_ESPECIFICACION 'Cantidad De Hilos'

-- TRIGGER QUE SE ASEGURA DE QUE NO SE CARGUE UNA ESPECIFICACIÓN CON UN NOMBRE REPETIDO
CREATE TRIGGER trg_VerificarEspecificaciones
ON ESPECIFICACIONES
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @id_spec INT;

    -- Verificar si ya existe una especificación con el mismo nombre
    SELECT @id_spec = e.id_espec
    FROM INSERTED i
    JOIN ESPECIFICACIONES e ON i.especificacion = e.especificacion;

    IF @id_spec IS NOT NULL
    BEGIN
        -- Si la especificación ya existe, lanzar un error con el id_spec existente
        RAISERROR ('Ya existe una especificación con ese nombre. ID de la especificación existente: %d', 16, 1, @id_spec);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    -- Si pasa la validación, permitir el insert
    INSERT INTO ESPECIFICACIONES(especificacion)
    SELECT i.especificacion
    FROM INSERTED i;
END;

-- PROCEDURE QUE CREA O MODIFICA LAS ESPECIFIACIONES PARA UN COMPONENTE
CREATE PROCEDURE SP_SAVE_ESPECIFIACION_COMPONENTES
(
	@id_spec_comp int,
	@id_comp int,
	@id_spec int,
	@valor varchar(100)
)
AS
BEGIN
	IF (@id_spec_comp = 0)
		BEGIN
			INSERT INTO Especificaciones_Componentes (id_componente,id_espec,valor) VALUES (@id_comp,@id_spec,@valor)	
		END
	ELSE
		BEGIN
			UPDATE Especificaciones_Componentes SET id_componente = @id_comp,id_espec = @id_spec,valor = @valor WHERE @id_spec_comp = @id_spec_comp
		END
END;

---TRIGGER PARA ALMACENAR LA RELACIÓN ENTRE EL TIPO DE COMPONENTE Y SUS CARACTERISTICAS---
CREATE TRIGGER TRG_TIPO_COMP_ESPEC
ON Especificaciones_Componentes
AFTER INSERT
AS
BEGIN
    DECLARE @id_componente INT, @id_espec INT, @id_tipo_componente INT;

    -- Obtener los valores del nuevo registro insertado en Especificaciones_Componentes
    SELECT @id_componente = id_componente, @id_espec = id_espec
    FROM INSERTED;

    -- Obtener el id_tipo_componente correspondiente del componente insertado
    SELECT @id_tipo_componente = id_tipo_componente
    FROM Componentes
    WHERE id_componente = @id_componente;

    -- Verificar si la especificación para el tipo de componente ya existe
    IF NOT EXISTS (
        SELECT 1
        FROM TIPOS_COMPONENTES_ESPECIFICACIONES
        WHERE id_tipo_componente = @id_tipo_componente AND id_espec = @id_espec
    )
    BEGIN
        -- Insertar la nueva especificación en TIPOS_COMPONENTES_ESPECIFICACIONES si no existe
        INSERT INTO TIPOS_COMPONENTES_ESPECIFICACIONES (id_tipo_componente, id_espec)
        VALUES (@id_tipo_componente, @id_espec);
    END
END;
SELECT
	TC.id_tipo_componente,
	TC.tipo 'TIPO COMPONENTE',
	E.id_espec,
	E.especificacion 'ESPECIFICACIÓN'
FROM TIPOS_COMPONENTES_ESPECIFICACIONES TCE
JOIN Tipos_Componentes TC ON TC.id_tipo_componente = TCE.id_tipo_componente
JOIN ESPECIFICACIONES E ON E.id_espec = TCE.id_espec
WHERE TC.id_tipo_componente = 3

SELECT * FROM Componentes WHERE id_componente = 5

EXEC SP_SAVE_ESPECIFIACION_COMPONENTES 0,5,31,'12 hilos'


-- POR DEFECTO CUANDO SE CREA UN NUEVO CLIENTE TIENE 0 GAMER COINS Y ESTADO EN 1.
CREATE PROCEDURE SP_NUEVO_CLIENTE
(
	@nombre varchar(50),
	@apellido varchar(50),
	@email varchar(100),
	@direccion varchar(100),
	@nro_calle int,
	@id_tipo_doc int,
	@id_barrio int,
	@documento int
)
AS
BEGIN
	INSERT INTO CLIENTES (nombre,apellido,email,id_tipo_doc,documento,id_barrio,direccion,nro_calle,estado,gamer_coins) 
	VALUES (@nombre,@apellido,@email,@id_tipo_doc,@documento,@id_barrio,@direccion,@nro_calle,1,0)
END;

-- TRIGGER QUE IMPIDE LA CARGA DE UN CLIENTE CON EL MISMO DOCUMENTO Y TIPO DE DOCUMENTO
CREATE TRIGGER trg_clienteValidarDOCUMENTO
ON Clientes
INSTEAD OF INSERT
AS
BEGIN 
    if exists (
         SELECT 1
         FROM CLIENTES C
         WHERE C.documento = (SELECT documento FROM inserted)
         AND C.id_tipo_doc = (SELECT id_tipo_doc FROM inserted)
    )
    begin
        RAISERROR('Este documento ya está asignado a un cliente, verifique el campo documento',16,1)
        ROLLBACK TRANSACTION
        RETURN;
    end


    --si no hay drama insertamos el cliente
    INSERT INTO CLIENTES(id_barrio, id_tipo_doc, documento, nombre, apellido, direccion, nro_calle, estado, email,gamer_coins)
            SELECT I.id_barrio, I.id_tipo_doc, I.documento, I.nombre, I.apellido, I.direccion, I.nro_calle, 1, I.email,0
            FROM inserted I
END;

--INSERT INTO CLIENTES(id_barrio, id_tipo_doc, documento, nombre, apellido, direccion, nro_calle, estado, email)
--            VALUES (3, 1, 45454123, 'Valentino', 'Pretto', 'Avenida Colon', 1897, 1, 'valentin')

--SELECT * FROM CLIENTES


-- FUNCIÓN PARA A PARTIR DEL MONTO TOTAL DE LA COMPRA Y LA CANTIDAD, ASIGNAR UN VALOR DE GAMER COINS AL USUARIO
CREATE FUNCTION Calcular_Gamer_Coins
(
	@total decimal(18,2),
	@cantidad int
)
RETURNS int
AS
BEGIN
	-- Definir los pesos
    DECLARE @PesoImporte DECIMAL(5,2) = 0.1;
    DECLARE @PesoCantidad DECIMAL(5,2) = 0.9;
    DECLARE @SumaPesos DECIMAL(5,2) = @PesoImporte + @PesoCantidad;
	DECLARE @GAMER_COINS int = ROUND(((@total * @PesoImporte) / (@SumaPesos * 100) ) + ((@cantidad * @pesoCantidad) / @SumaPesos),0)
	RETURN @GAMER_COINS
END;

-- PROCEDURE QUE NOS PERMITE ASIGNAR LAS GAMER COINS DESPUES DE UNA COMPRA Y UTILIZA LA FUNCIÓN CALCULAR_GAMER_COINS.
CREATE PROCEDURE SP_ASIGNAR_GAMER_COINS
(
	@ID_CLIENTE INT,
	@IMPORTE DECIMAL(18,2),
	@CANTIDAD INT
)
AS
BEGIN
	DECLARE @GAMER_COINS INT = dbo.Calcular_Gamer_Coins(@IMPORTE,@CANTIDAD);
	UPDATE CLIENTES SET gamer_coins = @GAMER_COINS WHERE id_cliente = @ID_CLIENTE;
END;

-- PROCEDURE QUE NOS PERMITE DESCONTAR LAS GAMER COINS, SI FUERON UTILIZADAS EN UNA COMPRA.

CREATE PROCEDURE SP_DESCONTAR_GAMER_COINS
(
	@ID_CLIENTE INT
)
AS
BEGIN
	UPDATE CLIENTES SET gamer_coins = 0 WHERE id_cliente = @ID_CLIENTE;
END;



SELECT * FROM CLIENTES
select * from TIPOS_DOCUMENTOS

SELECT * FROM PEDIDOS P
JOIN DETALLES_PEDIDOS DP ON P.id_pedido = DP.id_pedido
WHERE P.id_pedido = 11

SELECT * FROM Componentes WHERE id_componente = 3