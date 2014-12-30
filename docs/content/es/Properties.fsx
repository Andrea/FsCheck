(*** hide ***)
#I "../../src/FsCheck/bin/Release"
#r "FsCheck"

open FsCheck
open System

(**
# Propiedades

Las propiedades son expresadas como definiciones de funciones en F#. Las propiedades son universalmente
cuantificadas sobre sus parámetros, entonces:
 *)


let revRevIsOrig xs = List.rev(List.rev xs) = xs

(**
significa que la igualdad se mantiene para todas las listas xs.

No es necesario que las propiedades tengan tipos monomórficos. 
Propiedades polimórficas, como la que se muestra en el ejemplo anterior, serán
probadas por FsCheck como si el argumento genérico es de tipo object; esto significa que los
valores de varios tipos simples (bool, char, string, ...) son generados.
Es posible que se de el caso donde una lista generada contiene más de un tipo por ejemplo
`{['r', "1a", true]}` sería una lista que puede ser usada para probar la propiedad en el ejemplo
anterior.

Los valores generados estan basados en el tipo, sin embargo, este comportamiento puede
ser cambiado simplemente dando xs un tipo inferido o explicito diferente: *)

let revRevIsOrigInt (xs:list<int>) = List.rev(List.rev xs) = xs

(** es probado solamente con listas de int.

FsCheck puede chequear propiedades de varias formas - estas formas son llamadas testable,
y son indicadas en la API por un tipo genérico llamado `'Testable`. Un `'Testable` puede
ser una funcion de cualquier numero de parametros que devuelve bool o unit. En elúltimoo
caso, la prueba pasa si no tira una excepción
 
Las propiedades pueden tomar la forma `<condition> ==> <property>`

Por ejemplo,*)

(***hide***)
let rec ordered xs =
  match xs with
  | [] -> true
  | [x] -> true
  | x::y::ys ->  (x <= y) && ordered (y::ys)
let rec insert x xs =
  match xs with
  | [] -> [x]
  | c::cs -> if x <= c then x::xs else c::(insert x cs)

(***define-output:insertKeepsOrder***)
let insertKeepsOrder (x:int) xs = ordered xs ==> ordered (insert x xs)
Check.Quick insertKeepsOrder

(***include-output:insertKeepsOrder***)

(**
## Propiedades Condicionales

Cuando se corren pruebas que no satisfacen la condición, dichas pruebas son descartadas.
La generacion de pruebas continua hasta que 100 casos se hayan encontrado o hasta un que se llega a un límite global del número de casos de prueba (para evitar problemas de bucles continuos si la condición nunca se mantiene). En este caso un mensaje similar a: "Arguments exhausted after 97 tests." indica que se encontraron 97 casos de prueba que satisfacen la condición, y que la propiedad se mantuvo en esos 97 casos.

Es importante notar que, en este caso, los valores tuvieron que ser restringidos a int. Esto se debe a que los valores generados deben ser comparables, pero esto no se refleja en los tipos. Entonces sin la restriccion  explicita, FsCheck podria generar listas que contengan tipos diferente (subtipos de objetos) y estos no son mutuamente comparables.
 
    
## Propiedades perezosas

Desde que F # tiene evaluación ansiosa por defecto, la propiedad anterior hace más trabajo de lo necesario: evalúa la propiedad a la derecha de la condición, sin importar el resultado de la condición de la izquierda. Mientras es sólo una consideración de rendimiento en el anterior ejemplo, esto puede limitar la expresividad de propiedades - considere:
*)

(***define-output: eager***)
let tooEager a = a <> 0 ==> (1/a = 1/a)
Check.Quick tooEager

(***include-output: eager***)

(**
La evaluación perezosa es necesaria aquí para asegurarse de que la propiedad sea verificada correctamente:

*)

(***define-output: lazy***)
let moreLazy a = a <> 0 ==> (lazy (1/a = 1/a))
Check.Quick moreLazy

(***include-output: lazy***)

(**   
## Propiedades cuantificadas

Algunas propiedades pueden tomar la forma:  `forAll <arbitrary>  (fun <args> -> <property>)`.

por ejemplo, *)

