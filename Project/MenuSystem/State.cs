namespace MenuSystem
{
    public class State
    {
        private readonly int _start;
        private readonly int _end;
        private int _index;

        public State(int startIndex, int endIndex)
        {
            this._start = startIndex;
            this._end = endIndex;
            this._index = startIndex;
        }

        public void IncreaseState()
        {
            _index = _index + 1 >= _end ? _start : _index + 1;
        }

        public void DecreaseState()
        {
            _index = _index - 1 < _start ? _end - 1 : _index - 1;
        }

        public int GetState()
        {
            return _index;
        }
    }
}
    
