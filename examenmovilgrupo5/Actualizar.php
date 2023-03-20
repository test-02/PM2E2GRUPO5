<?php
    $hostname = "localhost";
    $database = "dbpm02examen";
    $username = "root";
    $password = "";

    $conexion=mysqli_connect($hostname, $username, $password, $database);

    if (!$conexion) {
        die("La conexión ha fallado: " . mysqli_connect_error());
    } 
    else
    {
    $id = $_POST['id'];
    $descripcion = $_POST['descripcion'];
    $latitud = $_POST['latitud'];
    $longitud = $_POST['longitud'];
    $firmadigital = $_POST['firmadigital'];
    $trazado = $_POST['trazado'];

    $sql = "UPDATE `sitios` SET descripcion='$descripcion', latitud='$latitud', longitud='$longitud', firmadigital='$firmadigital', trazado='$trazado' WHERE id='$id'";
    $result = mysqli_query($conexion, $sql);

    if (!$result) {
        echo "Los datos han sido actualizados correctamente.";
    } else {
        echo "Ha ocurrido un error al actualizar los datos: " . mysqli_error($conexion);
    }

    mysqli_close($conexion);
}

?>