(***define-output:insertWithArb***)
let orderedList = Arb.from<list<int>> |> Arb.mapFilter List.sort ordered
let insertWithArb x = Prop.forAll orderedList (fun xs -> ordered(insert x xs))
Check.Quick insertWithArb

(***include-output:insertWithArb***)

(**
El primer argumento de forAll es una instancia de IArbitraty. Dicha instancia encapsula un generador de test data y un shrinker (más acerca de esto en [Test Data](TestData.html))
 
The first argument of forAll is an IArbitrary instance. Such an instance
encapsulates a test data generator and a shrinker (more on that in [Test Data](TestData.html)).
Mediante el suministro de un generador personalizado, en lugar de utilizar el generador predeterminado para ese tipo, es posible controlar la distribución de datos de prueba. En el ejemplo, mediante el suministro de un generador personalizado para listas ordenadas, en lugar de la eliminación de casos de prueba que no se piden, FsCheck garantiza que 100 pruebas casos se pueden generar sin alcanzar el límite global de casos de prueba.
Los combinadores para definir generadores serán descriptos en [Test Data](TestData.html).
    
## Esperando excepciones

Quizás el usuario desee probar que una funcion o metodo tira una excepción bajo ciertas circunstancias.

You may want to test that a function or method throws an exception under certain circumstances.
Use `throws<'e :> exn,'a> Lazy<'a>` para lograrlo. Por ejemplo:*)

(***define-output: expectDivideByZero***)
let expectDivideByZero() = Prop.throws<DivideByZeroException,_> (lazy (raise <| DivideByZeroException()))
Check.Quick expectDivideByZero

(***include-output: expectDivideByZero***)
 
(**
## Propiedades temporizadas

Las propiedades pueden tomar la forma `within <timeout in ms> <Lazy<property>>`

Por ejemplo,*)

(***hide***)
//TODO: figure out why this does not exit cleanly. If I eval,
//FSharp.Formatting formats this file nicely, but hangs on any
//subsequent file. I suspect this has to do with some thread
//not closing cleanly...
//(***define-output:timesOut***)

(***do-not-eval***)
let timesOut (a:int) =
    lazy
        if a>10 then
            do System.Threading.Thread.Sleep(3000)
            true
        else
            true
    |> Prop.within 1000
Check.Quick timesOut

(***hide***)
//(***include-output:timesOut***)

(**
El primer argumento es el tiempo que la propiedad perezosa puede ejecutar. Si se ejecuta por mas tiempo, Fscheck considera la prueba como fallida. De lo contrario, el resultado de la propiedad perezosa es el resultado de su interior. Tenga en cuenta que, aunque dentro de los intentos para cancelar el hilo en el cual la propiedad se ejecuta, que puede no tener éxito, por lo que el hilo puede realmente continuar funcionando hasta que termina el proceso.
    
## Observando la distribucion de los casos de prueba

It is important to be aware of the distribution of test cases: if test data is not well 
distributed then conclusions drawn from the test results may be invalid. In particular, 
the `==>` operator can skew the distribution of test data badly, since only test data which 
satisfies the given condition is used.

FsCheck provides several ways to observe the distribution of test data. Code for 
making observations is incorporated into the statement of properties, each time 
the property is actually tested the observation is made, and the collected observations 
are then summarized when testing is complete.

### Contando casos triviales 

Una propiedad puede tomar la forma `trivial <condition> <property>`

Por ejemplo: *)

(***define-output:insertTrivial***)
let insertTrivial (x:int) xs =
  ordered xs ==> (ordered (insert x xs))
  |> Prop.trivial (List.length xs = 0)
Check.Quick insertTrivial

(**
Los casos de prueba para la que la condición es verdadera se clasifican como trivial, y la proporción de casos de prueba triviales en el total se reportan:
*)

(***include-output:insertTrivial***)

(**
### Clasificación de los casos de prueba

Una propiedad puede tomar la forma `classify <condition> <string> <property>`

Por ejemplo,*)

(***define-output:insertClassify***)
let insertClassify (x:int) xs =
  ordered xs ==> (ordered (insert x xs))
  |> Prop.classify (ordered (x::xs)) "at-head"
  |> Prop.classify (ordered (xs @ [x])) "at-tail"
