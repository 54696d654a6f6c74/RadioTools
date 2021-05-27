using System.Text;

namespace RadioTools
{
    public class Request
    {
        public byte[] reqEncoded;
        public byte[][] dataEncoded;

        public Request(Encoding encoder, string reqType, params string[] data)
        {
            dataEncoded = new byte[data.Length][];

            reqEncoded = encoder.GetBytes(reqType);

            for(int i = 0; i < data.Length; i++)
                dataEncoded[i] = encoder.GetBytes(data[i]);
        }

        public static byte[] ContainerizeString(Encoding encoder, string name)
        {
            if(name.Length > Settings.dat.cmdNameSize)
                return null;

            byte[] dataEncoded = encoder.GetBytes(name);
            byte[] container = new byte[Settings.dat.cmdNameSize];

            for(int i = 0; i < dataEncoded.Length; i++)
                container[i] = dataEncoded[i];

            return container;
        }
    }
}
