namespace xcite.tdn {
    public class CharStream {

        public static readonly char EOF = (char) 2409;

        private readonly char[] _stream;
        private int _n;

        public CharStream(string str) : this(str.ToCharArray()) {

        }

        public CharStream(char[] stream) {
            _stream = stream;
        }

        public void Reset() {
            _n = 0;
        }

        public bool HasNext()
            => _n != _stream.Length;

        public char Next() {
            if (!HasNext()) return EOF;
            char c = _stream[_n++];
            
            if (c == '\n' || c == '\t')
                c = ' ';

            if (c == '\r') return Next();
            return c;
        }

        public char Peek() {
            if (!HasNext()) return EOF;
            return _stream[_n];
        }

        public void Shift() {
            while (Next() == ' ') {
            }

            _n--;
        }
    }
}