using Gramaticas.Domain;

namespace Gramaticas
{
    public static class Extensions
    {
        public static Expresion SetNoTerminal(this Expresion expresion)
        {
            expresion.Terminal = false;
            expresion.SecuenciaNula = false;
            return expresion;
        }

        public static Expresion SetTerminal(this Expresion expresion)
        {
            expresion.Terminal = true;
            expresion.SecuenciaNula = false;
            return expresion;
        }
    }
}