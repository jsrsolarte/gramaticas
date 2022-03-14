namespace Gramaticas.Domain
{
    public class Expresion
    {
        public bool Terminal { get; set; }
        public bool Anulable { get; set; }
        public bool SecuenciaNula { get; set; }
        public char Valor { get; set; }
        public List<Expresion> Primeros { get; set; }
        public List<Expresion> Siguientes { get; set; }

        public Expresion(char valor)
        {
            Siguientes = new List<Expresion>();
            Valor = char.ToUpper(valor);
            Primeros = new List<Expresion>();
        }

        public override string ToString()
        {
            if (!Terminal)
            {
                return $"<{Valor}>";
            }
            else
            {
                return $"{char.ToLower(Valor)}";
            }
        }
    }
}