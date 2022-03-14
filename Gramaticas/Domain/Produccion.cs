namespace Gramaticas.Domain
{
    public class Produccion
    {
        public Expresion Izquierda { get; set; }
        public bool Anulable { get; set; }
        public List<Expresion> Derecha { get; set; }
        public List<Expresion> Primeros { get; set; }
        public List<Expresion> Siguientes { get; set; }

        public Produccion(char derecha, params char[] izquierda)
        {
            Primeros = new List<Expresion>();
            Izquierda = new Expresion(derecha);
            Derecha = new List<Expresion>();
            foreach (var i in izquierda)
            {
                var expresion = new Expresion(i);
                if (char.IsUpper(i))
                {
                    expresion.SetNoTerminal();
                }
                else
                {
                    expresion.SetTerminal();
                }
                Derecha.Add(expresion);
            }
        }

        public override string ToString()
        {
            return $"{Izquierda} -> {string.Join("", Derecha)}";
        }
    }
}