namespace Gramaticas.Domain
{
    public class Gramatica
    {
        private Dictionary<int, Produccion> Producciones;
        private List<Expresion> NoTerminales;
        private List<Expresion> Terminales;

        public Gramatica(Dictionary<int, Produccion> producciones)
        {
            Producciones = producciones;
            SetGramatica();
        }

        private void SetGramatica()
        {
            GetExpressions();
            SetProducciones();
            CalcularNoTerminalesAnulables();
            CalcularPrimerosNoTerminal();
            CalcularSiguientesNoTerminal();
            CalcularSeleccionProduccion();
        }

        private void CalcularSeleccionProduccion()
        {
            foreach (var pdn in Producciones.Values)
            {
                if (pdn.Anulable)
                {
                    pdn.Siguientes = pdn.Primeros;
                    pdn.Siguientes.AddRange(pdn.Izquierda.Siguientes);
                    pdn.Siguientes = pdn.Siguientes.Distinct().ToList();
                }
                else
                {
                    pdn.Siguientes = pdn.Primeros;
                }
            }
        }

        private void CalcularSiguientesNoTerminal()
        {
            foreach (var nt in NoTerminales)
            {
                var pdns = Producciones.Values.Where(_ => _.Derecha.Contains(nt));
                foreach (var pdn in pdns)
                {
                    if (pdn.Derecha.Last().Equals(nt))
                    {
                        var sig = pdn.Izquierda;
                        if (sig != nt)
                        {
                            if (sig.Siguientes.Any(_ => !_.Valor.Equals('¬')))
                            {
                                nt.Siguientes.AddRange(sig.Siguientes);
                            }
                            else
                            {
                                nt.Siguientes.Add(sig);
                            }
                        }
                    }
                    else
                    {
                        var i = pdn.Derecha.IndexOf(nt);
                        var sig = pdn.Derecha[i + 1];
                        if (sig.Terminal)
                        {
                            nt.Siguientes.Add(sig);
                        }
                        else
                        {
                            nt.Siguientes.AddRange(sig.Primeros);
                            if (sig.Anulable)
                            {
                                if (sig.Siguientes.Any())
                                {
                                    nt.Siguientes.AddRange(sig.Siguientes);
                                }
                                else
                                {
                                    nt.Siguientes.Add(sig);
                                }
                            }
                        }
                    }
                }
            }
            while (NoTerminales.Any(_ => _.Siguientes.Any(_ => !_.Terminal)))
            {
                foreach (var nt in NoTerminales)
                {
                    var ntSig = new List<Expresion>();
                    foreach (var sig in nt.Siguientes.Where(_ => !_.Equals(nt)))
                    {
                        if (sig.Terminal)
                        {
                            ntSig.Add(sig);
                        }
                        else
                        {
                            ntSig.AddRange(sig.Siguientes);
                        }
                    }
                    nt.Siguientes = ntSig.Distinct().OrderBy(_ => _.Valor).ToList();
                }
            }
        }

        private void CalcularPrimerosNoTerminal()
        {
            foreach (var produccion in Producciones.Values)
            {
                if (produccion.Derecha.Any())
                {
                    produccion.Izquierda.Primeros.Add(produccion.Derecha.First());
                    produccion.Primeros.Add(produccion.Derecha.First());
                }
            }
            foreach (var produccion in Producciones.Values)
            {
                var primero = produccion.Derecha.FirstOrDefault();
                if (primero != null && !primero.Terminal)
                {
                    produccion.Izquierda.Primeros.AddRange(primero.Primeros);
                    produccion.Primeros.AddRange(primero.Primeros);
                    if (primero.Anulable)
                    {
                        if (produccion.Derecha.Count > 1)
                        {
                            var segundo = produccion.Derecha.ElementAt(1);
                            if (segundo.Terminal)
                            {
                                produccion.Izquierda.Primeros.Add(segundo);
                                produccion.Primeros.Add(segundo);
                            }
                            else
                            {
                                produccion.Izquierda.Primeros.AddRange(segundo.Primeros);
                                produccion.Primeros.AddRange(segundo.Primeros);
                            }
                        }
                    }
                }
            }

            while (NoTerminales.Any(_ => _.Primeros.Any(_ => !_.Terminal)))
            {
                foreach (var nt in NoTerminales)
                {
                    var ntSig = new List<Expresion>();
                    foreach (var sig in nt.Primeros.Where(_ => !_.Equals(nt)))
                    {
                        if (sig.Terminal)
                        {
                            ntSig.Add(sig);
                        }
                        else
                        {
                            ntSig.AddRange(sig.Primeros);
                        }
                    }
                    nt.Primeros = ntSig.Distinct().OrderBy(_ => _.Valor).ToList();
                }
            }

            foreach (var pdn in Producciones.Values)
            {
                var ntSig = new List<Expresion>();
                foreach (var sig in pdn.Primeros)
                {
                    if (sig.Terminal)
                    {
                        ntSig.Add(sig);
                    }
                    else
                    {
                        ntSig.AddRange(sig.Primeros);
                    }
                }
                pdn.Primeros = ntSig.Distinct().OrderBy(_ => _.Valor).ToList();
            }
        }