Check.Quick insertClassify

(**
Los casos de prueba que satisfacen una condición son asignados como la clasificación obtenida, y la distribución de clasificaciones se informa después de la prueba::*)

(***include-output:insertClassify***)

(**
Notese que los casos de prueba pueden tener mas de una clasificación.

### Recopilación de valores de datos

Una propiedad puede tomar la forma `collect <expression> <property>`

Por ejemplo,*)

(***define-output: insertCollect***)
let insertCollect (x:int) xs =
  ordered xs ==> (ordered (insert x xs))
      |> Prop.collect (List.length xs)
Check.Quick insertCollect

(**
El argumento de collect se evalúa en cada caso de prueba, y la distribución de valores es reportada. El tipo de este argumento se imprime utilizando `sprintf "%A"`:*)

(***include-output: insertCollect***)


(**  
### Combinando observaciones

Las observaciones descriptas aqui pueden ser cobinadas en cualquier manera. Todas las observaciones de cada prueba son combinadas, y la distribucion de esta combinacion reportada. Por ejemplo: *)

(***define-output:insertCombined***)
let insertCombined (x:int) xs =
    ordered xs ==> (ordered (insert x xs))
    |> Prop.classify (ordered (x::xs)) "at-head"
    |> Prop.classify (ordered (xs @ [x])) "at-tail"
    |> Prop.collect (List.length xs)
Check.Quick insertCombined

(***include-output:insertCombined***)

(**
## And, Or y etiquetas

Las propiedades pueden tomar la forma

* `<property> .&. <property>` tiene éxito si las ambas tienen éxito y falla si una de las propiedades falla. Es rechazada cuando ambas son rechazadas.
* `<property> .|. <property>` tiene exito si una tiene exito, falla si las dos propiedades fallan y es rechazadas cuando ambas son rechazadas.

El combinador `.&.` es más comúnmente utilizado para escribir las propiedades complejas que comparten un generador.
En estos casos, puede ser dificil establecer exactamente cual subpropiedad ha causado el fallo. 
Este es el motivo por el cual, se pueden nombrar subpropiedades y FsCheck muestra los nombres de las subpropiedades que fallaron cuando encuentra un contraejemplo. Esto toma la forma : `<string> @| <property>` or `<property> |@ <string>`.

Por ejemplo,*)

(***define-output:complex***)
let complex (m: int) (n: int) =
  let res = n + m
  (res >= m)    |@ "result > #1" .&.
  (res >= n)    |@ "result > #2" .&.
  (res < m + n) |@ "result not sum"
Check.Quick complex

(***include-output:complex***)

(**
Es perfectamente posible aplicar más de una etiqueta a una propiedad; Fscheck muestra todas las etiquetas aplicables.
Esto es útil para mostrar los resultados intermedios,  por ejemplo:*)

(***define-output:multiply***)
let multiply (n: int, m: int) =
    let res = n*m
    sprintf "evidence = %i" res @| (
      "div1" @| (m <> 0 ==> lazy (res / m = n)),
      "div2" @| (n <> 0 ==> lazy (res / n = m)),
      "lt1"  @| (res > m),
      "lt2"  @| (res > n))
Check.Quick multiply

(***include-output:multiply***)

(**
Observe que la propiedad anterior combina subpropiedades creando tuplas. Esto funciona para tuplas hasta longitud 6 y listas:

*    `(<property1>,<property2>,...,<property6>)` means `<property1> .&. <property2> .&.... .&.<property6>`
*    `[property1;property2,...,propertyN]` means `<property1> .&. <property2> .&.... .&.<propertyN>`

El mismo ejemplo escrito como una lista:*)

let multiplyAsList (n: int, m: int) =
    let res = n*m
    sprintf "evidence = %i" res @| [
      "div1" @| (m <> 0 ==> lazy (res / m = n));
      "div2" @| (n <> 0 ==> lazy (res / n = m));
      "lt1"  @| (res > m);
      "lt2"  @| (res > n)]
(**
Produce el mismo resultado.*)
