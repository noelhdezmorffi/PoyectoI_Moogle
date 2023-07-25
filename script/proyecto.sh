#!/bin/bash


opciones=("run" "report" "slides" "show_report" "show_slides" "clean")

select opcion in "${opciones[@]}"
do
   case $opcion in 
   "run")
   echo "ejectutando el proyecto..."
   cd ..
   dotnet build
   dotnet watch run --project MoogleServer
   cd script
   ;;
   "report")
   echo "compilando y generando el pdf del informe..."
   cd ..
   cd informe
   pdflatex fichero
   pdflatex fichero
   ;;
   "slides")
   echo "compilando y generando el pdf de la presentación..."
   cd ..
   cd presentación
   pdflatex fichero
   pdflatex fichero
   ;;
   "show_report")
   echo "visualizando el informe..."
   cd ..
   cd informe
   if test -e fichero.pdf; then
    xdg-open fichero.pdf
   else
    pdflatex fichero
    pdflatex fichero
    xdg-open fichero.pdf
   fi
   ;;
   "show_slides")
   echo "visualizando la presentación..."
   cd ..
   cd presentación
   if test -e fichero.pdf; then
    xdg-open fichero.pdf
   else
    pdflatex fichero
    pdflatex fichero
    xdg-open fichero.pdf
   fi
   ;;
   "clean")
   echo "eliminados todos los ficheros auxiliares"
   cd ..
   cd informe
   rm -f fichero.aux fichero.log fichero.toc fichero.pdf
   cd ..
   cd presentación
   rm -f fichero.aux fichero.log fichero.nav fichero.out fichero.snm fichero.toc fichero.pdf
   cd ..
   cd MoogleEngine
   rm -r -f bin obj
   cd ..
   cd MoogleServer
   rm -r -f bin obj
   ;;
   *)
   echo "Opción no válida."
   esac
done