        private void SetProducciones()
        {
            foreach (var p in Producciones.Values)
            {
                p.Izquierda = NoTerminales.FirstOrDefault(_ => _.Valor.Equals(p.Izquierda.Valor));
                var newDerecha = new List<Expresion>();
                foreach (var e in p.Derecha)
                {
                    if (e.Terminal)
                    {
                        newDerecha.Add(Terminales.FirstOrDefault(_ => _.Valor.Equals(e.Valor)));
                    }
                    else
                    {
                        newDerecha.Add(NoTerminales.FirstOrDefault(_ => _.Valor.Equals(e.Valor)));
                    }
                }
                p.Derecha = newDerecha;
            }
            Producciones.First().Value.Izquierda.Siguientes.Add(new Expresion('¬') { Terminal = true });
        }

        private void GetExpressions()
        {
            Terminales = new List<Expresion>();
            NoTerminales = new List<Expresion>();
            foreach (var produccion in Producciones.Values)
            {
                if (!NoTerminales.Any(_ => _.Valor.Equals(produccion.Izquierda.Valor)))
                {
                    NoTerminales.Add(produccion.Izquierda);
                }
                foreach (var d in produccion.Derecha)
                {
                    if (d.Terminal && !Terminales.Any(_ => _.Valor.Equals(d.Valor)))
                    {
                        Terminales.Add(d);
                    }
                }
            }
        }

        public void Print()
        {
            foreach (var kvp in Producciones)
            {
                Console.WriteLine($"{kvp.Key}. {kvp.Value}");
            }
            Console.WriteLine();
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        public void PrintSiguientesNoTerminal()
        {
            Console.WriteLine("Siguientes No terminales");
            foreach (var nt in NoTerminales)
            {
                Console.WriteLine($"Siguientes({nt}) = {string.Join(", ", nt.Siguientes)}");
            }
            Console.WriteLine();
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        public void PrintSeleccionProduccion()
        {
            Console.WriteLine("Seleccion Produccion");

            foreach (var pdns in Producciones)
            {
                Console.WriteLine($"Seleccion({pdns.Key}) = {string.Join(", ", pdns.Value.Primeros)}");
            }

            Console.WriteLine();
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        public void PrintNoTerminalesAnulables()
        {
            Console.WriteLine("No terminales anulables");
            Console.WriteLine(string.Join(" ", NoTerminales.Where(_ => _.Anulable)));
            Console.WriteLine();
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        public void PrintProduccionesAnulables()
        {
            Console.WriteLine("Producciones anulables");
            Console.WriteLine(string.Join(" ", Producciones.Where(_ => _.Value.Anulable).Select(_ => _.Key)));
            Console.WriteLine();
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        public void PrintPrimerosNoTerminal()
        {
            Console.WriteLine("Primeros No Terminal");
            foreach (var nt in NoTerminales)
            {
                var primeros = NoTerminales.First(_ => _.Equals(nt)).Primeros;
                Console.WriteLine($"Primeros({nt}) = {string.Join(", ", primeros)}");
            }
            Console.WriteLine();
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        public void PrintPrimerosProduccion()
        {
            Console.WriteLine("Primeros Produccion");

            foreach (var pdns in Producciones)
            {
                Console.WriteLine($"Primeros({pdns.Key}) = {string.Join(", ", pdns.Value.Primeros)}");
            }

            Console.WriteLine();
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        public void PrintSimbolos()
        {
            Console.WriteLine($"Símbolos Entrada = {string.Join(", ", Terminales)}");
            Console.WriteLine($"Símbolos Pila = {string.Join(", ", NoTerminales)}");
            Console.WriteLine();
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        private void CalcularNoTerminalesAnulables()
        {
            foreach (var nt in NoTerminales)
            {
                var pdns = Producciones.Values.Where(a => a.Izquierda.Equals(nt));
                nt.Anulable = pdns.Any(_ => !_.Derecha.Any());
            }
            foreach (var nt in NoTerminales)
            {
                if (!nt.Anulable)
                {
                    var pdns = Producciones.Values.Where(a => a.Izquierda.Equals(nt));
                    nt.Anulable = pdns.Any(_ => _.Derecha.All(_ => _.Anulable));
                }
            }
            foreach (var pdn in Producciones.Values)
            {
                pdn.Anulable = pdn.Derecha.All(_ => _.Anulable);
            }
        }
    }
}