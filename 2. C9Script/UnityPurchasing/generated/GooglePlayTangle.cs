// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("GILwoZZ/kWRmEfu26X0u789RTO6WWjXFO+u2YIJB1ebknirM0OGfF6vDiLikDu9knY/cigHBBBSxuDmkv117ACEl4oItFHQCtSxah/E9zv//Evbk94BjYNIXFPDhKC28R6VOOVAwilK915kpjGcNOZlS8lPUBSCa5WZoZ1flZm1l5WZmZ6Qb+TXotWlA39chSVa2kQgDJKeFBiR3d9v/s5A53xlJ4+XO9pPiDTvGCP8ryD5YGKdwtveWkmVpRQ5xZkXTcJXIaSvUfe11/e4SBANuJfuJQTzpxF0eHa5TiGUE51P4eEsuNPrNe+6H5/ca+Ea/h46Q9pPLSE2Vmh5CUmzl5CBX5WZFV2phbk3hL+GQamZmZmJnZKeEIjaarZdwTGVkZmdm");
        private static int[] order = new int[] { 12,5,11,8,9,6,6,10,8,11,11,11,13,13,14 };
        private static int key = 103;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
