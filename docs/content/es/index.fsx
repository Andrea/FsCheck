(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../src/FsCheck/bin/Release"

(**
FsCheck
=======

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      FsCheck, NUnit and xUnit.NET plugin pueden ser <a href="https://www.nuget.org/packages?q=fscheck">instalados desde NuGet</a>:
      <pre>PM> Install-Package FsCheck
PM> Install-Package FsCheck.Nunit
PM> Install-Package FsCheck.Xunit</pre>
    </div>
  </div> 
  <div class="span1"></div>
</div>

Documentation
-----------------------

Esta documentacion tambien disponible en [Japones](ja/index.html)
Esta documentacion tambien disponible y en [Ingles](../index.html)

 * [QuickStart](QuickStart.html) para empezar.

 * [Propiedades](Properties.html) describe el lenguage de FsCheck para realizar pruebas (tests)
   en otros frameworks() estas prubas son llamadas prubas parametrizadas o generativas. En
   FsCheck las llamamos propiedades
   
 * [Generando datos para las pruebas](TestData.html) es una guia de FsCheck que ayuda a 
    generar mejores datos o para hacer que FsCheck deje de generar datos que no tienen sentido
    dado el contexto (lo que esta tratando de probar). FsCheck tiene un lenguage flexible para
    describitr los generadores de valores de prueba, y applicarlos a sus propiedades.

 * [Pruebas basadas en modelos](StatefulTesting.html) es una forma particular de hacer pruebas
    donde FsCheck genera un largo numero de operaciones aleatorias en un objecto o estructura 
    de datos, y los resultados de esa operacion son comparados con un modelo mas simple.

 * [Ejecutando las prubas](RunningTests.html) explica las varias maneras en las que se pueden
    ejecutar las prubas de FsCheck y como integrar los distintos frameworks the pruebas 
    unitarias.

 * [Consejos y trucos](TipsAndTricks.html) 

 * [Referencia de la API ](reference/index.html) contiene documentacion generada automaticamente
    para todos los tipos, modulos y funciones

 
Contributing and copyright
--------------------------

Este projecto esta (hosted?) en [GitHub][gh] donde se pueden [registrar problemas][issues], 
fork(?) el projecto y mandar pull-requests(?). Si esta agreagando a la API publica, por
favor considere aggregar [ejemplos][content] que puedan ser convertidos en documentacion.

Esta libreria esta disponible bajo licencia BSD, que permite modification y redistribucion 
para projectos commerciales y no comerciales. Por mas informacion vea la [Licencia][license] 
en el repositorio de Github.

  [content]: https://github.com/fsharp/FsCheck/tree/master/docs/content
  [gh]: https://github.com/fsharp/FsCheck
  [issues]: https://github.com/fsharp/FsCheck/issues
  [readme]: https://github.com/fsharp/FsCheck/blob/master/README.md
  [license]: https://github.com/fsharp/FsCheck/blob/master/License.txt
*)
