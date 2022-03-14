using Gramaticas;
using Gramaticas.Domain;

var dict = new Dictionary<int, Produccion>();
dict.Add(1, new Produccion('E', 'T', 'L'));
dict.Add(2, new Produccion('L', '+', 'T', 'L'));
dict.Add(3, new Produccion('L', '-', 'T', 'L'));
dict.Add(4, new Produccion('L'));
dict.Add(5, new Produccion('T', 'F', 'Y'));
dict.Add(6, new Produccion('Y', '*', 'F','Y'));
dict.Add(7, new Produccion('Y', '/', 'F', 'Y'));
dict.Add(8, new Produccion('Y', '%', 'F', 'Y'));
dict.Add(9, new Produccion('Y'));
dict.Add(10, new Produccion('F', 'P','R'));
dict.Add(11, new Produccion('R', '^','F'));
dict.Add(12, new Produccion('R'));
dict.Add(13, new Produccion('P', '(','E',')'));
dict.Add(14, new Produccion('P', 'i'));

var gramatica = new Gramatica(dict);

gramatica.Print();
gramatica.PrintNoTerminalesAnulables();
gramatica.PrintProduccionesAnulables();
gramatica.PrintPrimerosNoTerminal();
gramatica.PrintPrimerosProduccion();
gramatica.PrintSiguientesNoTerminal();
gramatica.PrintSeleccionProduccion();
gramatica.PrintSimbolos();

