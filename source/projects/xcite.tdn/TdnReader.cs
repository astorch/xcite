namespace xcite.tdn {
    public class TdnReader {

        public TdnIterator Read(string content) {
            return new TdnIterator(content);
        }
    }
}
