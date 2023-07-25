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
   pdflatex InformeLatex
   pdflatex InformeLatex
   ;;
   "slides")
   echo "compilando y generando el pdf de la presentación..."
   cd ..
   cd presentación
   pdflatex PresentaciónLatex
   pdflatex PresentaciónLatex
   ;;
   "show_report")
   echo "visualizando el informe..."
   cd ..
   cd informe
   if test -e InformeLatex.pdf; then
    xdg-open InformeLatex.pdf
   else
    pdflatex InformeLatex
    pdflatex InformeLatex
    xdg-open InformeLatex.pdf
   fi
   ;;
   "show_slides")
   echo "visualizando la presentación..."
   cd ..
   cd presentación
   if test -e PresentaciónLatex.pdf; then
    xdg-open PresentaciónLatex.pdf
   else
    pdflatex PresentaciónLatex
    pdflatex PresentaciónLatex
    xdg-open PresentaciónLatex.pdf
   fi
   ;;
   "clean")
   echo "eliminados todos los ficheros auxiliares"
   cd ..
   cd informe
   rm -f InformeLatex.aux InformeLatex.log InformeLatex.toc
   cd ..
   cd presentación
   rm -f PresentaciónLatex.aux PresentaciónLatex.log PresentaciónLatex.nav PresentaciónLatex.out PresentaciónLatex.snm PresentaciónLatex.toc
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
