# PROYECTO
PROYECTO
> Proyecto de Programación I.

 Para ejecutar el proyecto:
 1- En Moogle.cs, cambiar el valor de la variable 'ContentPath' por la ruta de los archivos txt. 
 2- Cargar el proyecto: Pararse en la carpeta del proyecto, abrir un terminal y usar el comando: 'dotnet build'.
 3- Abrir la carpeta del proyecto con vscode (en la carpeta del proyecto, abrir un terminal y usar el comando: '. code')(o click derecho->abrir con-> vscode).
 4- Dentro de vscode, abrir un nuevo terminal, y ejecutar el proyecto con el comando: 'dotnet watch run --project MoogleServer'.
 5- Cuando abra el navegador, usar el buscador. 
 
 Funcionalidades básicas:
 -El ranking de relevancia es TF-IDF.
 -Las consultas tienen operadores de aparición:
   -> Un símbolo `!` delante de una palabra indica que esa palabra no debe aparecer en ningún documento que sea devuelto.
   -> Un símbolo `^` delante de una palabra indica que esa palabra sí debe aparecer en cualquier documento que sea devuelto.
 -Los documentos solo se cargan con la primera búsqueda de cada ejecución, por lo que es eficiente.
